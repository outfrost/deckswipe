using System.Collections.Generic;
using Outfrost;
using TMPro;
using UnityEngine;

namespace DeckSwipe.World {

	public class ProgressDisplay : MonoBehaviour {
	
		private static readonly List<ProgressDisplay> _changeListeners = new List<ProgressDisplay>();
	
		private static int daysSurvived;
	
		public TextMeshProUGUI daysSurvivedText;
	
		private void Awake() {
			if (!Util.IsPrefab(gameObject)) {
				_changeListeners.Add(this);
				UpdateProgressDisplay();
			}
		}
	
		public static void SetDaysSurvived(int days) {
			daysSurvived = days;
			UpdateAllProgressDisplays();
		}
	
		private static void UpdateAllProgressDisplays() {
			for (int i = 0; i < _changeListeners.Count; i++) {
				if (_changeListeners[i] == null) {
					_changeListeners.RemoveAt(i);
				}
				else {
					_changeListeners[i].UpdateProgressDisplay();
				}
			}
		}
	
		private void UpdateProgressDisplay() {
			daysSurvivedText.text = daysSurvived.ToString();
		}
	
	}

}
