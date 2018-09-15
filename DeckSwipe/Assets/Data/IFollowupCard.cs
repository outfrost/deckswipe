public interface IFollowupCard {
	
	int Delay { get; set; }
	
	IFollowupCard Clone();
	CardModel Fetch(CardStorage cardStorage);

}
