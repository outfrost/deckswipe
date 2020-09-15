using System;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoCharacter {

		public int id;
		public string name;
		public int imageId;

		public ProtoCharacter() {}

		public ProtoCharacter(
				int id,
				string name,
				int imageId) {
			this.id = id;
			this.name = name;
			this.imageId = imageId;
		}

	}

}
