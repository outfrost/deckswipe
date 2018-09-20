using DeckSwipe.CardModel.DrawQueue;

namespace DeckSwipe.CardModel {

	public class CardActionOutcome {
	
		private readonly StatsModification statsModification;
		private readonly IFollowupCard followupCard;
	
		public CardActionOutcome() {
			statsModification = new StatsModification(0, 0, 0, 0);
		}
	
		public CardActionOutcome(int coalMod, int foodMod, int healthMod, int hopeMod) {
			statsModification = new StatsModification(coalMod, foodMod, healthMod, hopeMod);
		}
	
		public CardActionOutcome(int coalMod, int foodMod, int healthMod, int hopeMod, IFollowupCard followupCard) {
			statsModification = new StatsModification(coalMod, foodMod, healthMod, hopeMod);
			this.followupCard = followupCard;
		}
	
		public virtual void Perform(Game controller) {
			statsModification.Perform();
			if (followupCard != null) {
				controller.AddFollowupCard(followupCard);
			}
			controller.CardActionPerformed();
		}
	
	}

}
