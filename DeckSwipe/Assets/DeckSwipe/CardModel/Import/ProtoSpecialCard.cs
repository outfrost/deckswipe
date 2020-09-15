using System;
﻿using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoSpecialCard {

		public string id;
		public int characterId;
		public string cardText;
		public ProtoSpecialAction leftAction;
		public ProtoSpecialAction rightAction;

		public ProtoSpecialCard() {}

		public ProtoSpecialCard(
				string id,
				int characterId,
				string cardText,
				ProtoSpecialAction leftAction,
				ProtoSpecialAction rightAction) {
			this.id = id;
			this.characterId = characterId;
			this.cardText = cardText;
			this.leftAction = leftAction;
			this.rightAction = rightAction;
		}

		public ProtoSpecialCard(
				string id,
				int characterId,
				string cardText) {
			this.id = id;
			this.characterId = characterId;
			this.cardText = cardText;
			leftAction = new ProtoSpecialAction();
			rightAction = new ProtoSpecialAction();
		}

	}

}
