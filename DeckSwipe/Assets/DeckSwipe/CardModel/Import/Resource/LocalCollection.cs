using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DeckSwipe.CardModel.Import;
using Outfrost;
using UnityEngine;

namespace DeckSwipe.CardModel.Import.Resource {

	public static class LocalCollection {

		private const string _cardsPath = "Collection/Cards";
		private const string _specialCardsPath = "Collection/SpecialCards";
		private const string _charactersPath = "Collection/Characters";
		private const string _imagesPath = "Collection/Images";

		public static ProtoCollection Fetch() {
			List<ProtoCard> cards = JsonResources.Load<ProtoCard>(_cardsPath);
			List<ProtoSpecialCard> specialCards = JsonResources.Load<ProtoSpecialCard>(_specialCardsPath);
			List<ProtoCharacter> characters = JsonResources.Load<ProtoCharacter>(_charactersPath);
			List<ProtoImage> images = JsonResources.Load<ProtoImage>(_imagesPath);

			Debug.Log("[LocalCollection] Loaded " + cards.Count + " cards");
			Debug.Log("[LocalCollection] Loaded " + specialCards.Count + " special cards");
			Debug.Log("[LocalCollection] Loaded " + characters.Count + " characters");
			Debug.Log("[LocalCollection] Loaded " + images.Count + " images");

			return new ProtoCollection(cards, specialCards, characters, images);
		}

	}

}
