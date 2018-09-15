using System.Collections.Generic;
using Persistence;
using UnityEngine;

public class CardModel {
	
	public readonly List<ICardPrerequisite> prerequisites;
	public readonly string cardText;
	public readonly string leftSwipeText;
	public readonly string rightSwipeText;
	public CharacterModel character;
	public ICardProgress progress;
	
	private readonly CardActionOutcome leftSwipeOutcome;
	private readonly CardActionOutcome rightSwipeOutcome;
	
	public string CharacterName {
		get { return character != null ? character.name : ""; }
	}
	
	public Sprite CardSprite {
		get { return character?.sprite; }
	}
	
	public CardModel(
			string cardText,
			string leftSwipeText,
			string rightSwipeText,
			CharacterModel character,
			CardActionOutcome leftOutcome,
			CardActionOutcome rightOutcome,
			List<ICardPrerequisite> prerequisites) {
		this.cardText = cardText;
		this.leftSwipeText = leftSwipeText;
		this.rightSwipeText = rightSwipeText;
		this.character = character;
		leftSwipeOutcome = leftOutcome;
		rightSwipeOutcome = rightOutcome;
		this.prerequisites = prerequisites;
	}
	
	public void CardShown() {
		progress.Status |= CardStatus.CardShown;
	}
	
	public void PerformLeftDecision(Game controller) {
		leftSwipeOutcome.Perform(controller);
		progress.Status |= CardStatus.LeftActionTaken;
	}
	
	public void PerformRightDecision(Game controller) {
		rightSwipeOutcome.Perform(controller);
		progress.Status |= CardStatus.RightActionTaken;
	}
	
	public bool CheckPrerequisites(CardStorage cardStorage) {
		foreach (ICardPrerequisite prerequisite in prerequisites) {
			if (!prerequisite.IsSatisfied(cardStorage)) {
				return false;
			}
		}
		return true;
	}
	
}
