using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.Prerequisite {
	
	public interface ICardPrerequisite {
		
		CardStatus Status { get; }
		
		Card GetCard(CardStorage cardStorage);
		
	}
	
}
