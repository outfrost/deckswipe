namespace DeckSwipe.CardModel {
	
	public class GameOverOutcome : ActionOutcome {
		
		public override void Perform(Game controller) {
			controller.RestartGame();
		}
		
	}
	
}
