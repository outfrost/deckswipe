using System;
using System.Collections.Generic;
using DeckSwipe.CardModel;

namespace DeckSwipe.Gamestate {
	
	[Serializable]
	public class GameProgress {
		
		public float daysPassed;
		public float longestRunDays;
		public List<CardProgress> cardProgress = new List<CardProgress>();
		public List<SpecialCardProgress> specialCardProgress = new List<SpecialCardProgress>();
		
		public void AddDays(float days, float daysPassedPreviously) {
			daysPassed += days;
			float daysPassedThisRun = daysPassed - daysPassedPreviously;
			if (daysPassedThisRun > longestRunDays) {
				longestRunDays = daysPassedThisRun;
			}
		}
		
		public void AttachReferences(CardStorage cardStorage) {
			foreach (CardProgress entry in cardProgress) {
				Card card = cardStorage.ForId(entry.id); 
				if (card != null) {
					card.progress = entry;
				}
			}
			foreach (SpecialCardProgress entry in specialCardProgress) {
				Card specialCard = cardStorage.SpecialCard(entry.id);
				if (specialCard != null) {
					specialCard.progress = entry;
				}
			}
			
			// Fill in the missing card progress entries
			foreach (KeyValuePair<int, Card> entry in cardStorage.Cards) {
				if (entry.Value.progress == null) {
					CardProgress progress = new CardProgress(
							entry.Key, CardStatus.None);
					cardProgress.Add(progress);
					entry.Value.progress = progress;
				}
			}
			foreach (KeyValuePair<string, Card> entry in cardStorage.SpecialCards) {
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
