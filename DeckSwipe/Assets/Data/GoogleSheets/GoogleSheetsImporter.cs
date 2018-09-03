using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace GoogleSheets {
	
	public class GoogleSheetsImporter {
		
		private static string spreadsheetId = "1olhKo6JFItKpDU9Qd7X4cJgaAlFIChhB-P0rI48gNLs";
		private static string apiKey = "AIzaSyAzWqJRSu7Q3p3EfuwFYdtzQql7ygu1pv4";
		
		private Sprite defaultSprite;
		
		public GoogleSheetsImporter(Sprite defaultSprite) {
			this.defaultSprite = defaultSprite;
		}
		
		public async Task<ImportedCards> FetchCards() {
			Debug.Log("[GoogleSheetsImporter] Fetching cards from Google Sheet " + spreadsheetId + " ...");
			HttpWebResponse response;
			try {
				HttpWebRequest request = WebRequest.CreateHttp(
						"https://sheets.googleapis.com/v4/spreadsheets/"
						+ spreadsheetId
						+ "?includeGridData=true&key="
						+ apiKey);
				// TODO Handle Web exceptions
				response = (HttpWebResponse) await request.GetResponseAsync();
			}
			catch (WebException e) {
				Debug.LogError("[GoogleSheetsImporter] Request failed: " + e.Message);
				return new ImportedCards();
			}
			Debug.Log("[GoogleSheetsImporter] " + (int)response.StatusCode + " " + response.StatusDescription);
			
			if (!response.ContentType.Contains("application/json")) {
				Debug.LogError("[GoogleSheetsImporter] Google Sheets API returned unrecognised data format");
				return new ImportedCards();
			}
			
			Stream responseStream;
			if ((responseStream = response.GetResponseStream()) == null) {
				Debug.LogError("[GoogleSheetsImporter] Google Sheets API returned empty response");
				return new ImportedCards();
			}
			
			Spreadsheet spreadsheet = JsonUtility.FromJson<Spreadsheet>(
					new StreamReader(responseStream).ReadToEnd());
			
			RowData[] cardRowData = spreadsheet.sheets[0].data[0].rowData;
			if (!ValidateCardSheetFormat(cardRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid card format encountered in the spreadsheet");
				return new ImportedCards();
			}
			RowData[] specialCardRowData = spreadsheet.sheets[1].data[0].rowData;
			if (!ValidateCardSheetFormat(specialCardRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid special card format encountered in the spreadsheet");
				return new ImportedCards();
			}
			RowData[] characterRowData = spreadsheet.sheets[2].data[0].rowData;
			if (!ValidateCharacterSheetFormat(characterRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid character format encountered in the spreadsheet");
				return new ImportedCards();
			}
			RowData[] imageRowData = spreadsheet.sheets[3].data[0].rowData;
			if (!ValidateImageSheetFormat(imageRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid character format encountered in the spreadsheet");
				return new ImportedCards();
			}
			
			Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
			for (int i = 1; i < imageRowData.Length; i++) {
				int id = (int) imageRowData[i].values[0].effectiveValue.numberValue;
				if (sprites.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in Images sheet");
				}
				else {
					Debug.Log("[GoogleSheetsImporter] Fetching image from " + imageRowData[i].values[1].hyperlink + " ...");
					HttpWebRequest imageRequest = WebRequest.CreateHttp(imageRowData[i].values[1].hyperlink);
					HttpWebResponse imageResponse = (HttpWebResponse) await imageRequest.GetResponseAsync();
					Debug.Log("[GoogleSheetsImporter] " + (int)imageResponse.StatusCode + " " + imageResponse.StatusDescription);
					
					Stream imageStream;
					if ((imageStream = imageResponse.GetResponseStream()) == null) {
						Debug.LogWarning("[GoogleSheetsImporter] Remote host returned no image in response");
					}
					else {
						byte[] imageData = Util.BytesFromStream(imageStream);
						Texture2D texture = new Texture2D(1, 1);
						if (!texture.LoadImage(imageData)) {
							Debug.LogWarning("[GoogleSheetsImporter] Could not create sprite texture from image");
						}
						else {
							Sprite sprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height),
									new Vector2(0.5f, 0.5f));
							sprites.Add(id, sprite);
						}
					}
				}
			}
			
			Dictionary<int, CharacterModel> characters = new Dictionary<int, CharacterModel>();
			for (int i = 1; i < characterRowData.Length; i++) {
				int id = (int) characterRowData[i].values[0].effectiveValue.numberValue;
				if (characters.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in Images sheet");
				}
				else {
					CharacterModel character = new CharacterModel(characterRowData[i].values[1].formattedValue,
							defaultSprite);
					sprites.TryGetValue((int) characterRowData[i].values[2].effectiveValue.numberValue,
							out character.Sprite);
					characters.Add(id, character);
				}
			}
			
			Dictionary<int, CardModel> cards = new Dictionary<int, CardModel>();
			for (int i = 1; i < cardRowData.Length; i++) {
				int id = (int) cardRowData[i].values[0].effectiveValue.numberValue;
				if (cards.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in Cards sheet");
				}
				else {
					CardModel card = new CardModel(
							cardRowData[i].values[2].formattedValue,
							cardRowData[i].values[3].formattedValue,
							cardRowData[i].values[8].formattedValue,
							null,
							new CardActionOutcome(
									(int) cardRowData[i].values[4].effectiveValue.numberValue,
									(int) cardRowData[i].values[5].effectiveValue.numberValue,
									(int) cardRowData[i].values[6].effectiveValue.numberValue,
									(int) cardRowData[i].values[7].effectiveValue.numberValue),
							new CardActionOutcome(
									(int) cardRowData[i].values[9].effectiveValue.numberValue,
									(int) cardRowData[i].values[10].effectiveValue.numberValue,
									(int) cardRowData[i].values[11].effectiveValue.numberValue,
									(int) cardRowData[i].values[12].effectiveValue.numberValue));
					characters.TryGetValue((int) cardRowData[i].values[1].effectiveValue.numberValue,
							out card.Character);
					cards.Add(id, card);
				}
			}
			
			Dictionary<string, CardModel> specialCards = new Dictionary<string, CardModel>();
			for (int i = 1; i < specialCardRowData.Length; i++) {
				string id = specialCardRowData[i].values[0].formattedValue;
				if (specialCards.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in SpecialCards sheet");
				}
				else {
					CardModel card = new CardModel(
							specialCardRowData[i].values[2].formattedValue,
							specialCardRowData[i].values[3].formattedValue,
							specialCardRowData[i].values[8].formattedValue,
							null,
							new GameOverCardOutcome(),
							new GameOverCardOutcome());
					characters.TryGetValue((int) specialCardRowData[i].values[1].effectiveValue.numberValue,
							out card.Character);
					specialCards.Add(id, card);
				}
			}
			
			Debug.Log("[GoogleSheetsImporter] Cards imported successfully");
			return new ImportedCards(cards, specialCards);
		}
		
		private static bool ValidateCardSheetFormat(RowData headerRow) {
			return headerRow.values[0].formattedValue == "id"
			       && headerRow.values[1].formattedValue == "character_id"
			       && headerRow.values[2].formattedValue == "card_text"
			       && headerRow.values[3].formattedValue == "left_swipe_text"
			       && headerRow.values[4].formattedValue == "left_swipe_heat"
			       && headerRow.values[5].formattedValue == "left_swipe_food"
			       && headerRow.values[6].formattedValue == "left_swipe_hope"
			       && headerRow.values[7].formattedValue == "left_swipe_materials"
			       && headerRow.values[8].formattedValue == "right_swipe_text"
			       && headerRow.values[9].formattedValue == "right_swipe_heat"
			       && headerRow.values[10].formattedValue == "right_swipe_food"
			       && headerRow.values[11].formattedValue == "right_swipe_hope"
			       && headerRow.values[12].formattedValue == "right_swipe_materials";
		}
		
		private static bool ValidateCharacterSheetFormat(RowData headerRow) {
			return headerRow.values[0].formattedValue == "id"
			       && headerRow.values[1].formattedValue == "name"
			       && headerRow.values[2].formattedValue == "image_id";
		}
		
		private static bool ValidateImageSheetFormat(RowData headerRow) {
			return headerRow.values[0].formattedValue == "id"
			       && headerRow.values[1].formattedValue == "url";
		}
		
	}
	
}
