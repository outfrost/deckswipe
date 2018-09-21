using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.Prerequisite {
	
	public interface ICardPrerequisite {
		
		CardStatus Status { get; set; }
		
		bool IsSatisfied(CardStorage cardStorage);
		
	}
	
}
