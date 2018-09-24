using System.Collections;
using System.Collections.Generic;
using Outfrost;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DeckSwipe.World {
	
	public class GameStartOverlay : MonoBehaviour {
		
		private enum OverlayState {
			
			Hidden,
			FadingHiddenToBlack,
			Black,
			FadingBlackToVisible,
			Visible,
			FadingVisibleToHidden
			
		}
		
		private const float _fadeDuration = 0.5f;
		
		private static readonly List<GameStartOverlay> _controlListeners = new List<GameStartOverlay>();
		
		public static Callback FadeOutCallback { private get; set; }
		
		public float overlayTimeout = 2.0f;
		public float dayCounterRewindDuration = 1.0f;
		public Image backgroundImage;
		public Image blackSlate;
		public TextMeshProUGUI currentTimeText;
		public TextMeshProUGUI daysSurvivedLabel;
		public TextMeshProUGUI daysSurvivedText;
		
		private static float rewindStartDays;
		
		private float fadeStartTime;
		private OverlayState overlayState = OverlayState.Hidden;
		
		private float rewindStartTime;
		private bool rewindingDaysCounter;
		
		private void Awake() {
			if (!Util.IsPrefab(gameObject)) {
				_controlListeners.Add(this);
			}
		}
		
		private void Start() {
			SetOverlayVisible(false);
			SetBlackSlateVisible(true);
			overlayState = OverlayState.Black;
		}
		
		private void Update() {
			// Animate overlay by interpolating alpha values, manage fade states
			float fadeProgress;
			switch (overlayState) {
				case OverlayState.FadingHiddenToBlack:
					fadeProgress = (Time.time - fadeStartTime) / _fadeDuration;
					if (fadeProgress > 1.0f) {
						SetBlackSlateAlpha(1.0f);
						overlayState = OverlayState.Black;
						FadeToVisible();
					}
					else {
						SetBlackSlateAlpha(Mathf.Clamp01(fadeProgress));
					}
					break;
				case OverlayState.FadingBlackToVisible:
					fadeProgress = (Time.time - fadeStartTime) / _fadeDuration;
					if (fadeProgress > 1.0f) {
						SetBlackSlateVisible(false);
						overlayState = OverlayState.Visible;
						DelayForSeconds(FadeOut, overlayTimeout);
						rewindStartTime = Time.time;
						rewindingDaysCounter = true;
					}
					else {
						SetBlackSlateAlpha(Mathf.Clamp01(1.0f - fadeProgress));
					}
					break;
				case OverlayState.Visible:
					if (rewindingDaysCounter) {
						float rewindProgress = (Time.time - rewindStartTime) / dayCounterRewindDuration;
						if (rewindProgress > 1.0f) {
							rewindingDaysCounter = false;
							ProgressDisplay.SetDaysSurvived(0);
						}
						else {
							ProgressDisplay.SetDaysSurvived((int) Mathf.Lerp(rewindStartDays, 0.0f, rewindProgress));
						}
					}
					break;
				case OverlayState.FadingVisibleToHidden:
					fadeProgress = (Time.time - fadeStartTime) / _fadeDuration;
					if (fadeProgress > 1.0f) {
						SetOverlayVisible(false);
						overlayState = OverlayState.Hidden;
					}
					else {
						SetOverlayAlpha(Mathf.Clamp01(1.0f - fadeProgress));
					}
					break;
			}
		}
		
		public static void StartSequence(float daysPassed, float daysLastRun) {
			rewindStartDays = daysLastRun;
			for (int i = 0; i < _controlListeners.Count; i++) {
				if (_controlListeners[i] == null) {
					_controlListeners.RemoveAt(i);
				}
				else {
					_controlListeners[i].SetCurrentTimeText(daysPassed);
					_controlListeners[i].FadeIn();
				}
			}
		}
		
		private void FadeIn() {
			switch (overlayState) {
				case OverlayState.Hidden:
					FadeToBlack();
					break;
				case OverlayState.Black:
					FadeToVisible();
					break;
				case OverlayState.FadingVisibleToHidden:
					FadeToBlack();
					break;
			}
		}
		
		private void FadeToBlack() {
			fadeStartTime = Time.time;
			SetBlackSlateEnabled(true);
			overlayState = OverlayState.FadingHiddenToBlack;
		}
		
		private void FadeToVisible() {
			fadeStartTime = Time.time;
			SetOverlayVisible(true);
			overlayState = OverlayState.FadingBlackToVisible;
		}
		
		private void FadeOut() {
			fadeStartTime = Time.time;
			overlayState = OverlayState.FadingVisibleToHidden;
			FadeOutCallback?.Invoke();
		}
		
		private void SetCurrentTimeText(float daysPassed) {
			currentTimeText.text = ApproximateDate(daysPassed);
		}
		
		private static string ApproximateDate(float daysPassed) {
			int year = 1887 + (int)(daysPassed / 365.25f);
			int month = (int)((daysPassed % 365.25f) / 30.4375f);
			return MonthName(month) + " " + year;
		}
		
		private static string MonthName(int monthIndex) {
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
		
		private static void SetColorAlpha(Graphic image, float alpha) {
			Color color = image.color;
			color.a = alpha;
			image.color = color;
		}
		
		private void SetOverlayEnabled(bool enabled) {
			backgroundImage.enabled = enabled;
			currentTimeText.enabled = enabled;
			daysSurvivedLabel.enabled = enabled;
			daysSurvivedText.enabled = enabled;
		}
		
		private void SetOverlayAlpha(float alpha) {
			SetColorAlpha(backgroundImage, alpha);
			SetColorAlpha(currentTimeText, alpha);
			SetColorAlpha(daysSurvivedLabel, alpha);
			SetColorAlpha(daysSurvivedText, alpha);
		}
		
		private void SetOverlayVisible(bool visible) {
			SetOverlayEnabled(visible);
			SetOverlayAlpha(visible ? 1.0f : 0.0f);
		}
		
		private void SetBlackSlateEnabled(bool enabled) {
			blackSlate.enabled = enabled;
		}
		
		private void SetBlackSlateAlpha(float alpha) {
			SetColorAlpha(blackSlate, alpha);
		}
		
		private void SetBlackSlateVisible(bool visible) {
			SetBlackSlateEnabled(visible);
			SetBlackSlateAlpha(visible ? 1.0f : 0.0f);
		}
		
		private void DelayForSeconds(Callback callback, float seconds) {
			StartCoroutine(Util.DelayCoroutine(callback, seconds));
		}
		
	}
	
}
