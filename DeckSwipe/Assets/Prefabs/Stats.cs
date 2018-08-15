using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour {
	
	public RectTransform HeatBar;
	public RectTransform FoodBar;
	public RectTransform HopeBar;
	public RectTransform MaterialsBar;
	
	private const int maxStatValue = 32;
	private const int startingHeat = 16;
	private const int startingFood = 16;
	private const int startingHope = 16;
	private const int startingMaterials = 16;
	
	public static int Heat { get; private set; }
	public static int Food { get; private set; }
	public static int Hope { get; private set; }
	public static int Materials { get; private set; }
	
	private static LinkedList<Stats> changeListeners = new LinkedList<Stats>();
	
	public static void ApplyModification(StatsModification mod) {
		Heat += mod.Heat;
		Food += mod.Food;
		Hope += mod.Hope;
		Materials += mod.Materials;
		foreach (Stats listener in changeListeners) {
			listener.UpdateStatBars();
		}
	}
	
	public static void ResetStats() {
		Heat = startingHeat;
		Food = startingFood;
		Hope = startingHope;
		Materials = startingMaterials;
	}

	public Stats() {
		changeListeners.AddLast(this);
	}
	
	void Awake() {
		ResetStats();
	}
	
	void Start() {
		UpdateStatBars();
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
