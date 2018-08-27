using UnityEngine;

public class CardModel {
	
	public readonly string CardText;
	public readonly string LeftSwipeText;
	public readonly string RightSwipeText;
	public CharacterModel Character;
	
	public string CharacterName {
		get { return Character != null ? Character.Name : ""; }
	}
	
	public Sprite CardSprite {
		get { return Character?.Sprite; }
	}
	
	private CardActionOutcome leftSwipeOutcome;
	private CardActionOutcome rightSwipeOutcome;
	
	public CardModel(
			string cardText,
			string leftSwipeText,
			string rightSwipeText,
			CharacterModel character,
			CardActionOutcome leftOutcome,
			CardActionOutcome rightOutcome) {
		CardText = cardText;
		LeftSwipeText = leftSwipeText;
		RightSwipeText = rightSwipeText;
		Character = character;
		leftSwipeOutcome = leftOutcome;
		rightSwipeOutcome = rightOutcome;
	}
	
	public void PerformLeftDecision(Game controller) {
		leftSwipeOutcome.Perform(controller);
	}
	
	public void PerformRightDecision(Game controller) {
		rightSwipeOutcome.Perform(controller);
	}
	
}
