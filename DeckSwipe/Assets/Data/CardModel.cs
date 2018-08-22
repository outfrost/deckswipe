public class CardModel {
	
	private CardActionOutcome leftSwipeOutcome;
	private CardActionOutcome rightSwipeOutcome;
	
	public CardModel(CardActionOutcome leftOutcome, CardActionOutcome rightOutcome) {
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
