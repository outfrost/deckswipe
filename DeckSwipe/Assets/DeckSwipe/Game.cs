using DeckSwipe.CardModel;
using DeckSwipe.CardModel.DrawQueue;
using DeckSwipe.Gamestate;
using DeckSwipe.Gamestate.Persistence;
using DeckSwipe.World;
using Outfrost;
using UnityEngine;

namespace DeckSwipe {
	
	public class Game : MonoBehaviour {
		
		private const int _saveInterval = 8;
		
		public InputDispatcher inputDispatcher;
		public CardBehaviour cardPrefab;
		public Vector3 spawnPosition;
		public Sprite defaultCharacterSprite;
		
		private CardStorage cardStorage;
		private ProgressStorage progressStorage;
		private float daysPassedPreviously;
		private float daysLastRun;
		private int saveIntervalCounter;
		private CardDrawQueue cardDrawQueue = new CardDrawQueue();
		
		private void Awake() {
			// Listen for Escape key ('Back' on Android) to quit the game
			inputDispatcher.AddKeyDownHandler(KeyCode.Escape,
					keyCode => Application.Quit());
			
			cardStorage = new CardStorage(defaultCharacterSprite);
			progressStorage = new ProgressStorage(cardStorage);
			
			GameStartOverlay.FadeOutCallback = StartGameplayLoop;
		}
		
		private void Start() {
			CallbackWhenDoneLoading(StartGame);
		}
		
		private void StartGame() {
			daysPassedPreviously = progressStorage.Progress.daysPassed;
			GameStartOverlay.StartSequence(progressStorage.Progress.daysPassed, daysLastRun);
		}
		
		public void RestartGame() {
			progressStorage.Save();
			daysLastRun = progressStorage.Progress.daysPassed - daysPassedPreviously;
			cardDrawQueue.Clear();
			StartGame();
		}
		
		private void StartGameplayLoop() {
			Stats.ResetStats();
			ProgressDisplay.SetDaysSurvived(0);
			DrawNextCard();
		}
		
		public void DrawNextCard() {
			if (Stats.Coal == 0) {
				SpawnCard(cardStorage.SpecialCard("gameover_coal"));
			}
			else if (Stats.Food == 0) {
				SpawnCard(cardStorage.SpecialCard("gameover_food"));
			}
			else if (Stats.Health == 0) {
				SpawnCard(cardStorage.SpecialCard("gameover_health"));
			}
			else if (Stats.Hope == 0) {
				SpawnCard(cardStorage.SpecialCard("gameover_hope"));
			}
			else {
				IFollowup followup = cardDrawQueue.Next();
				Card card = followup?.Fetch(cardStorage);
				if (card != null) {
					SpawnCard(card);
				}
				else {
					bool prerequisitesSatisfied = false;
					while (!prerequisitesSatisfied) {
						card = cardStorage.Random();
						prerequisitesSatisfied = card.CheckPrerequisites(cardStorage);
					}
					SpawnCard(card);
				}
			}
			saveIntervalCounter = (saveIntervalCounter - 1) % _saveInterval;
			if (saveIntervalCounter == 0) {
				progressStorage.Save();
			}
		}
		
		public void CardActionPerformed() {
			progressStorage.Progress.AddDays(Random.Range(0.5f, 1.5f),
					daysPassedPreviously);
			ProgressDisplay.SetDaysSurvived(
					(int)(progressStorage.Progress.daysPassed - daysPassedPreviously));
			DrawNextCard();
		}
		
		public void AddFollowupCard(IFollowup followup) {
			cardDrawQueue.Insert(followup);
		}
		
		private async void CallbackWhenDoneLoading(Callback callback) {
			await progressStorage.ProgressStorageInit;
			callback();
		}
		
		private void SpawnCard(Card card) {
			CardBehaviour cardInstance = Instantiate(cardPrefab, spawnPosition,
					Quaternion.Euler(0.0f, -180.0f, 0.0f));
			cardInstance.Card = card;
			cardInstance.snapPosition.y = spawnPosition.y;
			cardInstance.Controller = this;
		}
		
	}
	
}
