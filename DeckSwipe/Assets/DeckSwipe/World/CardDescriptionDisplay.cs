using System.Collections.Generic;
using Outfrost;
using TMPro;
using UnityEngine;

namespace DeckSwipe.World {
	
	public class CardDescriptionDisplay : MonoBehaviour {
		
		private static readonly List<CardDescriptionDisplay> _changeListeners = new List<CardDescriptionDisplay>();
		
		private static string cardTextString = "";
		private static string characterNameString = "";
		
		public TextMeshProUGUI cardText;
		public TextMeshProUGUI characterNameText;
		
		private void Awake() {
			if (!Util.IsPrefab(gameObject)) {
				_changeListeners.Add(this);
				UpdateTextDisplay();
			}
		}
		
		public static void SetDescription(string cardText, string characterName) {
			cardTextString = cardText;
			characterNameString = characterName;
			UpdateAllTextDisplays();
		}
		
		public static void ResetDescription() {
			SetDescription("", "");
		}
		
		private static void UpdateAllTextDisplays() {
			for (int i = 0; i < _changeListeners.Count; i++) {
				if (_changeListeners[i] == null) {
					_changeListeners.RemoveAt(i);
				}
				else {
					_changeListeners[i].UpdateTextDisplay();
				}
			}
		}
		
		private void UpdateTextDisplay() {
			cardText.text = cardTextString;
			characterNameText.text = characterNameString;
		}
		
	}
	
}
