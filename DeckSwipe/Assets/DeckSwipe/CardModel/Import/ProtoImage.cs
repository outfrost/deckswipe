using System;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoImage {

		public int id;
		public bool localFirst;
		public string url;

		public ProtoImage() {}

		public ProtoImage(
				int id,
				bool localFirst,
				string url) {
			this.id = id;
			this.localFirst = localFirst;
			this.url = url;
		}

	}

}
