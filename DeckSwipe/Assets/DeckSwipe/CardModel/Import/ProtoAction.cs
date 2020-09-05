using System;
using DeckSwipe.CardModel.DrawQueue;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoAction {

		public string text;
		public StatsModification statsModification;
		public Followup followup;
		public SpecialFollowup specialFollowup;

		public ProtoAction(
				string text,
				StatsModification statsModification,
				Followup followup,
				SpecialFollowup specialFollowup) {
			this.text = text;
			this.statsModification = statsModification;
			this.followup = followup;
			this.specialFollowup = specialFollowup;
		}

	}

}
