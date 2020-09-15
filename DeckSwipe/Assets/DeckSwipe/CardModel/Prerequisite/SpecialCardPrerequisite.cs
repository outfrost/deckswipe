using System;
using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel.Prerequisite {

	[Serializable]
	public class SpecialCardPrerequisite : ICardPrerequisite {

		public string id;
		public CardStatus status;

		public CardStatus Status {
			get { return status; }
			set { status = value; }
		}

		public SpecialCardPrerequisite(string id) {
			this.id = id;
			status = CardStatus.None;
		}

		public ICard GetCard(CardStorage cardStorage) {
			return cardStorage.SpecialCard(id);
		}

	}

}
