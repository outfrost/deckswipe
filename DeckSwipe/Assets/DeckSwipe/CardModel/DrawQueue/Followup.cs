using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.DrawQueue {
	
	public class Followup : IFollowup {
		
		public int id;
		public int delay;
		
		public int Delay {
			get { return delay; }
			set { delay = value; }
		}
		
		public Followup(int id, int delay) {
			this.id = id;
			this.delay = delay;
		}
		
		public IFollowup Clone() {
			return new Followup(id, delay);
		}
		
		public Card Fetch(CardStorage cardStorage) {
			return cardStorage.ForId(id);
		}
		
	}
	
}
