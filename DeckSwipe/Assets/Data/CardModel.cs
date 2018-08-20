public class CardModel {
	
	private CardActionOutcome leftSwipeOutcome;
	private CardActionOutcome rightSwipeOutcome;
	
	public CardModel(CardActionOutcome leftOutcome, CardActionOutcome rightOutcome) {
		leftSwipeOutcome = leftOutcome;
		rightSwipeOutcome = rightOutcome;
	}
	
	public void PerformLeftDecision() {
		leftSwipeOutcome.Perform();
	}
	
	public void PerformRightDecision() {
		rightSwipeOutcome.Perform();
	}
	
}
