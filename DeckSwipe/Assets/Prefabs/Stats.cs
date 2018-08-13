public class Stats {
	
	public static int Heat { get; private set; }
	public static int Food { get; private set; }
	public static int Hope { get; private set; }
	public static int Materials { get; private set; }
	
	public static void ApplyModification(StatsModification mod) {
		Heat += mod.Heat;
		Food += mod.Food;
		Hope += mod.Hope;
		Materials += mod.Materials;
	}
	
	public static void Reset() {
		Heat = 16;
		Food = 16;
		Hope = 16;
		Materials = 16;
	}
	
}
