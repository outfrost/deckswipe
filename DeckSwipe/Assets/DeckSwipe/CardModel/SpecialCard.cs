using System.Collections.Generic;
using DeckSwipe.CardModel.Prerequisite;
using DeckSwipe.Gamestate;
using UnityEngine;

namespace DeckSwipe.CardModel {

	public class SpecialCard : ICard {

		public string CardText { get; }
		public string LeftSwipeText { get; }
		public string RightSwipeText { get; }

		public string CharacterName {
			get { return character != null ? character.name : ""; }
		}

		public Sprite CardSprite {
			get { return character?.sprite; }
		}

		public ICardProgress Progress {
			get { return progress; }
		}

		public Character character;
		public SpecialCardProgress progress;

		private readonly IActionOutcome leftSwipeOutcome;
		private readonly IActionOutcome rightSwipeOutcome;

		private List<Card> dependentCards = new List<Card>();

		public SpecialCard(
				string cardText,
				string leftSwipeText,
				string rightSwipeText,
				Character character,
				IActionOutcome leftOutcome,
				IActionOutcome rightOutcome) {
			this.CardText = cardText;
			this.LeftSwipeText = leftSwipeText;
			this.RightSwipeText = rightSwipeText;
			this.character = character;
			leftSwipeOutcome = leftOutcome;
			rightSwipeOutcome = rightOutcome;
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

		public void AddDependentCard(Card card) {
			dependentCards.Add(card);
		}

		public void RemoveDependentCard(Card card) {
			dependentCards.Remove(card);
		}

	}

}
