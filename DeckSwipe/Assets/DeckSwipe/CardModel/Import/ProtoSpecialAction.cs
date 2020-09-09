using System;
using DeckSwipe.CardModel.DrawQueue;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoSpecialAction {

		public string text;
		public Followup followup;
		public SpecialFollowup specialFollowup;

		public ProtoSpecialAction() {}

		public ProtoSpecialAction(
				string text,
				Followup followup,
				SpecialFollowup specialFollowup) {
			this.text = text;
			this.followup = followup;
			this.specialFollowup = specialFollowup;
		}

	}

}
