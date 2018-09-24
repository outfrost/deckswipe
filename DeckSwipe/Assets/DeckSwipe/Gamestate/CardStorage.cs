using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeckSwipe.CardModel;
using DeckSwipe.CardModel.Prerequisite;
using DeckSwipe.CardModel.Resource;
using UnityEngine;

namespace DeckSwipe.Gamestate {
	
	public class CardStorage {
		
		private static readonly Character _defaultGameOverCharacter = new Character("", null);
		
		private readonly Sprite defaultSprite;
		
		public Dictionary<int, Card> Cards { get; private set; }
		public Dictionary<string, Card> SpecialCards { get; private set; }
		
		public Task CardCollectionImport { get; }
		
		private List<Card> drawableCards = new List<Card>();
		
		public CardStorage(Sprite defaultSprite) {
			this.defaultSprite = defaultSprite;
			CardCollectionImport = PopulateCollection();
		}
		
		public Card Random() {
			return drawableCards[UnityEngine.Random.Range(0, drawableCards.Count)];
		}
		
		public Card ForId(int id) {
			Card card;
			Cards.TryGetValue(id, out card);
			return card;
		}
		
		public Card SpecialCard(string id) {
			Card card;
			SpecialCards.TryGetValue(id, out card);
			return card;
		}
		
		public void ResolvePrerequisites() {
			foreach (Card card in Cards.Values) {
				card.ResolvePrerequisites(this);
				if (card.PrerequisitesSatisfied()) {
					AddDrawableCard(card);
				}
			}
		}
		
		public void AddDrawableCard(Card card) {
			drawableCards.Add(card);
		}
		
		private async Task PopulateCollection() {
			ImportedCards importedCards = await new GoogleSheetsImporter(defaultSprite).FetchCards();
			Cards = importedCards.cards;
			SpecialCards = importedCards.specialCards;
			if (Cards == null || Cards.Count == 0) {
				PopulateFallback();
			}
			VerifySpecialCards();
		}
		
		private void PopulateFallback() {
			Cards = new Dictionary<int, Card>();
			Character leadExplorer = new Character("Lead explorer", defaultSprite);
			Cards.Add(0, new Card("This is a test card text that should appear on screen.",
					"Here goes",
					"There goes",
					new Character("Dr. Bartholomew Oobleck", defaultSprite), 
					new ActionOutcome(-4, 4, -2, 2),
					new ActionOutcome(2, 0, 4, -2),
					new List<ICardPrerequisite>()));
			Cards.Add(1, new Card("And this is yet another card that you may encounter in the game. Are you having fun thus far?",
					"No",
					"Yes",
					new Character("Peter Port", defaultSprite),
					new ActionOutcome(-1, -1, -1, -1),
					new ActionOutcome(2, 2, 2, 2),
					new List<ICardPrerequisite>()));
			Cards.Add(2, new Card("The hothouse is very cold. The crops are freezing up. What should we do?",
					"Turn on the heater in the hothouse",
					"Put the generator in overdrive",
					new Character("Oliver Rain", defaultSprite),
					new ActionOutcome(1, 1, 0, -2),
					new ActionOutcome(2, 2, -2, -4),
					new List<ICardPrerequisite>()));
			Cards.Add(3, new Card("The scouts come across what looks to be a wrecked ship. Should they explore it?",
					"No",
					"Yes",
					leadExplorer,
					new ActionOutcome(0, 0, 0, 0),
					new ActionOutcome(0, 0, 0, 0),
					new List<ICardPrerequisite>()));
			Cards.Add(4, new Card("The scouts find 2 steam cores and a shipment of wood on board the ship. A crew had been trying to survive in the wreck but everybody's dead now.",
					"",
					"",
					leadExplorer,
					new ActionOutcome(3, 0, 2, 6),
					new ActionOutcome(3, 0, 2, 6),
					new List<ICardPrerequisite>()));
			Cards.Add(5, new Card("The guards bring a woman to you. She yells \"These swine have been pestering me day in day out! How much does it take to tell them to leave me alone?\".",
					"Reprimend the guards",
					"Leave it be",
					new Character("", defaultSprite),
					new ActionOutcome(0, 1, 1, 0),
					new ActionOutcome(0, 0, -1, 0),
					new List<ICardPrerequisite>()));
			Cards.Add(6, new Card("An automaton got stuck in a hothouse. What do we do with it?",
					"Disable the automaton",
					"Disable the hothouse",
					new Character("Alice Woodrow", defaultSprite),
					new ActionOutcome(-2, -2, 0, -2),
					new ActionOutcome(+2, -4, 0, -2),
					new List<ICardPrerequisite>()));
		}
		
		private void VerifySpecialCards() {
			if (SpecialCards == null) {
				SpecialCards = new Dictionary<string, Card>();
			}
			
			if (!SpecialCards.ContainsKey("gameover_coal")) {
				SpecialCards.Add("gameover_coal", new Card("The city runs out of coal to run the generator, and freezes over.", "", "",
						_defaultGameOverCharacter,
						new GameOverOutcome(),
						new GameOverOutcome(),
						null));
			}
			if (!SpecialCards.ContainsKey("gameover_food")) {
				SpecialCards.Add("gameover_food", new Card("Hunger consumes the city, as food reserves deplete.", "", "",
						_defaultGameOverCharacter,
						new GameOverOutcome(),
						new GameOverOutcome(),
						null));
			}
			if (!SpecialCards.ContainsKey("gameover_health")) {
				SpecialCards.Add("gameover_health", new Card("The city's population succumbs to wounds and spreading diseases.", "", "",
						_defaultGameOverCharacter,
						new GameOverOutcome(),
						new GameOverOutcome(),
						null));
			}
			if (!SpecialCards.ContainsKey("gameover_hope")) {
				SpecialCards.Add("gameover_hope", new Card("All hope among the people is lost.", "", "",
						_defaultGameOverCharacter,
						new GameOverOutcome(),
						new GameOverOutcome(),
						null));
			}
		}
		
	}
	
}
