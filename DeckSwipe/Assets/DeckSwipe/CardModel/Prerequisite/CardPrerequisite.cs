using System;
using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.Prerequisite {

	[Serializable]
	public class CardPrerequisite : ICardPrerequisite {

		public int id;
		public CardStatus status;

		public CardStatus Status {
			get { return status; }
			set { status = value; }
		}

		public CardPrerequisite(int id) {
			this.id = id;
			status = CardStatus.None;
		}

		public ICard GetCard(CardStorage cardStorage) {
			return cardStorage.ForId(id);
		}

	}

}
