using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.DrawQueue {

	public class FollowupSpecialCard : IFollowupCard {
	
		public string id;
		public int delay;
	
		public int Delay {
			get { return delay; }
			set { delay = value; }
		}
	
		public FollowupSpecialCard(string id, int delay) {
			this.id = id;
			this.delay = delay;
		}
	
		public IFollowupCard Clone() {
			return new FollowupSpecialCard(id, delay);
		}
	
		public global::DeckSwipe.CardModel.CardModel Fetch(CardStorage cardStorage) {
			return cardStorage.SpecialCard(id);
		}
	
	}

}
