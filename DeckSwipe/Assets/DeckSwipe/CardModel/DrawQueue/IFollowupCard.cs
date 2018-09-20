using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.DrawQueue {

	public interface IFollowupCard {
	
		int Delay { get; set; }
	
		IFollowupCard Clone();
		global::DeckSwipe.CardModel.CardModel Fetch(CardStorage cardStorage);

	}

}
