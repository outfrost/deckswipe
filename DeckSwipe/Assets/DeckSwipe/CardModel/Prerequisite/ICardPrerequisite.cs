using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.Prerequisite {

	public interface ICardPrerequisite {

		CardStatus Status { get; }

		ICard GetCard(CardStorage cardStorage);
		
	}

}
