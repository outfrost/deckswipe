using System.Collections.Generic;
using DeckSwipe.CardModel.Prerequisite;
using DeckSwipe.Gamestate;
using UnityEngine;

namespace DeckSwipe.CardModel {
	
	public class Card {
		
		public readonly string cardText;
		public readonly string leftSwipeText;
		public readonly string rightSwipeText;
		public Character character;
		public ICardProgress progress;
		
		private readonly List<ICardPrerequisite> prerequisites;
		private readonly ActionOutcome leftSwipeOutcome;
		private readonly ActionOutcome rightSwipeOutcome;
		
		private Dictionary<Card, ICardPrerequisite> unsatisfiedPrerequisites;
		private List<Card> dependentCards = new List<Card>();
		
		public string CharacterName {
			get { return character != null ? character.name : ""; }
		}
		
		public Sprite CardSprite {
			get { return character?.sprite; }
		}
		
		public Card(
				string cardText,
				string leftSwipeText,
				string rightSwipeText,
				Character character,
				ActionOutcome leftOutcome,
				ActionOutcome rightOutcome,
				List<ICardPrerequisite> prerequisites) {
			this.cardText = cardText;
			this.leftSwipeText = leftSwipeText;
			this.rightSwipeText = rightSwipeText;
			this.character = character;
			leftSwipeOutcome = leftOutcome;
			rightSwipeOutcome = rightOutcome;
			this.prerequisites = prerequisites;
		}
		
		public void CardShown(Game controller) {
			progress.Status |= CardStatus.CardShown;
			foreach (Card card in dependentCards) {
				card.CheckPrerequisite(this, controller.CardStorage);
			}
		}
		
		public void PerformLeftDecision(Game controller) {
			progress.Status |= CardStatus.LeftActionTaken;
			foreach (Card card in dependentCards) {
				card.CheckPrerequisite(this, controller.CardStorage);
			}
			leftSwipeOutcome.Perform(controller);
		}
		
		public void PerformRightDecision(Game controller) {
			progress.Status |= CardStatus.RightActionTaken;
			foreach (Card card in dependentCards) {
				card.CheckPrerequisite(this, controller.CardStorage);
			}
			rightSwipeOutcome.Perform(controller);
		}
		
		public void ResolvePrerequisites(CardStorage cardStorage) {
			unsatisfiedPrerequisites = new Dictionary<Card, ICardPrerequisite>();
			foreach (ICardPrerequisite prerequisite in prerequisites) {
				Card card = prerequisite.GetCard(cardStorage);
				if (card != null
						&& (card.progress.Status & prerequisite.Status) != prerequisite.Status
						&& !unsatisfiedPrerequisites.ContainsKey(card)) {
					unsatisfiedPrerequisites.Add(card, prerequisite);
					card.AddDependentCard(this);
				}
			}
		}
		
		public bool PrerequisitesSatisfied() {
			return unsatisfiedPrerequisites.Count == 0;
		}
		
		private void CheckPrerequisite(Card dependency, CardStorage cardStorage) {
			if (PrerequisitesSatisfied()
					|| !unsatisfiedPrerequisites.ContainsKey(dependency)) {
				dependency.RemoveDependentCard(this);
				return;
			}
			
			ICardPrerequisite prerequisite = unsatisfiedPrerequisites[dependency];
			if ((dependency.progress.Status & prerequisite?.Status) == prerequisite?.Status) {
				unsatisfiedPrerequisites.Remove(dependency);
				dependency.RemoveDependentCard(this);
			}
			
			if (PrerequisitesSatisfied()) {
				// Duplicate-proof because we've verified that this card's
				// prerequisites were not satisfied before
				cardStorage.AddDrawableCard(this);
			}
		}
		
		private void AddDependentCard(Card card) {
			dependentCards.Add(card);
		}
		
		private void RemoveDependentCard(Card card) {
			dependentCards.Remove(card);
		}
		
	}
	
}
