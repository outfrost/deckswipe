using System;
﻿using System.Collections.Generic;
using UnityEngine;

namespace DeckSwipe.CardModel.Import {

	[Serializable]
	public class ProtoCard {

		public int id;
		public int characterId;
		public string cardText;
		public ProtoAction leftAction;
		public ProtoAction rightAction;
		public List<ProtoCardPrerequisite> cardPrerequisites;
		public List<ProtoSpecialCardPrerequisite> specialCardPrerequisites;

		public ProtoCard() {}

		public ProtoCard(
				int id,
				int characterId,
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

		public ProtoCard(
				int id,
				int characterId,
				string cardText) {
			this.id = id;
			this.characterId = characterId;
			this.cardText = cardText;
			leftAction = new ProtoAction();
			rightAction = new ProtoAction();
			cardPrerequisites = new List<ProtoCardPrerequisite>();
			specialCardPrerequisites = new List<ProtoSpecialCardPrerequisite>();
		}

	}

}
