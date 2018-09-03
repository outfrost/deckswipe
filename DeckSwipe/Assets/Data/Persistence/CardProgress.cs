using System;

namespace Persistence {
	
	[Serializable]
	public class CardProgress : ICardProgress {
		
		public int id;
		public CardStatus status;
		
		public CardStatus Status {
			get { return status; }
			set { status = value; }
		}
		
		public CardProgress(int id, CardStatus status) {
			this.id = id;
			this.status = status;
		}
		
	}
	
}
