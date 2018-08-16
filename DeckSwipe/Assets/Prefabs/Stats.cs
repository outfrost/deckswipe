using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
	
	private const int maxStatValue = 32;
	private const int startingHeat = 16;
	private const int startingFood = 16;
	private const int startingHope = 16;
	private const int startingMaterials = 16;
	
	public static int Heat { get; private set; }
	public static int Food { get; private set; }
	public static int Hope { get; private set; }
	public static int Materials { get; private set; }
	
	private static readonly List<Stats> changeListeners = new List<Stats>();
	
	public RectTransform HeatBar;
	public RectTransform FoodBar;
	public RectTransform HopeBar;
	public RectTransform MaterialsBar;

	static Stats() {
		Heat = startingHeat;
		Food = startingFood;
		Hope = startingHope;
		Materials = startingMaterials;
	}

	public static void ApplyModification(StatsModification mod) {
		Heat += mod.Heat;
		Food += mod.Food;
		Hope += mod.Hope;
		Materials += mod.Materials;
		UpdateAllStatBars();
	}
	
	public static void ResetStats() {
		Heat = startingHeat;
		Food = startingFood;
		Hope = startingHope;
		Materials = startingMaterials;
		UpdateAllStatBars();
	}
	
	private static void UpdateAllStatBars() {
		for (int i = 0; i < changeListeners.Count; i++) {
			if (changeListeners[i] == null) {
				changeListeners.RemoveAt(i);
			}
			else {
				changeListeners[i].UpdateStatBars();
			}
		}
	}
	
	void Awake() {
		if (!Util.IsPrefab(gameObject)) {
			changeListeners.Add(this);
		}
	}
	
	void UpdateStatBars() {
		HeatBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Heat / maxStatValue * 100.0f);
		FoodBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Food / maxStatValue * 100.0f);
		HopeBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Hope / maxStatValue * 100.0f);
		MaterialsBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,
				(float) Materials / maxStatValue * 100.0f);
	}
	
}
