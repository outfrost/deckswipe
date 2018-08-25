using UnityEngine;

public class CardModel {

	public readonly string CardText;
	public readonly string CharacterName;
	public readonly string LeftSwipeText;
	public readonly string RightSwipeText;
	public readonly Sprite CardImage;
	private CardActionOutcome leftSwipeOutcome;
	private CardActionOutcome rightSwipeOutcome;
	
	public CardModel(
			string cardText,
			string characterName,
			string leftSwipeText,
			string rightSwipeText,
			Sprite cardImage,
			CardActionOutcome leftOutcome,
			CardActionOutcome rightOutcome) {
		CardText = cardText;
		CharacterName = characterName;
		LeftSwipeText = leftSwipeText;
		RightSwipeText = rightSwipeText;
		CardImage = cardImage;
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
