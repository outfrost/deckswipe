using System;
using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.DrawQueue {

	[Serializable]
	public class SpecialFollowup : IFollowup {

		public string id;
		public int delay;

		public int Delay {
			get { return delay; }
			set { delay = value; }
		}

		public SpecialFollowup(string id, int delay) {
			this.id = id;
			this.delay = delay;
		}

		public IFollowup Clone() {
			return new SpecialFollowup(id, delay);
		}

		public ICard Fetch(CardStorage cardStorage) {
			return cardStorage.SpecialCard(id);
		}

	}

}
