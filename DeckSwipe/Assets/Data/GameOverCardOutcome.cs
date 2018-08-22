public class GameOverCardOutcome : CardActionOutcome {
	
	public override void Perform(Game controller) {
		controller.RestartGame();
	}
	
}
