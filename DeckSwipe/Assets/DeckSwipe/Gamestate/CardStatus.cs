using System;

namespace DeckSwipe.Gamestate {

	[Serializable]
	[Flags]
	public enum CardStatus {

		None = 0,
		CardShown = 1 << 0,
		RightActionTaken = 1 << 1,
		LeftActionTaken = 1 << 2,

	}

}
