using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressDisplay : MonoBehaviour {
	
	public TextMeshProUGUI DaysSurvivedText;
	
	private static int daysSurvived;
	
	private static readonly List<ProgressDisplay> changeListeners = new List<ProgressDisplay>();
	
	private void Awake() {
		if (!Util.IsPrefab(gameObject)) {
			changeListeners.Add(this);
			UpdateProgressDisplay();
		}
	}
	
	public static void SetDaysSurvived(int days) {
		daysSurvived = days;
		UpdateAllProgressDisplays();
	}
	
	private static void UpdateAllProgressDisplays() {
		for (int i = 0; i < changeListeners.Count; i++) {
			if (changeListeners[i] == null) {
				changeListeners.RemoveAt(i);
			}
			else {
				changeListeners[i].UpdateProgressDisplay();
			}
		}
	}
	
	private void UpdateProgressDisplay() {
		DaysSurvivedText.text = daysSurvived.ToString();
	}
	
}
