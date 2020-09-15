using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DeckSwipe.CardModel.DrawQueue;
using DeckSwipe.CardModel.Import.Resource;
using DeckSwipe.CardModel.Prerequisite;
using DeckSwipe.Gamestate;
using Outfrost;
using UnityEngine;

namespace DeckSwipe.CardModel.Import {

	public class CollectionImporter {

		private readonly Sprite defaultSprite;
		private readonly bool remoteCollectionFirst;

		public CollectionImporter(Sprite defaultSprite, bool remoteCollectionFirst) {
			this.defaultSprite = defaultSprite;
			this.remoteCollectionFirst = remoteCollectionFirst;
		}

		public async Task<ImportedCards> Import() {
			ProtoCollection collection;
			if (remoteCollectionFirst) {
				try {
					collection = await new GoogleSheetsImporter().Fetch();
				}
				catch (Exception e) when (e is WebException || e is FormatException) {
					Debug.LogError("[CollectionImporter] Import from Google Sheets failed: " + e.Message);
					Debug.LogWarning("[CollectionImporter] Trying local collection...");
					collection = LocalCollection.Fetch();
					if (collection.cards.Count == 0) {
						Debug.LogWarning("[CollectionImporter] Import from local collection returned 0 cards");
					}
				}
			}
			else {
				collection = LocalCollection.Fetch();
				if (collection.cards.Count == 0) {
					Debug.LogWarning("[CollectionImporter] Import from local collection returned 0 cards, "
							+ "trying Google Sheets...");
					try {
						collection = await new GoogleSheetsImporter().Fetch();
					}
					catch (Exception e) when (e is WebException || e is FormatException) {
						Debug.LogError("[CollectionImporter] Import from Google Sheets failed: " + e.Message);
					}
				}
			}

			Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
			foreach (ProtoImage image in collection.images) {
				if (sprites.ContainsKey(image.id)) {
					Debug.LogWarning("[CollectionImporter] Duplicate id found in Images");
					continue;
				}
				if (image.url == null) {
					Debug.LogWarning("[CollectionImporter] Image (id: " + image.id + ") has a null URL");
					continue;
				}

				Debug.Log("[CollectionImporter] Fetching image from " + image.url + " ...");
				HttpWebResponse imageResponse;
				try {
					HttpWebRequest imageRequest = WebRequest.CreateHttp(image.url);
					imageResponse = (HttpWebResponse) await imageRequest.GetResponseAsync();
				}
				catch (WebException e) {
					Debug.LogError("[CollectionImporter] Request failed: " + e.Message);
					continue;
				}

				if (imageResponse.StatusCode != HttpStatusCode.OK) {
					Debug.LogWarning(
							"[CollectionImporter] "
							+ image.url
							+ ": "
							+ (int)imageResponse.StatusCode
							+ " "
							+ imageResponse.StatusDescription);
					continue;
				}

				Stream imageStream;
				if ((imageStream = imageResponse.GetResponseStream()) == null) {
					Debug.LogWarning(
							"[CollectionImporter] Remote host returned no image in response (URL: "
							+ image.url
							+ ")");
					continue;
				}

				byte[] imageData = Util.BytesFromStream(imageStream);
				Texture2D texture = new Texture2D(1, 1);
				if (!texture.LoadImage(imageData)) {
					Debug.LogWarning(
							"[CollectionImporter] Could not create sprite texture from image (URL: "
							+ image.url
							+ ")");
					continue;
				}

				Sprite sprite = Sprite.Create(
						texture,
						new Rect(0.0f, 0.0f, texture.width, texture.height),
						new Vector2(0.5f, 0.5f));
				sprites.Add(image.id, sprite);
			}

			Dictionary<int, Character> characters = new Dictionary<int, Character>();
			foreach (ProtoCharacter protoCharacter in collection.characters) {
				if (characters.ContainsKey(protoCharacter.id)) {
					Debug.LogWarning("[CollectionImporter] Duplicate id found in Characters");
					continue;
				}
				Character character = new Character(protoCharacter.name, defaultSprite);
				sprites.TryGetValue(protoCharacter.imageId, out character.sprite);
				characters.Add(protoCharacter.id, character);
			}

			Dictionary<int, Card> cards = new Dictionary<int, Card>();
			foreach (ProtoCard protoCard in collection.cards) {
				if (cards.ContainsKey(protoCard.id)) {
					Debug.LogWarning("[CollectionImporter] Duplicate id found in Cards");
					continue;
				}

				IFollowup leftActionFollowup = null;
				ProtoAction leftAction = protoCard.leftAction;
				if (leftAction.followup != null && leftAction.followup.Count > 0) {
					if (leftAction.followup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] Card (id: "
								+ protoCard.id
								+ ") left action has more than one Followup");
						continue;
					}
					leftActionFollowup = leftAction.followup[0];
				}
				if (leftAction.specialFollowup != null && leftAction.specialFollowup.Count > 0) {
					if (leftAction.specialFollowup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] Card (id: "
								+ protoCard.id
								+ ") left action has more than one SpecialFollowup");
						continue;
					}
					if (leftActionFollowup != null) {
						Debug.LogWarning(
								"[CollectionImporter] Card (id: "
								+ protoCard.id
								+ ") left action has both types of followups");
						continue;
					}
					leftActionFollowup = leftAction.specialFollowup[0];
				}

				IFollowup rightActionFollowup = null;
				ProtoAction rightAction = protoCard.rightAction;
				if (rightAction.followup != null && rightAction.followup.Count > 0) {
					if (rightAction.followup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] Card (id: "
								+ protoCard.id
								+ ") right action has more than one Followup");
						continue;
					}
					rightActionFollowup = rightAction.followup[0];
				}
				if (rightAction.specialFollowup != null && rightAction.specialFollowup.Count > 0) {
					if (rightAction.specialFollowup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] Card (id: "
								+ protoCard.id
								+ ") right action has more than one SpecialFollowup");
						continue;
					}
					if (rightActionFollowup != null) {
						Debug.LogWarning(
								"[CollectionImporter] Card (id: "
								+ protoCard.id
								+ ") right action has both types of followups");
						continue;
					}
					rightActionFollowup = rightAction.specialFollowup[0];
				}

				ActionOutcome leftActionOutcome = new ActionOutcome(leftAction.statsModification, leftActionFollowup);
				ActionOutcome rightActionOutcome = new ActionOutcome(rightAction.statsModification, rightActionFollowup);

				List<ICardPrerequisite> prerequisites = new List<ICardPrerequisite>();

				bool failed = false;
				foreach (ProtoCardPrerequisite prereq in protoCard.cardPrerequisites) {
					CardPrerequisite cardPrerequisite = new CardPrerequisite(prereq.id);
					try {
						foreach (string s in prereq.status) {
							cardPrerequisite.Status |= CardStatusFor(s);
						}
					}
					catch (ArgumentException e) {
						Debug.LogWarning("[CollectionImporter] Card (id: " + protoCard.id + "): " + e.Message);
						failed = true;
						break;
					}
					prerequisites.Add(cardPrerequisite);
				}
				if (failed) {
					continue;
				}

				foreach (ProtoSpecialCardPrerequisite prereq in protoCard.specialCardPrerequisites) {
					SpecialCardPrerequisite cardPrerequisite = new SpecialCardPrerequisite(prereq.id);
					try {
						foreach (string s in prereq.status) {
							cardPrerequisite.Status |= CardStatusFor(s);
						}
					}
					catch (ArgumentException e) {
						Debug.LogWarning("[CollectionImporter] Card (id: " + protoCard.id + "): " + e.Message);
						failed = true;
						break;
					}
					prerequisites.Add(cardPrerequisite);
				}
				if (failed) {
					continue;
				}

				Card card = new Card(
						protoCard.cardText,
						leftAction.text,
						rightAction.text,
						null,
						leftActionOutcome,
						rightActionOutcome,
						prerequisites);

				characters.TryGetValue(protoCard.characterId, out card.character);

				cards.Add(protoCard.id, card);
			}

			Dictionary<string, SpecialCard> specialCards = new Dictionary<string, SpecialCard>();
			foreach (ProtoSpecialCard protoSpecialCard in collection.specialCards) {
				if (protoSpecialCard.id == null) {
					Debug.LogWarning("[CollectionImporter] Null id found in SpecialCards");
					continue;
				}
				if (specialCards.ContainsKey(protoSpecialCard.id)) {
					Debug.LogWarning("[CollectionImporter] Duplicate id found in SpecialCards");
					continue;
				}

				IFollowup leftActionFollowup = null;
				ProtoSpecialAction leftAction = protoSpecialCard.leftAction;
				if (leftAction.followup != null && leftAction.followup.Count > 0) {
					if (leftAction.followup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] SpecialCard (id: "
								+ protoSpecialCard.id
								+ ") left action has more than one Followup");
						continue;
					}
					leftActionFollowup = leftAction.followup[0];
				}
				if (leftAction.specialFollowup != null && leftAction.specialFollowup.Count > 0) {
					if (leftAction.specialFollowup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] SpecialCard (id: "
								+ protoSpecialCard.id
								+ ") left action has more than one SpecialFollowup");
						continue;
					}
					if (leftActionFollowup != null) {
						Debug.LogWarning(
								"[CollectionImporter] SpecialCard (id: "
								+ protoSpecialCard.id
								+ ") left action has both types of followups");
						continue;
					}
					leftActionFollowup = leftAction.specialFollowup[0];
				}

				IFollowup rightActionFollowup = null;
				ProtoSpecialAction rightAction = protoSpecialCard.rightAction;
				if (rightAction.followup != null && rightAction.followup.Count > 0) {
					if (rightAction.followup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] SpecialCard (id: "
								+ protoSpecialCard.id
								+ ") right action has more than one Followup");
						continue;
					}
					rightActionFollowup = rightAction.followup[0];
				}
				if (rightAction.specialFollowup != null && rightAction.specialFollowup.Count > 0) {
					if (rightAction.specialFollowup.Count > 1) {
						Debug.LogWarning(
								"[CollectionImporter] SpecialCard (id: "
								+ protoSpecialCard.id
								+ ") right action has more than one SpecialFollowup");
						continue;
					}
					if (rightActionFollowup != null) {
						Debug.LogWarning(
								"[CollectionImporter] SpecialCard (id: "
								+ protoSpecialCard.id
								+ ") right action has both types of followups");
						continue;
					}
					rightActionFollowup = rightAction.specialFollowup[0];
				}

				IActionOutcome leftActionOutcome = null;
				IActionOutcome rightActionOutcome = null;

				if (protoSpecialCard.id == "gameover_coal"
						|| protoSpecialCard.id == "gameover_food"
						|| protoSpecialCard.id == "gameover_health"
						|| protoSpecialCard.id == "gameover_hope") {
					leftActionOutcome = new GameOverOutcome();
					rightActionOutcome = new GameOverOutcome();
				}

				SpecialCard specialCard = new SpecialCard(
						protoSpecialCard.cardText,
						leftAction.text,
						rightAction.text,
						null,
						leftActionOutcome,
						rightActionOutcome);

				characters.TryGetValue(protoSpecialCard.characterId, out specialCard.character);

				specialCards.Add(protoSpecialCard.id, specialCard);
			}

			return new ImportedCards(cards, specialCards);
		}

		private CardStatus CardStatusFor(string s) {
			foreach (CardStatus status in Enum.GetValues(typeof(CardStatus))) {
				if (s == Enum.GetName(typeof(CardStatus), status)) {
					return status;
				}
			}
			throw new ArgumentException("No CardStatus value for \"" + s + "\"");
		}

	}

}
