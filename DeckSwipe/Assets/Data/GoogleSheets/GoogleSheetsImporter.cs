using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

namespace GoogleSheets {
	
	public class GoogleSheetsImporter {
		
		private const string _spreadsheetId = "1olhKo6JFItKpDU9Qd7X4cJgaAlFIChhB-P0rI48gNLs";
		private const string _apiKey = "AIzaSyAzWqJRSu7Q3p3EfuwFYdtzQql7ygu1pv4";
		private const int _majorFormatVersion = 3;
		private const int _minorFormatVersion = 0;
		
		private readonly Sprite defaultSprite;
		
		public GoogleSheetsImporter(Sprite defaultSprite) {
			this.defaultSprite = defaultSprite;
		}
		
		public async Task<ImportedCards> FetchCards() {
			Debug.Log("[GoogleSheetsImporter] Fetching cards from Google Sheet " + _spreadsheetId + " ...");
			HttpWebResponse response;
			try {
				HttpWebRequest request = WebRequest.CreateHttp(
						"https://sheets.googleapis.com/v4/spreadsheets/"
						+ _spreadsheetId
						+ "?includeGridData=true&key="
						+ _apiKey);
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

			RowData[] metaRowData = spreadsheet.sheets[0].data[0].rowData;
			Dictionary<string, CellData> metadata = new Dictionary<string, CellData>();
			foreach (RowData row in metaRowData) {
				if (metadata.ContainsKey(row.values[0].formattedValue)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate key found in Metadata sheet");
				}
				else {
					metadata.Add(row.values[0].formattedValue, row.values[1]);
				}
			}

			if (!RequireMetadata("majorFormatVersion", metadata)) {
				return new ImportedCards();
			}
			if (!RequireMetadata("minorFormatVersion", metadata)) {
				return new ImportedCards();
			}
			int sheetMajorVersion = (int) metadata["majorFormatVersion"].effectiveValue.numberValue;
			if (sheetMajorVersion != _majorFormatVersion) {
				Debug.LogError("[GoogleSheetsImporter] Incompatible sheet format major version (required: " + _majorFormatVersion + ", found: " + sheetMajorVersion + ")");
				return new ImportedCards();
			}
			int sheetMinorVersion = (int) metadata["minorFormatVersion"].effectiveValue.numberValue;
			if (sheetMinorVersion < _minorFormatVersion) {
				Debug.LogError("[GoogleSheetsImporter] Incompatible sheet format minor version (required min: " + _minorFormatVersion + ", found: " + sheetMinorVersion + ")");
				return new ImportedCards();
			}

			if (!RequireMetadata("cardSheetIndex", metadata)) {
				return new ImportedCards();
			}
			if (!RequireMetadata("specialCardSheetIndex", metadata)) {
				return new ImportedCards();
			}
			if (!RequireMetadata("characterSheetIndex", metadata)) {
				return new ImportedCards();
			}
			if (!RequireMetadata("imageSheetIndex", metadata)) {
				return new ImportedCards();
			}
			int cardSheetIndex = (int) metadata["cardSheetIndex"].effectiveValue.numberValue;
			int specialCardSheetIndex = (int) metadata["specialCardSheetIndex"].effectiveValue.numberValue;
			int characterSheetIndex = (int) metadata["characterSheetIndex"].effectiveValue.numberValue;
			int imageSheetIndex = (int) metadata["imageSheetIndex"].effectiveValue.numberValue;
			
			RowData[] cardRowData = spreadsheet.sheets[cardSheetIndex].data[0].rowData;
			if (!ValidateCardSheetFormat(cardRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid card format encountered in the spreadsheet");
				return new ImportedCards();
			}
			RowData[] specialCardRowData = spreadsheet.sheets[specialCardSheetIndex].data[0].rowData;
			if (!ValidateCardSheetFormat(specialCardRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid special card format encountered in the spreadsheet");
				return new ImportedCards();
			}
			RowData[] characterRowData = spreadsheet.sheets[characterSheetIndex].data[0].rowData;
			if (!ValidateCharacterSheetFormat(characterRowData[0])) {
				Debug.LogError("[GoogleSheetsImporter] Invalid character format encountered in the spreadsheet");
				return new ImportedCards();
			}
			RowData[] imageRowData = spreadsheet.sheets[imageSheetIndex].data[0].rowData;
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
							out character.sprite);
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
							out card.character);
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
							out card.character);
					specialCards.Add(id, card);
				}
			}
			
			Debug.Log("[GoogleSheetsImporter] Cards imported successfully");
			return new ImportedCards(cards, specialCards);
		}
		
		private static bool ValidateCardSheetFormat(RowData headerRow) {
			return headerRow.values[0].formattedValue == "id"
			       && headerRow.values[1].formattedValue == "characterId"
			       && headerRow.values[2].formattedValue == "cardText"
			       && headerRow.values[3].formattedValue == "leftActionText"
			       && headerRow.values[4].formattedValue == "leftActionCoal"
			       && headerRow.values[5].formattedValue == "leftActionFood"
			       && headerRow.values[6].formattedValue == "leftActionHealth"
			       && headerRow.values[7].formattedValue == "leftActionHope"
			       && headerRow.values[8].formattedValue == "rightActionText"
			       && headerRow.values[9].formattedValue == "rightActionCoal"
			       && headerRow.values[10].formattedValue == "rightActionFood"
			       && headerRow.values[11].formattedValue == "rightActionHealth"
			       && headerRow.values[12].formattedValue == "rightActionHope";
		}
		
		private static bool ValidateCharacterSheetFormat(RowData headerRow) {
			return headerRow.values[0].formattedValue == "id"
			       && headerRow.values[1].formattedValue == "name"
			       && headerRow.values[2].formattedValue == "imageId";
		}
		
		private static bool ValidateImageSheetFormat(RowData headerRow) {
			return headerRow.values[0].formattedValue == "id"
			       && headerRow.values[1].formattedValue == "url";
		}

		private static bool RequireMetadata(string key, Dictionary<string, CellData> metadata) {
			if (!metadata.ContainsKey(key)) {
				Debug.LogError("[GoogleSheetsImporter] " + key + " not found in Metadata sheet");
				return false;
			}
			return true;
		}
		
	}
	
}
