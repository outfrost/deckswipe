using System.Collections.Generic;
using Outfrost;
using TMPro;
using UnityEngine;

namespace DeckSwipe.World {
	
	public class ProgressDisplay : MonoBehaviour {
		
		private static readonly List<ProgressDisplay> _changeListeners = new List<ProgressDisplay>();
		
		public TextMeshProUGUI daysSurvivedText;
		
		private void Awake() {
			if (!Util.IsPrefab(gameObject)) {
				_changeListeners.Add(this);
				SetDisplay(0);
			}
		}
		
		public static void SetDaysSurvived(int days) {
			SetAllDisplays(days);
		}
		
		private static void SetAllDisplays(int days) {
			for (int i = 0; i < _changeListeners.Count; i++) {
				if (_changeListeners[i] == null) {
					_changeListeners.RemoveAt(i);
				}
				else {
					_changeListeners[i].SetDisplay(days);
				}
			}
		}
		
		private void SetDisplay(int days) {
			daysSurvivedText.text = days.ToString();
		}
		
	}
	
}
