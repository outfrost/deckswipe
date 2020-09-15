using System;
using System.Collections.Generic;
using DeckSwipe.CardModel.DrawQueue;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoAction {

		public string text;
		public StatsModification statsModification;
		public List<Followup> followup;
		public List<SpecialFollowup> specialFollowup;

		public ProtoAction() {}

		public ProtoAction(
				string text,
				StatsModification statsModification,
				List<Followup> followup,
				List<SpecialFollowup> specialFollowup) {
			this.text = text;
			this.statsModification = statsModification;
			this.followup = followup;
			this.specialFollowup = specialFollowup;
		}

	}

}
