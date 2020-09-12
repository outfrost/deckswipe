using System.Collections.Generic;

namespace DeckSwipe.CardModel.Import {

	public class ProtoCollection {

		public readonly List<ProtoCard> cards;
		public readonly List<ProtoSpecialCard> specialCards;
		public readonly List<ProtoCharacter> characters;
		public readonly List<ProtoImage> images;

		public ProtoCollection(
				List<ProtoCard> cards,
				List<ProtoSpecialCard> specialCards,
				List<ProtoCharacter> characters,
				List<ProtoImage> images) {
			this.cards = cards;
			this.specialCards = specialCards;
			this.characters = characters;
			this.images = images;
		}

	}

}
