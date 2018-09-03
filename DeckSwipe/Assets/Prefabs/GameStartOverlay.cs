using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	public TextMeshProUGUI CurrentTimeText;
	
	private float fadeStartTime;
	private OverlayState overlayState = OverlayState.Hidden;
	
	private void Awake() {
		if (!Util.IsPrefab(gameObject)) {
			controlListeners.Add(this);
		}
	}
	
	private void Start() {
		SetGraphicVisible(BackgroundImage, false);
		SetGraphicVisible(CurrentTimeText, false);
		SetGraphicVisible(BlackSlate, true);
		overlayState = OverlayState.Black;
	}
	
	private void Update() {
		float fadeProgress;
		switch (overlayState) {
			case OverlayState.FadingHiddenToBlack:
				fadeProgress = (Time.time - fadeStartTime) / fadeDuration;
				if (fadeProgress > 1.0f) {
					SetColorAlpha(BlackSlate, 1.0f);
					overlayState = OverlayState.Black;
					FadeToVisible();
				}
				else {
					SetColorAlpha(BlackSlate, Mathf.Clamp01(fadeProgress));
				}
				break;
			case OverlayState.FadingBlackToVisible:
				fadeProgress = (Time.time - fadeStartTime) / fadeDuration;
				if (fadeProgress > 1.0f) {
					SetGraphicVisible(BlackSlate, false);
					overlayState = OverlayState.Visible;
					DelayForSeconds(FadeOut, OverlayTimeout);
				}
				else {
					SetColorAlpha(BlackSlate, Mathf.Clamp01(1.0f - fadeProgress));
				}
				break;
			case OverlayState.FadingVisibleToHidden:
				fadeProgress = (Time.time - fadeStartTime) / fadeDuration;
				if (fadeProgress > 1.0f) {
					SetGraphicVisible(BackgroundImage, false);
					SetGraphicVisible(CurrentTimeText, false);
					overlayState = OverlayState.Hidden;
				}
				else {
					SetColorAlpha(BackgroundImage, Mathf.Clamp01(1.0f - fadeProgress));
					SetColorAlpha(CurrentTimeText, Mathf.Clamp01(1.0f - fadeProgress));
				}
				break;
		}
	}
	
	public static void StartSequence(float daysPassed, bool fadeToBlack = true) {
		for (int i = 0; i < controlListeners.Count; i++) {
			if (controlListeners[i] == null) {
				controlListeners.RemoveAt(i);
			}
			else {
				controlListeners[i].SetCurrentTimeText(daysPassed);
				controlListeners[i].FadeIn(fadeToBlack);
			}
		}
	}
	
	private static void SetColorAlpha(Graphic image, float alpha) {
		Color color = image.color;
		color.a = alpha;
		image.color = color;
	}
	
	private static void SetGraphicVisible(Graphic image, bool visible) {
		image.enabled = visible;
		SetColorAlpha(image, visible ? 1.0f : 0.0f);
	}
	
	private void FadeIn(bool fadeToBlack = true) {
		if (fadeToBlack && overlayState != OverlayState.Black) {
            FadeToBlack();
        }
		else if (!fadeToBlack && overlayState == OverlayState.Black) {
			FadeToVisible();
		}
		else if (!fadeToBlack && overlayState != OverlayState.Black) {
            SetGraphicVisible(BackgroundImage, true);
			SetGraphicVisible(CurrentTimeText, true);
            SetGraphicVisible(BlackSlate, false);
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
		SetGraphicVisible(BackgroundImage, true);
		SetGraphicVisible(CurrentTimeText, true);
		overlayState = OverlayState.FadingBlackToVisible;
	}
	
	private void FadeOut() {
		fadeStartTime = Time.time;
		overlayState = OverlayState.FadingVisibleToHidden;
		FadeOutCallback?.Invoke();
	}
	
	private void SetCurrentTimeText(float daysPassed) {
		CurrentTimeText.text = ApproximateDate(daysPassed);
	}
	
	private string ApproximateDate(float daysPassed) {
		int year = 1887 + (int)(daysPassed / 365.25f);
		int month = (int)((daysPassed % 365.25f) / 30.4375f);
		return MonthName(month) + " " + year;
	}
	
	private string MonthName(int monthIndex) {
		switch (monthIndex) {
			case 0:
				return "January";
			case 1:
				return "February";
			case 2:
				return "March";
			case 3:
				return "April";
			case 4:
				return "May";
			case 5:
				return "June";
			case 6:
				return "July";
			case 7:
				return "August";
			case 8:
				return "September";
			case 9:
				return "October";
			case 10:
				return "November";
			case 11:
				return "December";
			default:
				return "";
		}
	}
	
	private IEnumerator DelayCoroutine(Callback callback, float seconds) {
		yield return new WaitForSeconds(seconds);
		callback();
	}
	
	private void DelayForSeconds(Callback callback, float seconds) {
		StartCoroutine(DelayCoroutine(callback, seconds));
	}
	
}
