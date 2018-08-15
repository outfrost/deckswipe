public class CardDecisionOutcome {

	private readonly StatsModification statsModification;

	public CardDecisionOutcome(int heatMod, int foodMod, int hopeMod, int materialsMod) {
		statsModification = new StatsModification(heatMod, foodMod, hopeMod, materialsMod);
	}

	public void Perform() {
		statsModification.Perform();
	}

}
