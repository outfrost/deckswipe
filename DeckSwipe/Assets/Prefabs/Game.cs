using UnityEngine;

public class Game : MonoBehaviour {
	
	public InputDispatcher InputDispatcher;
	public CardBehaviour CardPrefab;
	public Vector3 SpawnPosition;

	public Sprite TempCardSprite;

	private CardStorage cardStorage;
	
	private void Awake() {
		// Listen for Escape key ('Back' on Android) to quit the game
		InputDispatcher.AddKeyDownHandler(KeyCode.Escape, keyCode => Application.Quit());
		cardStorage = new CardStorage(TempCardSprite);
	}
	
	private void Start() {
		DrawNextCard();
	}
	
	public void DrawNextCard() {
		if (Stats.Heat == 0) {
			SpawnCard(new CardModel("The city runs out of heat and freezes over.", "", "", "",
					null,
					new GameOverCardOutcome(),
					new GameOverCardOutcome()));
		}
		else if (Stats.Food == 0) {
			SpawnCard(new CardModel("Hunger consumes the city, as food reserves deplete.", "", "", "",
					null,
					new GameOverCardOutcome(),
					new GameOverCardOutcome()));
		}
		else if (Stats.Hope == 0) {
			SpawnCard(new CardModel("All hope among the people is lost.", "", "", "",
					null,
					new GameOverCardOutcome(),
					new GameOverCardOutcome()));
		}
		else if (Stats.Materials == 0) {
			SpawnCard(new CardModel("The city runs out of materials to sustain itself.", "", "", "",
					null,
					new GameOverCardOutcome(),
					new GameOverCardOutcome()));
		}
		else {
			SpawnCard(cardStorage.Random());
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
