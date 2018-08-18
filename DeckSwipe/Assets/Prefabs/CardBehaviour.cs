using System;
using UnityEngine;

public class CardBehaviour : MonoBehaviour {
	
	private enum AnimationState {

		Idle,
		Converging,
		FlyingAway

	}
	
	private const float animationDuration = 1.0f;
	
	public float SwipeThreshold = 1.0f;
	
	public CardModel Card { private get; set; }
	
	private Vector3 snapPosition;
	private Vector3 dragStartPosition;
	private Vector3 dragStartPointerPosition;
	private Vector3 lastDragPosition;
	private float dragStopTime;
	private AnimationState animationState;
	
	private void Start() {
		snapPosition = transform.position;
	}
	
	private void Update() {
		if (animationState != AnimationState.Idle) { // Animate card snap
			float animationProgress = (Time.time - dragStopTime) / animationDuration;
			float scaledProgress = 0.0f;
			if (animationState == AnimationState.Converging) {
				scaledProgress = 0.15f * Mathf.Pow(animationProgress, 3.0f)
				                 - 1.5f * Mathf.Pow(animationProgress, 2.0f)
				                 + 2.38f * animationProgress;
			}
			if (scaledProgress > 1.0f || animationProgress > 1.0f) {
				animationState = AnimationState.Idle;
				transform.position = snapPosition;
			}
			else {
				Vector3 totalDisplacement = snapPosition - lastDragPosition;
				transform.position = lastDragPosition + totalDisplacement * scaledProgress;
			}
		}
	}
	
	private void OnMouseDown() {
		animationState = AnimationState.Idle;
		dragStartPosition = transform.position;
		dragStartPointerPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
	}
	
	private void OnMouseDrag() {
		Vector3 displacement = Camera.main.ScreenToWorldPoint(Input.mousePosition) - dragStartPointerPosition;
		displacement.z = dragStartPosition.z;
		transform.position = dragStartPosition + displacement;
	}
	
	private void OnMouseUp() {
		lastDragPosition = transform.position;
		dragStopTime = Time.time;
		if (transform.position.x < snapPosition.x - SwipeThreshold) {
			Card.PerformLeftDecision();
			//Destroy(gameObject);
			animationState = AnimationState.Converging;
		}
		else if (transform.position.x > snapPosition.x + SwipeThreshold) {
			Card.PerformRightDecision();
			//Destroy(gameObject);
			animationState = AnimationState.Converging;
		}
		else {
			animationState = AnimationState.Converging;
		}
	}
	
}
