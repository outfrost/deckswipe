using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardDescriptionDisplay : MonoBehaviour {
	
	public TextMeshProUGUI CardText;
	public TextMeshProUGUI CharacterNameText;
	
	private static string cardTextString = "";
	private static string characterNameString = "";
	
	private static readonly List<CardDescriptionDisplay> changeListeners = new List<CardDescriptionDisplay>();
	
	private void Awake() {
		if (!Util.IsPrefab(gameObject)) {
			changeListeners.Add(this);
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
		for (int i = 0; i < changeListeners.Count; i++) {
			if (changeListeners[i] == null) {
				changeListeners.RemoveAt(i);
			}
			else {
				changeListeners[i].UpdateTextDisplay();
			}
		}
	}
	
	private void UpdateTextDisplay() {
		CardText.text = cardTextString;
		CharacterNameText.text = characterNameString;
	}
	
}
