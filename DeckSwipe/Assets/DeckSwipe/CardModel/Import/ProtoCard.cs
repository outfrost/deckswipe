using System;
﻿using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoCard {

		public int id;
		public string characterId;
		public string cardText;
		public ProtoAction leftAction;
		public ProtoAction rightAction;
		public List<ProtoCardPrerequisite> cardPrerequisites;
		public List<ProtoSpecialCardPrerequisite> specialCardPrerequisites;

		public ProtoCard(
				int id,
				string characterId,
				string cardText,
				ProtoAction leftAction,
				ProtoAction rightAction,
				List<ProtoCardPrerequisite> cardPrerequisites,
				List<ProtoSpecialCardPrerequisite> specialCardPrerequisites) {
			this.id = id;
			this.characterId = characterId;
			this.cardText = cardText;
			this.leftAction = leftAction;
			this.rightAction = rightAction;
			this.cardPrerequisites = cardPrerequisites;
			this.specialCardPrerequisites = specialCardPrerequisites;
		}

	}

}
