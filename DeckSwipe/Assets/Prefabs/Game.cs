using UnityEngine;

public class Game : MonoBehaviour {
	
	public CardBehaviour CardPrefab;
	public Vector3 SpawnPosition;
	
	private void Start() {
		DrawNextCard();
	}
	
	public void DrawNextCard() {
		if (Stats.Heat == 0) {
			SpawnCard(new CardModel(
					new GameOverCardOutcome(),
					new GameOverCardOutcome()));
		}
		else if (Stats.Food == 0) {
			SpawnCard(new CardModel(
				new GameOverCardOutcome(),
				new GameOverCardOutcome()));
		}
		else if (Stats.Hope == 0) {
			SpawnCard(new CardModel(
				new GameOverCardOutcome(),
				new GameOverCardOutcome()));
		}
		else if (Stats.Materials == 0) {
			SpawnCard(new CardModel(
				new GameOverCardOutcome(),
				new GameOverCardOutcome()));
		}
		else {
			SpawnCard(new CardModel(
				new CardActionOutcome(-4, 4, -2, 2),
				new CardActionOutcome(2, 0, 4, -2)));
		}
	}

	public void RestartGame() {
		Stats.ResetStats();
		DrawNextCard();
	}

	private void SpawnCard(CardModel card) {
		CardBehaviour cardInstance = Instantiate(CardPrefab, SpawnPosition, Quaternion.Euler(0.0f, -180.0f, 0.0f));
		cardInstance.Card = card;
		cardInstance.SnapPosition.y = SpawnPosition.y;
		cardInstance.Controller = this;
	}
	
}
