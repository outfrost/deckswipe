using Persistence;

public interface ICardPrerequisite {
	
	CardStatus Status { get; set; }
	
	bool IsSatisfied(CardStorage cardStorage);
	
}
