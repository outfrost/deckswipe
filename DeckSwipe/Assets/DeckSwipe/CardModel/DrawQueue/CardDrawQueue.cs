using System.Collections.Generic;

namespace DeckSwipe.CardModel.DrawQueue {
	
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
			int delayBefore = 0;
			while (i < queue.Count && (delayBefore < followup.Delay || queue[i].Delay == 1)) {
				delayBefore += queue[i].Delay;
				i++;
			}
			queue.Insert(i, followup.Clone());
			
			queue[i].Delay -= delayBefore;
			if (queue[i].Delay < 1) {
				queue[i].Delay = 1;
			}
			
			if (i + 1 < queue.Count) {
				queue[i + 1].Delay -= queue[i].Delay;
			}
		}
		
	}
	
}
