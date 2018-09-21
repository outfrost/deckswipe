using System.Collections.Generic;

namespace DeckSwipe.CardModel {
	
	public struct ImportedCards {
		
		public readonly Dictionary<int, CardModel> cards;
		public readonly Dictionary<string, CardModel> specialCards;
		
		public ImportedCards(Dictionary<int, CardModel> cards,
				Dictionary<string, CardModel> specialCards) {
			this.cards = cards;
			this.specialCards = specialCards;
		}
		
	}
	
}
