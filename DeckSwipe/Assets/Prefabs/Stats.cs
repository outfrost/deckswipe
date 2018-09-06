using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
	
	private const int _maxStatValue = 32;
	private const int _startingHeat = 16;
	private const int _startingFood = 16;
	private const int _startingHope = 16;
	private const int _startingMaterials = 16;
	
	private static readonly List<Stats> _changeListeners = new List<Stats>();
	
	public static int Heat { get; private set; }
	public static int Food { get; private set; }
	public static int Hope { get; private set; }
	public static int Materials { get; private set; }
	
	/* TODO Refactor stats
	public static int Coal;
	public static int Food;
	public static int Health;
	public static int Hope;
	*/
	
	public RectTransform heatBar;
	public RectTransform foodBar;
	public RectTransform hopeBar;
	public RectTransform materialsBar;
	
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
		Heat = ClampValue(Heat + mod.heat);
		Food = ClampValue(Food + mod.food);
		Hope = ClampValue(Hope + mod.hope);
		Materials = ClampValue(Materials + mod.materials);
		UpdateAllStatBars();
	}
	
	public static void ResetStats() {
		ApplyStartingValues();
		UpdateAllStatBars();
	}
	
	private static void ApplyStartingValues() {
		Heat = ClampValue(_startingHeat);
		Food = ClampValue(_startingFood);
		Hope = ClampValue(_startingHope);
		Materials = ClampValue(_startingMaterials);
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
		heatBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Heat / _maxStatValue * 100.0f);
		foodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Food / _maxStatValue * 100.0f);
		hopeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Hope / _maxStatValue * 100.0f);
		materialsBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Materials / _maxStatValue * 100.0f);
	}
	
	private static int ClampValue(int value) {
		return Mathf.Clamp(value, 0, _maxStatValue);
	}
	
}
