using Persistence;
using UnityEngine;

public class Game : MonoBehaviour {
	
	public InputDispatcher InputDispatcher;
	public CardBehaviour CardPrefab;
	public Vector3 SpawnPosition;
	public Sprite DefaultCharacterSprite;
	
	private CardStorage cardStorage;
	private GameProgress gameProgress;
	private float daysPassedPreviously;
	
	private void Awake() {
		// Listen for Escape key ('Back' on Android) to quit the game
		InputDispatcher.AddKeyDownHandler(KeyCode.Escape, keyCode => Application.Quit());
		cardStorage = new CardStorage(DefaultCharacterSprite);
	}
	
	private void Start() {
		cardStorage.CallbackWhenCardsAvailable(StartGame);
	}
	
	public void DrawNextCard() {
		if (Stats.Heat == 0) {
			SpawnCard(cardStorage.SpecialCard("gameover_heat"));
		}
		else if (Stats.Food == 0) {
			SpawnCard(cardStorage.SpecialCard("gameover_food"));
		}
		else if (Stats.Hope == 0) {
			SpawnCard(cardStorage.SpecialCard("gameover_hope"));
		}
		else if (Stats.Materials == 0) {
			SpawnCard(cardStorage.SpecialCard("gameover_materials"));
		}
		else {
			SpawnCard(cardStorage.Random());
		}
		gameProgress.daysPassed += Random.Range(0.5f, 1.5f);
		float daysPassedThisRun = gameProgress.daysPassed - daysPassedPreviously;
		if (daysPassedThisRun > gameProgress.longestRunDays) {
			gameProgress.longestRunDays = daysPassedThisRun;
		}
	}
	
	public void RestartGame() {
		Stats.ResetStats();
		GameStartOverlay.StartSequence(gameProgress.daysPassed);
	}
	
	private void StartGame() {
		gameProgress = new GameProgress(cardStorage);
		daysPassedPreviously = gameProgress.daysPassed;
		GameStartOverlay.FadeOutCallback = DrawNextCard;
		GameStartOverlay.StartSequence(gameProgress.daysPassed, false);
	}
	
	private void SpawnCard(CardModel card) {
		CardBehaviour cardInstance = Instantiate(CardPrefab, SpawnPosition, Quaternion.Euler(0.0f, -180.0f, 0.0f));
		cardInstance.Card = card;
		cardInstance.SnapPosition.y = SpawnPosition.y;
		cardInstance.Controller = this;
	}
	
}
