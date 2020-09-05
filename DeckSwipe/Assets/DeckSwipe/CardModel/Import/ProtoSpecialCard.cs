using System;
﻿using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoSpecialCard {

		public int id;
		public string characterId;
		public string cardText;
		public ProtoSpecialAction leftAction;
		public ProtoSpecialAction rightAction;

		public ProtoSpecialCard(
				int id,
				string characterId,
				string cardText,
				ProtoSpecialAction leftAction,
				ProtoSpecialAction rightAction) {
			this.id = id;
			this.characterId = characterId;
			this.cardText = cardText;
			this.leftAction = leftAction;
			this.rightAction = rightAction;
		}

	}

}
