using UnityEngine;

public class CardBehaviour : MonoBehaviour {

	public float SwipeThreshold = 1.0f;

	public CardModel Card { private get; set; }
	
	private Vector3 snapPosition;
	private Vector3 dragStartPosition;
	private Vector3 dragStartPointerPosition;
	
	private void Start() {
		snapPosition = transform.position;
	}
	
	private void OnMouseDown() {
		dragStartPosition = transform.position;
		dragStartPointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	private void OnMouseDrag() {
		Vector3 positionDelta = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragStartPointerPosition;
		positionDelta.z = dragStartPosition.z;
		transform.position = dragStartPosition + positionDelta;
	}
	
	private void OnMouseUp() {
		if (transform.position.x < snapPosition.x - SwipeThreshold) {
			Card.PerformLeftDecision();
			Destroy(gameObject);
		}
		else if (transform.position.x > snapPosition.x + SwipeThreshold) {
			Card.PerformRightDecision();
			Destroy(gameObject);
		}
		else {
			transform.position = snapPosition;
		}
	}
	
}
