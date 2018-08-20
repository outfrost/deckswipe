using UnityEngine;

public class Game : MonoBehaviour {
	
	public CardBehaviour CardPrefab;
	public Vector3 SpawnPosition;
	
	private void Start() {
		SpawnCard();
	}
	
	public void SpawnCard() {
		CardBehaviour cardInstance = Instantiate(CardPrefab, SpawnPosition, Quaternion.Euler(0.0f, -180.0f, 0.0f));
		cardInstance.Card = new CardModel(
				new CardActionOutcome(-4, 4, -2, 2),
				new CardActionOutcome(2, 0, 4, -2));
		cardInstance.SnapPosition.y = -0.5f;
		cardInstance.Controller = this;
	}
	
}
