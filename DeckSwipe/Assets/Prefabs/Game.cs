using Persistence;
using UnityEngine;

public class Game : MonoBehaviour {
	
	private const int saveInterval = 8;
	
	public InputDispatcher InputDispatcher;
	public CardBehaviour CardPrefab;
	public Vector3 SpawnPosition;
	public Sprite DefaultCharacterSprite;
	
	private CardStorage cardStorage;
	private ProgressStorage progressStorage;
	private float daysPassedPreviously;
	private int saveIntervalCounter;
	
	private void Awake() {
		// Listen for Escape key ('Back' on Android) to quit the game
		InputDispatcher.AddKeyDownHandler(KeyCode.Escape,
				keyCode => Application.Quit());
		
		cardStorage = new CardStorage(DefaultCharacterSprite);
		progressStorage = new ProgressStorage(cardStorage);
		
		GameStartOverlay.FadeOutCallback = StartGameplayLoop;
	}
	
	private void Start() {
		CallbackWhenDoneLoading(StartGame);
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
		saveIntervalCounter = (saveIntervalCounter - 1) % saveInterval;
		if (saveIntervalCounter == 0) {
			progressStorage.Save();
		}
	}
	
	public void CardActionPerformed() {
		progressStorage.Progress.AddDays(Random.Range(0.5f, 1.5f),
				daysPassedPreviously);
		DrawNextCard();
	}
	
	public void RestartGame() {
		progressStorage.Save();
		StartGame();
	}
	
	private void StartGame() {
		daysPassedPreviously = progressStorage.Progress.daysPassed;
		GameStartOverlay.StartSequence(progressStorage.Progress.daysPassed);
	}
	
	private void StartGameplayLoop() {
		Stats.ResetStats();
		DrawNextCard();
	}
	
	private async void CallbackWhenDoneLoading(Callback callback) {
		await progressStorage.ProgressStorageInit;
		callback();
	}
	
	private void SpawnCard(CardModel card) {
		CardBehaviour cardInstance = Instantiate(CardPrefab, SpawnPosition,
				Quaternion.Euler(0.0f, -180.0f, 0.0f));
		cardInstance.Card = card;
		cardInstance.SnapPosition.y = SpawnPosition.y;
		cardInstance.Controller = this;
	}
	
}
