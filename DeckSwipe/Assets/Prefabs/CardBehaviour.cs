using System;
using TMPro;
using UnityEngine;

public class CardBehaviour : MonoBehaviour {
	
	private enum AnimationState {

		Idle,
		Converging,
		FlyingAway,
		Revealing

	}
	
	private const float animationDuration = 0.4f;
	
	public float SwipeThreshold = 1.0f;
	public Vector3 SnapPosition;
	public Vector3 SnapRotationAngles;
	public TextMeshPro LeftActionText;
	public TextMeshPro RightActionText;
	public SpriteRenderer CardImageSpriteRenderer;

	public CardModel Card {
		set {
			card = value;
			LeftActionText.text = card.LeftSwipeText;
			RightActionText.text = card.RightSwipeText;
			if (card.CardImage != null) {
				CardImageSpriteRenderer.sprite = card.CardImage;
			}
		}
	}

	public Game Controller { private get; set;  }

	private CardModel card;
	private Vector3 dragStartPosition;
	private Vector3 dragStartPointerPosition;
	private Vector3 animationStartPosition;
	private Vector3 animationStartRotationAngles;
	private float animationStartTime;
	private AnimationState animationState = AnimationState.Idle;

	private void Awake() {
		Util.SetTextAlpha(LeftActionText, 0.0f);
		Util.SetTextAlpha(RightActionText, 0.0f);
	}

	private void Start() {
		// Rotate clockwise on reveal instead of anticlockwise 
		SnapRotationAngles.y += 360.0f;
		
		animationStartPosition = transform.position;
		animationStartRotationAngles = transform.eulerAngles;
		animationStartTime = Time.time;
		animationState = AnimationState.Revealing;
	}
	
	private void Update() {
		// Animate card by interpolating translation and rotation, destroy swiped cards
		if (animationState != AnimationState.Idle) {
			float animationProgress = (Time.time - animationStartTime) / animationDuration;
			float scaledProgress = ScaleProgress(animationProgress);
			if (scaledProgress > 1.0f || animationProgress > 1.0f) {
				transform.position = SnapPosition;
				transform.eulerAngles = SnapRotationAngles;
				
				if (animationState == AnimationState.Revealing) {
					CardDescriptionDisplay.SetDescription(card.CardText, card.CharacterName);
					SnapRotationAngles.y -= 360.0f;
				}
				
				if (animationState == AnimationState.FlyingAway) {
					Destroy(gameObject);
				}
				else {
					animationState = AnimationState.Idle;
				}
			}
			else {
				transform.position = Vector3.Lerp(animationStartPosition, SnapPosition, scaledProgress);
				transform.eulerAngles = Vector3.Lerp(animationStartRotationAngles, SnapRotationAngles, scaledProgress);
				
				// Hide card face elements unless it's facing the main camera
				bool isFacingCamera = Util.IsFacingCamera(gameObject);
				CardImageSpriteRenderer.enabled = isFacingCamera;
				LeftActionText.enabled = isFacingCamera;
				RightActionText.enabled = isFacingCamera;
			}
			if (animationState != AnimationState.Revealing) {
				float alphaCoord = (transform.position.x - SnapPosition.x) / (SwipeThreshold / 2);
				Util.SetTextAlpha(LeftActionText, Mathf.Clamp01(-alphaCoord));
				Util.SetTextAlpha(RightActionText, Mathf.Clamp01(alphaCoord));
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
		displacement.z = 0.0f;
		transform.position = dragStartPosition + displacement;
		
		float alphaCoord = (transform.position.x - SnapPosition.x) / (SwipeThreshold / 2);
		Util.SetTextAlpha(LeftActionText, -alphaCoord);
		Util.SetTextAlpha(RightActionText, alphaCoord);
	}
	
	private void OnMouseUp() {
		animationStartPosition = transform.position;
		animationStartRotationAngles = transform.eulerAngles;
		animationStartTime = Time.time;
		if (transform.position.x < SnapPosition.x - SwipeThreshold) {
			card.PerformLeftDecision(Controller);
			Vector3 displacement = animationStartPosition - SnapPosition;
			SnapPosition += displacement.normalized
			                * Util.OrthoCameraWorldDiagonalSize(Camera.main)
			                * 2.0f;
			SnapRotationAngles = transform.eulerAngles;
			animationState = AnimationState.FlyingAway;
			CardDescriptionDisplay.ResetDescription();
		}
		else if (transform.position.x > SnapPosition.x + SwipeThreshold) {
			card.PerformRightDecision(Controller);
			Vector3 displacement = animationStartPosition - SnapPosition;
			SnapPosition += displacement.normalized
			                * Util.OrthoCameraWorldDiagonalSize(Camera.main)
			                * 2.0f;
			SnapRotationAngles = transform.eulerAngles;
			animationState = AnimationState.FlyingAway;
			CardDescriptionDisplay.ResetDescription();
		}
		else {
			animationState = AnimationState.Converging;
		}
	}

	private float ScaleProgress(float animationProgress) {
		switch (animationState) {
			case AnimationState.Converging:
				return 0.15f * Mathf.Pow(animationProgress, 3.0f)
				       - 1.5f * Mathf.Pow(animationProgress, 2.0f)
				       + 2.38f * animationProgress;
			case AnimationState.FlyingAway:
				return 1.5f * Mathf.Pow(animationProgress, 3.0f)
				      + 0.55f * animationProgress;
			default:
				return animationProgress;
		}
	}
	
}
