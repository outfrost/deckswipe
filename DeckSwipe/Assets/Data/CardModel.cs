using UnityEngine;

public class CardModel {
	
	public readonly string CardText;
	public readonly string LeftSwipeText;
	public readonly string RightSwipeText;
	
	public string CharacterName {
		get { return character.Name; }
	}
	
	public Sprite CardSprite {
		get { return character.Sprite; }
	}
	
	private CharacterModel character;
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
		this.character = character;
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
