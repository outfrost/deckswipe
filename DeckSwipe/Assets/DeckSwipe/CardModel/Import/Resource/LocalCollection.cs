using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DeckSwipe.CardModel.Import;
using Outfrost;
using UnityEngine;

namespace DeckSwipe.CardModel.Import.Resource {

	public class LocalCollection {

		private readonly string _collectionPath = Application.dataPath + "/Resources/Collection";
		private readonly string _cardsPath = Application.dataPath + "/Resources/Collection" + "/Cards";
		private readonly string _specialCardsPath = Application.dataPath + "/Resources/Collection" + "/SpecialCards";
		private readonly string _charactersPath = Application.dataPath + "/Resources/Collection" + "/Characters";
		private readonly string _imagesPath = Application.dataPath + "/Resources/Collection" + "/Images";

		public async Task<ProtoCollection> Fetch() {
			List<ProtoCard> cards = new List<ProtoCard>();
			List<ProtoSpecialCard> specialCards = new List<ProtoSpecialCard>();
			List<ProtoCharacter> characters = new List<ProtoCharacter>();
			List<ProtoImage> images = new List<ProtoImage>();

			var ioTasks = new List<Task>();

			ioTasks.Add(Task.Run(async () => {
				foreach (string path in Directory.EnumerateFiles(
						_cardsPath, "*.json", SearchOption.AllDirectories)) {
					ProtoCard card = await new JsonFile<ProtoCard>(path).Deserialize();
					if (card != null) {
						cards.Add(card);
					}
				}
			}));

			ioTasks.Add(Task.Run(async () => {
				foreach (string path in Directory.EnumerateFiles(
						_specialCardsPath, "*.json", SearchOption.AllDirectories)) {
					ProtoSpecialCard specialCard = await new JsonFile<ProtoSpecialCard>(path).Deserialize();
					if (specialCard != null) {
						specialCards.Add(specialCard);
					}
				}
			}));

			ioTasks.Add(Task.Run(async () => {
				foreach (string path in Directory.EnumerateFiles(
						_charactersPath, "*.json", SearchOption.AllDirectories)) {
					ProtoCharacter character = await new JsonFile<ProtoCharacter>(path).Deserialize();
					if (character != null) {
						characters.Add(character);
					}
				}
			}));

			ioTasks.Add(Task.Run(async () => {
				foreach (string path in Directory.EnumerateFiles(
						_imagesPath, "*.json", SearchOption.AllDirectories)) {
					ProtoImage image = await new JsonFile<ProtoImage>(path).Deserialize();
					if (image != null) {
						images.Add(image);
					}
				}
			}));

			await Task.WhenAll(ioTasks);

			return new ProtoCollection(cards, specialCards, characters, images);
		}

	}

}
