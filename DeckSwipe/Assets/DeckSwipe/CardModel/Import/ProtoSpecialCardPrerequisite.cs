using System;
using System.Collections.Generic;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoSpecialCardPrerequisite {

		public string id;
		public List<string> status;

		public ProtoSpecialCardPrerequisite(string id, List<string> status) {
			this.id = id;
			this.status = status;
		}

	}

}
