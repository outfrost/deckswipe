using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.DrawQueue {
	
	public class FollowupCard : IFollowupCard {
		
		public int id;
		public int delay;
		
		public int Delay {
			get { return delay; }
			set { delay = value; }
		}
		
		public FollowupCard(int id, int delay) {
			this.id = id;
			this.delay = delay;
		}
		
		public IFollowupCard Clone() {
			return new FollowupCard(id, delay);
		}
		
		public global::DeckSwipe.CardModel.CardModel Fetch(CardStorage cardStorage) {
			return cardStorage.ForId(id);
		}
		
	}
	
}
