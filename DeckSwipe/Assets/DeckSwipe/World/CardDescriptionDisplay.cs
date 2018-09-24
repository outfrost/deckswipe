using System.Collections.Generic;
using Outfrost;
using TMPro;
using UnityEngine;

namespace DeckSwipe.World {
	
	public class CardDescriptionDisplay : MonoBehaviour {
		
		private static readonly List<CardDescriptionDisplay> _changeListeners = new List<CardDescriptionDisplay>();
		
		public TextMeshProUGUI cardText;
		public TextMeshProUGUI characterNameText;
		
		private void Awake() {
			if (!Util.IsPrefab(gameObject)) {
				_changeListeners.Add(this);
				ResetDescription();
			}
		}
		
		public static void SetDescription(string cardCaption, string characterName) {
			SetAllDisplays(cardCaption, characterName);
		}
		
		public static void ResetDescription() {
			SetDescription("", "");
		}
		
		private static void SetAllDisplays(string cardCaption, string characterName) {
			for (int i = 0; i < _changeListeners.Count; i++) {
				if (_changeListeners[i] == null) {
					_changeListeners.RemoveAt(i);
				}
				else {
					_changeListeners[i].SetDisplay(cardCaption, characterName);
				}
			}
		}
		
		private void SetDisplay(string cardCaption, string characterName) {
			cardText.text = cardCaption;
			characterNameText.text = characterName;
		}
		
	}
	
}
