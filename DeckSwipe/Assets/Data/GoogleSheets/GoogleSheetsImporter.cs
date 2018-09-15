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
		private const int _minorFormatVersion = 1;
		
		private readonly Sprite defaultSprite;
		
		public GoogleSheetsImporter(Sprite defaultSprite) {
			this.defaultSprite = defaultSprite;
		}
		
		public async Task<ImportedCards> FetchCards() {
			// Fetch spreadsheet from Google Sheet API V4
			Debug.Log("[GoogleSheetsImporter] Fetching cards from Google Sheet " + _spreadsheetId + " ...");
			HttpWebResponse response;
			try {
				HttpWebRequest request = WebRequest.CreateHttp(
						"https://sheets.googleapis.com/v4/spreadsheets/"
						+ _spreadsheetId
						+ "?includeGridData=true&key="
						+ _apiKey);
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
			
			// Parse Metadata sheet
			RowData[] metaRowData = spreadsheet.sheets[0].data[0].rowData;
			Dictionary<string, CellData> metadata = new Dictionary<string, CellData>();
			foreach (RowData row in metaRowData) {
				if (row.values[0].StringValue != null) {
					if (metadata.ContainsKey(row.values[0].StringValue)) {
						Debug.LogWarning("[GoogleSheetsImporter] Duplicate key found in Metadata sheet");
					}
					else {
						metadata.Add(row.values[0].StringValue, row.values[1]);
					}
				}
			}
			
			// Check sheet format version
			if (!RequireMetadata("majorFormatVersion", metadata)) {
				return new ImportedCards();
			}
			if (!RequireMetadata("minorFormatVersion", metadata)) {
				return new ImportedCards();
			}
			int sheetMajorVersion = metadata["majorFormatVersion"].IntValue;
			if (sheetMajorVersion != _majorFormatVersion) {
				Debug.LogError("[GoogleSheetsImporter] Incompatible sheet format major version (required: " + _majorFormatVersion + ", found: " + sheetMajorVersion + ")");
				return new ImportedCards();
			}
			int sheetMinorVersion = metadata["minorFormatVersion"].IntValue;
			if (sheetMinorVersion < _minorFormatVersion) {
				Debug.LogError("[GoogleSheetsImporter] Incompatible sheet format minor version (required min: " + _minorFormatVersion + ", found: " + sheetMinorVersion + ")");
				return new ImportedCards();
			}
			
			// Get sheet indices from metadata
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
			int cardSheetIndex = metadata["cardSheetIndex"].IntValue;
			int specialCardSheetIndex = metadata["specialCardSheetIndex"].IntValue;
			int characterSheetIndex = metadata["characterSheetIndex"].IntValue;
			int imageSheetIndex = metadata["imageSheetIndex"].IntValue;
			
			// Sanity-check sheet formats
			Sheet cardSheet = spreadsheet.sheets[cardSheetIndex];
			if (!CheckCardSheetFormat(cardSheet)) {
				return new ImportedCards();
			}
			Sheet specialCardSheet = spreadsheet.sheets[specialCardSheetIndex];
			if (!CheckCardSheetFormat(specialCardSheet)) {
				return new ImportedCards();
			}
			Sheet characterSheet = spreadsheet.sheets[characterSheetIndex];
			if (!CheckCharacterSheetFormat(characterSheet)) {
				return new ImportedCards();
			}
			Sheet imageSheet = spreadsheet.sheets[imageSheetIndex];
			if (!CheckImageSheetFormat(imageSheet)) {
				return new ImportedCards();
			}
			
			// Parse Images sheet
			Dictionary<int, Sprite> sprites = new Dictionary<int, Sprite>();
			RowData[] imageRowData = imageSheet.data[0].rowData;
			for (int i = 1; i < imageRowData.Length; i++) {
				int id = imageRowData[i].values[0].IntValue;
				string imageUrl = imageRowData[i].values[1].hyperlink;
				if (sprites.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in Images sheet");
				}
				else if (imageUrl == null) {
					Debug.LogWarning("[GoogleSheetsImporter] Image (id: " + id + ") has a null URL");
				}
				else {
					Debug.Log("[GoogleSheetsImporter] Fetching image from " + imageUrl + " ...");
					HttpWebRequest imageRequest = WebRequest.CreateHttp(imageUrl);
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
			
			// Parse Characters sheet
			Dictionary<int, CharacterModel> characters = new Dictionary<int, CharacterModel>();
			RowData[] characterRowData = characterSheet.data[0].rowData;
			for (int i = 1; i < characterRowData.Length; i++) {
				int id = characterRowData[i].values[0].IntValue;
				if (characters.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in Images sheet");
				}
				else {
					CharacterModel character = new CharacterModel(characterRowData[i].values[1].GetStringValue(""),
							defaultSprite);
					sprites.TryGetValue(characterRowData[i].values[2].IntValue,
							out character.sprite);
					characters.Add(id, character);
				}
			}
			
			// Parse Cards sheet
			Dictionary<int, CardModel> cards = new Dictionary<int, CardModel>();
			RowData[] cardRowData = cardSheet.data[0].rowData;
			for (int i = 1; i < cardRowData.Length; i++) {
				int id = cardRowData[i].values[0].IntValue;
				if (cards.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in Cards sheet");
				}
				else {
					JsonArray<CardPrerequisite> cardPrerequisites =
							JsonUtility.FromJson<JsonArray<CardPrerequisite>>(cardRowData[i].values[13].StringValue);
					JsonArray<SpecialCardPrerequisite> specialCardPrerequisites =
							JsonUtility.FromJson<JsonArray<SpecialCardPrerequisite>>(cardRowData[i].values[14].StringValue);
					List<ICardPrerequisite> prerequisites = new List<ICardPrerequisite>();
					if (cardPrerequisites?.array != null) {
						prerequisites.AddRange(cardPrerequisites.array);
					}
					if (specialCardPrerequisites?.array != null) {
						prerequisites.AddRange(specialCardPrerequisites.array);
					}
					
					CardModel card = new CardModel(
							cardRowData[i].values[2].GetStringValue(""),
							cardRowData[i].values[3].GetStringValue(""),
							cardRowData[i].values[8].GetStringValue(""),
							null,
							new CardActionOutcome(
									cardRowData[i].values[4].IntValue,
									cardRowData[i].values[5].IntValue,
									cardRowData[i].values[6].IntValue,
									cardRowData[i].values[7].IntValue),
							new CardActionOutcome(
									cardRowData[i].values[9].IntValue,
									cardRowData[i].values[10].IntValue,
									cardRowData[i].values[11].IntValue,
									cardRowData[i].values[12].IntValue),
							prerequisites);
					characters.TryGetValue(cardRowData[i].values[1].IntValue,
							out card.character);
					cards.Add(id, card);
				}
			}
			
			// Parse SpecialCards sheet
			Dictionary<string, CardModel> specialCards = new Dictionary<string, CardModel>();
			RowData[] specialCardRowData = specialCardSheet.data[0].rowData;
			for (int i = 1; i < specialCardRowData.Length; i++) {
				string id = specialCardRowData[i].values[0].StringValue;
				if (id == null) {
					Debug.LogWarning("[GoogleSheetsImporter] Null id found in SpecialCards sheet");
				}
				else if (specialCards.ContainsKey(id)) {
					Debug.LogWarning("[GoogleSheetsImporter] Duplicate id found in SpecialCards sheet");
				}
				else {
					CardModel card = new CardModel(
							specialCardRowData[i].values[2].GetStringValue(""),
							specialCardRowData[i].values[3].GetStringValue(""),
							specialCardRowData[i].values[8].GetStringValue(""),
							null,
							new GameOverCardOutcome(),
							new GameOverCardOutcome(),
							null);
					characters.TryGetValue(specialCardRowData[i].values[1].IntValue,
							out card.character);
					specialCards.Add(id, card);
				}
			}
			
			Debug.Log("[GoogleSheetsImporter] Cards imported successfully");
			return new ImportedCards(cards, specialCards);
		}
		
		private static bool CheckCardSheetFormat(Sheet sheet) {
			RowData headerRow = sheet.data[0].rowData[0];
			if (headerRow.values[0].StringValue == "id"
					&& headerRow.values[1].StringValue == "characterId"
					&& headerRow.values[2].StringValue == "cardText"
					&& headerRow.values[3].StringValue == "leftActionText"
					&& headerRow.values[4].StringValue == "leftActionCoal"
					&& headerRow.values[5].StringValue == "leftActionFood"
					&& headerRow.values[6].StringValue == "leftActionHealth"
					&& headerRow.values[7].StringValue == "leftActionHope"
					&& headerRow.values[8].StringValue == "rightActionText"
					&& headerRow.values[9].StringValue == "rightActionCoal"
					&& headerRow.values[10].StringValue == "rightActionFood"
					&& headerRow.values[11].StringValue == "rightActionHealth"
					&& headerRow.values[12].StringValue == "rightActionHope"
					&& headerRow.values[13].StringValue == "cardPrerequisites"
					&& headerRow.values[14].StringValue == "specialCardPrerequisites"
					&& headerRow.values[15].StringValue == "GUARD") {
				return true;
			}
			else {
				Debug.LogError("[GoogleSheetsImporter] Invalid card format encountered in "
				               + sheet.properties.title
				               + " sheet");
				return false;
			}
		}
		
		private static bool CheckCharacterSheetFormat(Sheet sheet) {
			RowData headerRow = sheet.data[0].rowData[0];
			if (headerRow.values[0].StringValue == "id"
					&& headerRow.values[1].StringValue == "name"
					&& headerRow.values[2].StringValue == "imageId"
					&& headerRow.values[3].StringValue == "GUARD") {
				return true;
			}
			else {
				Debug.LogError("[GoogleSheetsImporter] Invalid character format encountered in "
				               + sheet.properties.title
				               + " sheet");
				return false;
			}
		}
		
		private static bool CheckImageSheetFormat(Sheet sheet) {
			RowData headerRow = sheet.data[0].rowData[0];
			if (headerRow.values[0].StringValue == "id"
					&& headerRow.values[1].StringValue == "url"
					&& headerRow.values[2].StringValue == "GUARD") {
				return true;
			}
			else {
				Debug.LogError("[GoogleSheetsImporter] Invalid image format encountered in "
				               + sheet.properties.title
				               + " sheet");
				return false;
			}
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
