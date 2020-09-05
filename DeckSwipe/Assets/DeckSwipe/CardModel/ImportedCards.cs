using System.Collections.Generic;

namespace DeckSwipe.CardModel {

	public struct ImportedCards {

		public readonly Dictionary<int, Card> cards;
		public readonly Dictionary<string, SpecialCard> specialCards;

		public ImportedCards(
				Dictionary<int, Card> cards,
				Dictionary<string, SpecialCard> specialCards) {
			this.cards = cards;
			this.specialCards = specialCards;
		}

	}

}
