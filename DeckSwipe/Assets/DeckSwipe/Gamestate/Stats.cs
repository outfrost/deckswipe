using System.Collections.Generic;
using DeckSwipe.CardModel;
using Outfrost;
using UnityEngine;
using UnityEngine.UI;

namespace DeckSwipe.Gamestate {
	
	public class Stats : MonoBehaviour {
		
		private const int _maxStatValue = 32;
		private const int _startingCoal = 16;
		private const int _startingFood = 16;
		private const int _startingHealth = 16;
		private const int _startingHope = 16;
		
		private static readonly List<Stats> _changeListeners = new List<Stats>();
		
		public static int Coal { get; private set; }
		public static int Food { get; private set; }
		public static int Health { get; private set; }
		public static int Hope { get; private set; }
		
		public Image coalBar;
		public Image foodBar;
		public Image healthBar;
		public Image hopeBar;
		public float relativeMargin;
		
		private float minFillAmount;
		private float maxFillAmount;
		
		/*static Stats() {
		ApplyStartingValues();
	}*/
		
		private void Awake() {
			if (!Util.IsPrefab(gameObject)) {
				_changeListeners.Add(this);
				UpdateStatBars();
			}
			minFillAmount = Mathf.Clamp01(relativeMargin);
			maxFillAmount = Mathf.Clamp01(1.0f - relativeMargin);
		}
		
		public static void ApplyModification(StatsModification mod) {
			Coal = ClampValue(Coal + mod.coal);
			Food = ClampValue(Food + mod.food);
			Health = ClampValue(Health + mod.health);
			Hope = ClampValue(Hope + mod.hope);
			UpdateAllStatBars();
		}
		
		public static void ResetStats() {
			ApplyStartingValues();
			UpdateAllStatBars();
		}
		
		private static void ApplyStartingValues() {
			Coal = ClampValue(_startingCoal);
			Food = ClampValue(_startingFood);
			Health = ClampValue(_startingHealth);
			Hope = ClampValue(_startingHope);
		}
		
		private static void UpdateAllStatBars() {
			for (int i = 0; i < _changeListeners.Count; i++) {
				if (_changeListeners[i] == null) {
					_changeListeners.RemoveAt(i);
				}
				else {
					_changeListeners[i].UpdateStatBars();
				}
			}
		}
		
		private void UpdateStatBars() {
			coalBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, (float) Coal / _maxStatValue);
			foodBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, (float) Food / _maxStatValue);
			healthBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, (float) Health / _maxStatValue);
			hopeBar.fillAmount = Mathf.Lerp(minFillAmount, maxFillAmount, (float) Hope / _maxStatValue);
		}
		
		private static int ClampValue(int value) {
			return Mathf.Clamp(value, 0, _maxStatValue);
		}
		
	}
	
}
