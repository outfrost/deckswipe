using DeckSwipe.CardModel.DrawQueue;

namespace DeckSwipe.CardModel {

	public class ActionOutcome : IActionOutcome {

		private readonly StatsModification statsModification;
		private readonly IFollowup followup;

		public ActionOutcome() {
			statsModification = new StatsModification(0, 0, 0, 0);
		}

		public ActionOutcome(int coalMod, int foodMod, int healthMod, int hopeMod) {
			statsModification = new StatsModification(coalMod, foodMod, healthMod, hopeMod);
		}

		public ActionOutcome(int coalMod, int foodMod, int healthMod, int hopeMod, IFollowup followup) {
			statsModification = new StatsModification(coalMod, foodMod, healthMod, hopeMod);
			this.followup = followup;
		}

		public ActionOutcome(StatsModification statsModification, IFollowup followup) {
			this.statsModification = statsModification;
			this.followup = followup;
		}

		public void Perform(Game controller) {
			statsModification.Perform();
			if (followup != null) {
				controller.AddFollowupCard(followup);
			}
			controller.CardActionPerformed();
		}

		public StatsModification GetStatsModification()
        {
			return statsModification;
        }
	}

}
