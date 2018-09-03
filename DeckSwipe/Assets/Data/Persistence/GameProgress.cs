using System;
using System.Collections.Generic;

namespace Persistence {
	
	[Serializable]
	public class GameProgress {
		
		public float daysPassed;
		public float longestRunDays;
		public CardProgress[] cardProgress;
		public SpecialCardProgress[] specialCardProgress;
		
		public GameProgress(CardStorage cardStorage) {
			daysPassed = 0.0f;
			longestRunDays = 0.0f;
			
			List<CardProgress> dummyProgress = new List<CardProgress>();
			foreach (KeyValuePair<int, CardModel> entry in cardStorage.Cards) {
				CardProgress progress = new CardProgress(entry.Key,
						CardStatus.None); 
				dummyProgress.Add(progress);
				entry.Value.Progress = progress;
			}
			cardProgress = dummyProgress.ToArray();
			
			List<SpecialCardProgress> dummySpecialProgress = new List<SpecialCardProgress>();
			foreach (KeyValuePair<string, CardModel> entry in cardStorage.SpecialCards) {
				SpecialCardProgress progress = new SpecialCardProgress(entry.Key,
						CardStatus.None); 
				dummySpecialProgress.Add(progress);
				entry.Value.Progress = progress;
			}
			specialCardProgress = dummySpecialProgress.ToArray();
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
		}
		
	}
	
}
