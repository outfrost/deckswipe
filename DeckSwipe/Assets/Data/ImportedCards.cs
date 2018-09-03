using System.Collections.Generic;

public struct ImportedCards {
	
	public Dictionary<int, CardModel> Cards;
	public Dictionary<string, CardModel> SpecialCards;
	
	public ImportedCards(Dictionary<int, CardModel> cards,
			Dictionary<string, CardModel> specialCards) {
		Cards = cards;
		SpecialCards = specialCards;
	}
	
}
