using System.Collections.Generic;

public class CardDrawQueue {
	
	private readonly List<IFollowupCard> queue = new List<IFollowupCard>();
	
	public IFollowupCard Next() {
		if (queue.Count > 0) {
			if (--queue[0].Delay == 0) {
				IFollowupCard followup = queue[0];
				queue.RemoveAt(0);
				return followup;
			}
		}
		return null;
	}
	
	public void Insert(IFollowupCard followup) {
		int i = 0;
		while (i < queue.Count && queue[i].Delay <= followup.Delay) {
			i++;
		}
		queue.Insert(i, followup.Clone());
		
		if (i > 0) {
			queue[i].Delay -= queue[i - 1].Delay;
			if (queue[i].Delay == 0) {
				queue[i].Delay++;
			}
		}
		if (i < queue.Count - 1) {
			queue[i + 1].Delay -= queue[i].Delay;
			if (queue[i + 1].Delay == 0) {
				queue[i + 1].Delay++;
			}
		}
	}
	
}
