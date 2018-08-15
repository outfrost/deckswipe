using UnityEngine;

public class Game : MonoBehaviour {
	
	public CardBehaviour CardPrefab;
	public Vector3 SpawnPosition;
	
	void Start() {
		CardBehaviour cardInstance = Instantiate<CardBehaviour>(CardPrefab, SpawnPosition, Quaternion.identity);
		cardInstance.Card = new CardModel(
				new CardDecisionOutcome(4, 0, -2, -3),
				new CardDecisionOutcome(-1, -1, -1, -1));

		cardInstance = Instantiate<CardBehaviour>(CardPrefab, SpawnPosition, Quaternion.identity);
		cardInstance.Card = new CardModel(
				new CardDecisionOutcome(-6, 3, 3, -2),
				new CardDecisionOutcome(0, 5, -5, 0));
		
		cardInstance = Instantiate<CardBehaviour>(CardPrefab, SpawnPosition, Quaternion.identity);
		cardInstance.Card = new CardModel(
				new CardDecisionOutcome(2, 5, -2, 0),
				new CardDecisionOutcome(0, 1, 2, 3));
		
		cardInstance = Instantiate<CardBehaviour>(CardPrefab, SpawnPosition, Quaternion.identity);
		cardInstance.Card = new CardModel(
				new CardDecisionOutcome(-4, 2, 3, 0),
				new CardDecisionOutcome(3, 4, -2, 0));

		cardInstance = Instantiate<CardBehaviour>(CardPrefab, SpawnPosition, Quaternion.identity);
		cardInstance.Card = new CardModel(
				new CardDecisionOutcome(2, 2, 2, -2),
				new CardDecisionOutcome(2, -4, -2, 4));
		
	}
	
}
