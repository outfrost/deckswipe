using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.DrawQueue {
	
	public interface IFollowup {
		
		int Delay { get; set; }
		
		IFollowup Clone();
		Card Fetch(CardStorage cardStorage);
		
	}
	
}
