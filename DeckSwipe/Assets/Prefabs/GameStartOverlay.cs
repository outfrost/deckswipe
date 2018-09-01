using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStartOverlay : MonoBehaviour {
	
	private enum OverlayState {
		
		Hidden,
		FadingHiddenToBlack,
		Black,
		FadingBlackToVisible,
		Visible,
		FadingVisibleToHidden
		
	}
	
	private const float fadeDuration = 0.5f;

	public static Callback FadeOutCallback { private get; set; }

	private static readonly List<GameStartOverlay> controlListeners = new List<GameStartOverlay>();
	
	public float OverlayTimeout = 2.0f;
	public Image BackgroundImage;
	public Image BlackSlate;
	
	private float fadeStartTime;
	private OverlayState overlayState = OverlayState.Hidden;
	
	private void Awake() {
		if (!Util.IsPrefab(gameObject)) {
			controlListeners.Add(this);
		}
	}
	
	private void Start() {
		SetImageVisible(BackgroundImage, false);
		SetImageVisible(BlackSlate, true);
		overlayState = OverlayState.Black;
	}
	
	private void Update() {
		float fadeProgress;
		switch (overlayState) {
			case OverlayState.FadingHiddenToBlack:
				fadeProgress = (Time.time - fadeStartTime) / fadeDuration;
				if (fadeProgress > 1.0f) {
					SetImageAlpha(BlackSlate, 1.0f);
					overlayState = OverlayState.Black;
					FadeToVisible();
				}
				else {
					SetImageAlpha(BlackSlate, Mathf.Clamp01(fadeProgress));
				}
				break;
			case OverlayState.FadingBlackToVisible:
				fadeProgress = (Time.time - fadeStartTime) / fadeDuration;
				if (fadeProgress > 1.0f) {
					SetImageVisible(BlackSlate, false);
					overlayState = OverlayState.Visible;
					DelayForSeconds(FadeOut, OverlayTimeout);
				}
				else {
					SetImageAlpha(BlackSlate, Mathf.Clamp01(1.0f - fadeProgress));
				}
				break;
			case OverlayState.FadingVisibleToHidden:
				fadeProgress = (Time.time - fadeStartTime) / fadeDuration;
				if (fadeProgress > 1.0f) {
					SetImageVisible(BackgroundImage, false);
					overlayState = OverlayState.Hidden;
				}
				else {
					SetImageAlpha(BackgroundImage, Mathf.Clamp01(1.0f - fadeProgress));
				}
				break;
		}
	}
	
	public static void StartSequence(bool fadeToBlack = true) {
		for (int i = 0; i < controlListeners.Count; i++) {
			if (controlListeners[i] == null) {
				controlListeners.RemoveAt(i);
			}
			else {
				controlListeners[i].FadeIn(fadeToBlack);
			}
		}
	}
	
	private static void SetImageAlpha(Image image, float alpha) {
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}
	
	private static void SetImageVisible(Image image, bool visible) {
		image.enabled = visible;
		SetImageAlpha(image, visible ? 1.0f : 0.0f);
	}
	
	private void FadeIn(bool fadeToBlack = true) {
		if (fadeToBlack && overlayState != OverlayState.Black) {
            FadeToBlack();
        }
		else if (!fadeToBlack && overlayState == OverlayState.Black) {
			FadeToVisible();
		}
		else if (!fadeToBlack && overlayState != OverlayState.Black) {
            SetImageVisible(BackgroundImage, true);
            SetImageVisible(BlackSlate, false);
            overlayState = OverlayState.Visible;
            DelayForSeconds(FadeOut, OverlayTimeout);
        }
	}
	
	private void FadeToBlack() {
		fadeStartTime = Time.time;
		BlackSlate.enabled = true;
		overlayState = OverlayState.FadingHiddenToBlack;
	}
	
	private void FadeToVisible() {
		fadeStartTime = Time.time;
		SetImageVisible(BackgroundImage, true);
		overlayState = OverlayState.FadingBlackToVisible;
	}
	
	private void FadeOut() {
		fadeStartTime = Time.time;
		overlayState = OverlayState.FadingVisibleToHidden;
		FadeOutCallback?.Invoke();
	}
	
	private IEnumerator DelayCoroutine(Callback callback, float seconds) {
		yield return new WaitForSeconds(seconds);
		callback();
	}
	
	private void DelayForSeconds(Callback callback, float seconds) {
		StartCoroutine(DelayCoroutine(callback, seconds));
	}
	
}
