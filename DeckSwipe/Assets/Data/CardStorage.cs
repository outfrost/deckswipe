using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoogleSheets;
using UnityEngine;

public class CardStorage {
	
	private static readonly CharacterModel _defaultGameOverCharacter = new CharacterModel("", null);
	
	private readonly Sprite defaultSprite;
	
	public Dictionary<int, CardModel> Cards { get; private set; }
	public Dictionary<string, CardModel> SpecialCards { get; private set; }
	
	public Task CardCollectionImport { get; }
	
	public CardStorage(Sprite defaultSprite) {
		this.defaultSprite = defaultSprite;
		CardCollectionImport = PopulateCollection();
	}
	
	public CardModel Random() {
		return Cards.ElementAt(UnityEngine.Random.Range(0, Cards.Count)).Value;
	}
	
	public CardModel ForId(int id) {
		CardModel card;
		Cards.TryGetValue(id, out card);
		return card;
	}
	
	public CardModel SpecialCard(string id) {
		CardModel card;
		SpecialCards.TryGetValue(id, out card);
		return card;
	}
	
	private async Task PopulateCollection() {
		ImportedCards importedCards = await new GoogleSheetsImporter(defaultSprite).FetchCards();
		Cards = importedCards.cards;
		SpecialCards = importedCards.specialCards;
		if (Cards == null || Cards.Count == 0) {
			PopulateFallback();
		}
		VerifySpecialCards();
	}
	
	private void PopulateFallback() {
		Cards = new Dictionary<int, CardModel>();
		CharacterModel leadExplorer = new CharacterModel("Lead explorer", defaultSprite);
		Cards.Add(0, new CardModel("This is a test card text that should appear on screen.",
				"Here goes",
				"There goes",
				new CharacterModel("Dr. Bartholomew Oobleck", defaultSprite), 
				new CardActionOutcome(-4, 4, -2, 2),
				new CardActionOutcome(2, 0, 4, -2),
				new List<ICardPrerequisite>()));
		Cards.Add(1, new CardModel("And this is yet another card that you may encounter in the game. Are you having fun thus far?",
				"No",
				"Yes",
				new CharacterModel("Peter Port", defaultSprite),
				new CardActionOutcome(-1, -1, -1, -1),
				new CardActionOutcome(2, 2, 2, 2),
				new List<ICardPrerequisite>()));
		Cards.Add(2, new CardModel("The hothouse is very cold. The crops are freezing up. What should we do?",
				"Turn on the heater in the hothouse",
				"Put the generator in overdrive",
				new CharacterModel("Oliver Rain", defaultSprite),
				new CardActionOutcome(1, 1, 0, -2),
				new CardActionOutcome(2, 2, -2, -4),
				new List<ICardPrerequisite>()));
		Cards.Add(3, new CardModel("The scouts come across what looks to be a wrecked ship. Should they explore it?",
				"No",
				"Yes",
				leadExplorer,
				new CardActionOutcome(0, 0, 0, 0),
				new CardActionOutcome(0, 0, 0, 0),
				new List<ICardPrerequisite>()));
		Cards.Add(4, new CardModel("The scouts find 2 steam cores and a shipment of wood on board the ship. A crew had been trying to survive in the wreck but everybody's dead now.",
				"",
				"",
				leadExplorer,
				new CardActionOutcome(3, 0, 2, 6),
				new CardActionOutcome(3, 0, 2, 6),
				new List<ICardPrerequisite>()));
		Cards.Add(5, new CardModel("The guards bring a woman to you. She yells \"These swine have been pestering me day in day out! How much does it take to tell them to leave me alone?\".",
				"Reprimend the guards",
				"Leave it be",
				new CharacterModel("", defaultSprite),
				new CardActionOutcome(0, 1, 1, 0),
				new CardActionOutcome(0, 0, -1, 0),
				new List<ICardPrerequisite>()));
		Cards.Add(6, new CardModel("An automaton got stuck in a hothouse. What do we do with it?",
				"Disable the automaton",
				"Disable the hothouse",
				new CharacterModel("Alice Woodrow", defaultSprite),
				new CardActionOutcome(-2, -2, 0, -2),
				new CardActionOutcome(+2, -4, 0, -2),
				new List<ICardPrerequisite>()));
	}
	
	private void VerifySpecialCards() {
		if (SpecialCards == null) {
			SpecialCards = new Dictionary<string, CardModel>();
		}
		
		if (!SpecialCards.ContainsKey("gameover_coal")) {
			SpecialCards.Add("gameover_coal", new CardModel("The city runs out of coal to run the generator, and freezes over.", "", "",
					_defaultGameOverCharacter,
					new GameOverCardOutcome(),
					new GameOverCardOutcome(),
					null));
		}
		if (!SpecialCards.ContainsKey("gameover_food")) {
			SpecialCards.Add("gameover_food", new CardModel("Hunger consumes the city, as food reserves deplete.", "", "",
					_defaultGameOverCharacter,
					new GameOverCardOutcome(),
					new GameOverCardOutcome(),
					null));
		}
		if (!SpecialCards.ContainsKey("gameover_health")) {
			SpecialCards.Add("gameover_health", new CardModel("The city's population succumbs to wounds and spreading diseases.", "", "",
					_defaultGameOverCharacter,
					new GameOverCardOutcome(),
					new GameOverCardOutcome(),
					null));
		}
		if (!SpecialCards.ContainsKey("gameover_hope")) {
			SpecialCards.Add("gameover_hope", new CardModel("All hope among the people is lost.", "", "",
					_defaultGameOverCharacter,
					new GameOverCardOutcome(),
					new GameOverCardOutcome(),
					null));
		}
	}
	
}
