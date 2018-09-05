using System;
using System.Collections.Generic;

namespace Persistence {
	
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
				CardModel card = cardStorage.ForId(entry.id); 
				if (card != null) {
					card.Progress = entry;
				}
			}
			foreach (SpecialCardProgress entry in specialCardProgress) {
				CardModel specialCard = cardStorage.SpecialCard(entry.id);
				if (specialCard != null) {
					specialCard.Progress = entry;
				}
			}
			
			// Fill in the missing card progress entries
			foreach (KeyValuePair<int, CardModel> entry in cardStorage.Cards) {
				if (entry.Value.Progress == null) {
					CardProgress progress = new CardProgress(
							entry.Key, CardStatus.None);
					cardProgress.Add(progress);
					entry.Value.Progress = progress;
				}
			}
			foreach (KeyValuePair<string, CardModel> entry in cardStorage.SpecialCards) {
				if (entry.Value.Progress == null) {
					SpecialCardProgress progress = new SpecialCardProgress(
							entry.Key, CardStatus.None);
					specialCardProgress.Add(progress);
					entry.Value.Progress = progress;
				}
			}
		}
		
	}
	
}
