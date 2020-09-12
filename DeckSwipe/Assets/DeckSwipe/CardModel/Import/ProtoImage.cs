using System;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoImage {

		public int id;
		public string url;

		public ProtoImage() {}

		public ProtoImage(
				int id,
				string url) {
			this.id = id;
			this.url = url;
		}

	}

}
