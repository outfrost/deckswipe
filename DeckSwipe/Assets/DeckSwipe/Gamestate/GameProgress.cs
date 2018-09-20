using System;
using System.Collections.Generic;

namespace DeckSwipe.Gamestate {
	
	[Serializable]
	public class GameProgress {
		
		public float daysPassed;
		public float longestRunDays;
		public List<CardProgress> cardProgress;
		public List<SpecialCardProgress> specialCardProgress;
		
		public void AddDays(float days, float daysPassedPreviously) {
			daysPassed += days;
			float daysPassedThisRun = daysPassed - daysPassedPreviously;
			if (daysPassedThisRun > longestRunDays) {
				longestRunDays = daysPassedThisRun;
			}
		}
		
		public void AttachReferences(CardStorage cardStorage) {
			foreach (CardProgress entry in cardProgress) {
				CardModel.CardModel card = cardStorage.ForId(entry.id); 
				if (card != null) {
					card.progress = entry;
				}
			}
			foreach (SpecialCardProgress entry in specialCardProgress) {
				CardModel.CardModel specialCard = cardStorage.SpecialCard(entry.id);
				if (specialCard != null) {
					specialCard.progress = entry;
				}
			}
			
			// Fill in the missing card progress entries
			foreach (KeyValuePair<int, CardModel.CardModel> entry in cardStorage.Cards) {
				if (entry.Value.progress == null) {
					CardProgress progress = new CardProgress(
							entry.Key, CardStatus.None);
					cardProgress.Add(progress);
					entry.Value.progress = progress;
				}
			}
			foreach (KeyValuePair<string, CardModel.CardModel> entry in cardStorage.SpecialCards) {
				if (entry.Value.progress == null) {
					SpecialCardProgress progress = new SpecialCardProgress(
							entry.Key, CardStatus.None);
					specialCardProgress.Add(progress);
					entry.Value.progress = progress;
				}
			}
		}
		
	}
	
}
