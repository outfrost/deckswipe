using System.Collections.Generic;
using UnityEngine;

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
	
	public RectTransform coalBar;
	public RectTransform foodBar;
	public RectTransform healthBar;
	public RectTransform hopeBar;
	
	/*static Stats() {
		ApplyStartingValues();
	}*/
	
	private void Awake() {
		if (!Util.IsPrefab(gameObject)) {
			_changeListeners.Add(this);
			UpdateStatBars();
		}
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
		coalBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Coal / _maxStatValue * 100.0f);
		foodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Food / _maxStatValue * 100.0f);
		healthBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Health / _maxStatValue * 100.0f);
		hopeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Hope / _maxStatValue * 100.0f);
	}
	
	private static int ClampValue(int value) {
		return Mathf.Clamp(value, 0, _maxStatValue);
	}
	
}
