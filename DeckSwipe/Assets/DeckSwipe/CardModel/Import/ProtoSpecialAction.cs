using System;
using System.Collections.Generic;
using DeckSwipe.CardModel.DrawQueue;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoSpecialAction {

		public string text;
		public List<Followup> followup;
		public List<SpecialFollowup> specialFollowup;

		public ProtoSpecialAction() {}

		public ProtoSpecialAction(
				string text,
				List<Followup> followup,
				List<SpecialFollowup> specialFollowup) {
			this.text = text;
			this.followup = followup;
			this.specialFollowup = specialFollowup;
		}

	}

}
