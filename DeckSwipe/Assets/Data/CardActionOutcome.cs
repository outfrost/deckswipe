public class CardActionOutcome {
	
	private readonly StatsModification statsModification;
	
	public CardActionOutcome() {
		statsModification = new StatsModification(0, 0, 0, 0);
	}
	
	public CardActionOutcome(int coalMod, int foodMod, int healthMod, int hopeMod) {
		statsModification = new StatsModification(coalMod, foodMod, healthMod, hopeMod);
	}
	
	public virtual void Perform(Game controller) {
		statsModification.Perform();
		controller.CardActionPerformed();
	}
	
}
