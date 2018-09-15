using System;
using Persistence;

[Serializable]
public class CardPrerequisite : ICardPrerequisite {
	
	public int id;
	public CardStatus status;
	
	public CardStatus Status {
		get { return status; }
		set { status = value; }
	}
	
	public bool IsSatisfied(CardStorage cardStorage) {
		CardModel card = cardStorage.ForId(id);
		return (card?.progress.Status & status) == status;
	}
	
}
