public class StatsModification {
	
	public int Heat;
	public int Food;
	public int Hope;
	public int Materials;
	
	public StatsModification(int heat, int food, int hope, int materials) {
		Heat = heat;
		Food = food;
		Hope = hope;
		Materials = materials;
	}
	
	public void Perform() {
		// TODO Pass through status effects
		Stats.ApplyModification(this);
	}
	
}
