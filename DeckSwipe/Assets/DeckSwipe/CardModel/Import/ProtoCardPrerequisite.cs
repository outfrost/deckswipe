using System;
using System.Collections.Generic;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoCardPrerequisite {

		public int id;
		public List<string> status;

		public ProtoCardPrerequisite(int id, List<string> status) {
			this.id = id;
			this.status = status;
		}

	}

}
