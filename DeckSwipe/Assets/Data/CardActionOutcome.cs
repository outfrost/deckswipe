public class CardActionOutcome {
	
	private readonly StatsModification statsModification;
	
	public CardActionOutcome() {
		statsModification = new StatsModification(0, 0, 0, 0);
	}
	
	public CardActionOutcome(int heatMod, int foodMod, int hopeMod, int materialsMod) {
		statsModification = new StatsModification(heatMod, foodMod, hopeMod, materialsMod);
	}
	
	public virtual void Perform(Game controller) {
		statsModification.Perform();
		controller.DrawNextCard();
	}
	
}
