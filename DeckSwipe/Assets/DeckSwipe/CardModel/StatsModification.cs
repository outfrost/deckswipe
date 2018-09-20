using DeckSwipe.Gamestate;

namespace DeckSwipe.CardModel {

	public class StatsModification {
	
		public int coal;
		public int food;
		public int health;
		public int hope;
	
		public StatsModification(int coal, int food, int health, int hope) {
			this.coal = coal;
			this.food = food;
			this.health = health;
			this.hope = hope;
		}
	
		public void Perform() {
			// TODO Pass through status effects
			Stats.ApplyModification(this);
		}
	
	}

}
