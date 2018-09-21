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
		
		public bool IsSatisfied(CardStorage cardStorage) {
			global::DeckSwipe.CardModel.CardModel card = cardStorage.SpecialCard(id);
			return (card?.progress.Status & status) == status;
		}
		
	}
	
}
