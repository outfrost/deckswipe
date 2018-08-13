public class StatsModification {
	
	public int Heat;
	public int Food;
	public int Hope;
	public int Materials;

	public void Perform() {
		// TODO Pass through status effects
		Stats.ApplyModification(this);
	}
	
}
