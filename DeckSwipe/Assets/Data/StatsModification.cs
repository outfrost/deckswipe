public class StatsModification {
	
	public int heat;
	public int food;
	public int hope;
	public int materials;
	
	public StatsModification(int heat, int food, int hope, int materials) {
		this.heat = heat;
		this.food = food;
		this.hope = hope;
		this.materials = materials;
	}
	
	public void Perform() {
		// TODO Pass through status effects
		Stats.ApplyModification(this);
	}
	
}
