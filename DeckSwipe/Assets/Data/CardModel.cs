public class CardModel {

	private CardDecisionOutcome leftDecisionOutcome;
	private CardDecisionOutcome rightDecisionOutcome;

	public CardModel(CardDecisionOutcome leftOutcome, CardDecisionOutcome rightOutcome) {
		leftDecisionOutcome = leftOutcome;
		rightDecisionOutcome = rightOutcome;
	}
	
	public void PerformLeftDecision() {
		leftDecisionOutcome.Perform();
	}
	
	public void PerformRightDecision() {
		rightDecisionOutcome.Perform();
	}
	
}
