using System;

namespace DeckSwipe.Gamestate {
	
	[Serializable]
	public class SpecialCardProgress : ICardProgress {
		
		public string id;
		public CardStatus status;
		
		public CardStatus Status {
			get { return status; }
			set { status = value; }
		}
		
		public SpecialCardProgress(string id, CardStatus status) {
			this.id = id;
			this.status = status;
		}
		
	}
	
}
