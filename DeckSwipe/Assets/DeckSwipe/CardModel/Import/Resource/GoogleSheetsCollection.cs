using System;
﻿using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using DeckSwipe.CardModel.DrawQueue;
using DeckSwipe.Gamestate;
using Outfrost;
using Outfrost.GoogleSheets;
using UnityEngine;

namespace DeckSwipe.CardModel.Import.Resource {

	public class GoogleSheetsCollection {

		private const int _majorFormatVersion = 4;
		private const int _minorFormatVersion = 0;

		private GoogleSheetsConfig config;
		private GoogleSheetsSecrets secrets;

		public async Task<ProtoCollection> Fetch() {
			config = await GoogleSheetsConfig.Load();
			secrets = await GoogleSheetsSecrets.Load();

			// Fetch spreadsheet from Google Sheet API V4
			Debug.Log("[GoogleSheetsCollection] Fetching cards from Google Sheet " + config.spreadsheetId + " ...");
			HttpWebRequest request = WebRequest.CreateHttp(
					"https://sheets.googleapis.com/v4/spreadsheets/"
					+ config.spreadsheetId
					+ "?includeGridData=true&key="
					+ secrets.apiKey);
			HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync();

			if (response.StatusCode != HttpStatusCode.OK) {
				throw new WebException((int)response.StatusCode + " " + response.StatusDescription);
			}

			if (!response.ContentType.Contains("application/json")) {
				throw new WebException("Google Sheets API returned unrecognised data format");
			}

			Stream responseStream;
			if ((responseStream = response.GetResponseStream()) == null) {
				throw new WebException("Google Sheets API returned empty response");
			}

			Spreadsheet spreadsheet = JsonUtility.FromJson<Spreadsheet>(
					new StreamReader(responseStream).ReadToEnd());

			// Parse Metadata sheet
			RowData[] metaRowData = spreadsheet.sheets[0].data[0].rowData;
			Dictionary<string, CellData> metadata = new Dictionary<string, CellData>();
			foreach (RowData row in metaRowData) {
				if (row.values[0].StringValue != null) {
					if (metadata.ContainsKey(row.values[0].StringValue)) {
						Debug.LogWarning("[GoogleSheetsCollection] Duplicate key found in Metadata sheet");
					}
					else {
						metadata.Add(row.values[0].StringValue, row.values[1]);
					}
				}
			}

			// Check sheet format version
			if (!RequireMetadata("majorFormatVersion", metadata)
					|| !RequireMetadata("minorFormatVersion", metadata)) {
				throw new FormatException("Invalid sheet format");
			}
			int sheetMajorVersion = metadata["majorFormatVersion"].IntValue;
			if (sheetMajorVersion != _majorFormatVersion) {
				throw new FormatException(
						"Incompatible sheet format major version (required: "
						+ _majorFormatVersion
						+ ", found: "
						+ sheetMajorVersion
						+ ")");
			}
			int sheetMinorVersion = metadata["minorFormatVersion"].IntValue;
			if (sheetMinorVersion < _minorFormatVersion) {
				throw new FormatException(
						"Incompatible sheet format minor version (required min: "
						+ _minorFormatVersion
						+ ", found: "
						+ sheetMinorVersion
						+ ")");
			}

			// Get sheet indices from metadata
			if (!RequireMetadata("cardSheetIndex", metadata)
					|| !RequireMetadata("specialCardSheetIndex", metadata)
					|| !RequireMetadata("characterSheetIndex", metadata)
					|| !RequireMetadata("imageSheetIndex", metadata)) {
				throw new FormatException("Invalid sheet format");
			}
			int cardSheetIndex = metadata["cardSheetIndex"].IntValue;
			int specialCardSheetIndex = metadata["specialCardSheetIndex"].IntValue;
			int characterSheetIndex = metadata["characterSheetIndex"].IntValue;
			int imageSheetIndex = metadata["imageSheetIndex"].IntValue;

			// Sanity-check sheet formats
			Sheet cardSheet = spreadsheet.sheets[cardSheetIndex];
			if (!CheckCardSheetFormat(cardSheet)) {
				throw new FormatException("Invalid sheet format");
			}
			Sheet specialCardSheet = spreadsheet.sheets[specialCardSheetIndex];
			if (!CheckSpecialCardSheetFormat(specialCardSheet)) {
				throw new FormatException("Invalid sheet format");
			}
			Sheet characterSheet = spreadsheet.sheets[characterSheetIndex];
			if (!CheckCharacterSheetFormat(characterSheet)) {
				throw new FormatException("Invalid sheet format");
			}
			Sheet imageSheet = spreadsheet.sheets[imageSheetIndex];
			if (!CheckImageSheetFormat(imageSheet)) {
				throw new FormatException("Invalid sheet format");
			}

			// Parse Images sheet
			var images = new List<ProtoImage>();
			RowData[] imageRowData = imageSheet.data[0].rowData;
			for (int i = 1; i < imageRowData.Length; i++) {
				int id = imageRowData[i].values[0].IntValue;
				string imageUrl = imageRowData[i].values[1].hyperlink;
				images.Add(new ProtoImage(id, imageUrl));
			}

			// Parse Characters sheet
			var characters = new List<ProtoCharacter>();
			RowData[] characterRowData = characterSheet.data[0].rowData;
			for (int i = 1; i < characterRowData.Length; i++) {
				int id = characterRowData[i].values[0].IntValue;
				string name = characterRowData[i].values[1].GetStringValue("");
				int imageId = characterRowData[i].values[2].IntValue;
				characters.Add(new ProtoCharacter(id, name, imageId));
			}

			// Parse Cards sheet
			var cards = new List<ProtoCard>();
			RowData[] cardRowData = cardSheet.data[0].rowData;
			for (int i = 1; i < cardRowData.Length; i++) {
				int id = cardRowData[i].values[0].IntValue;
				int characterId = cardRowData[i].values[1].IntValue;
				string cardText = cardRowData[i].values[2].GetStringValue("");
				ProtoCard proto = new ProtoCard(
					id,
					characterId,
					cardText);

				proto.leftAction.text = cardRowData[i].values[3].GetStringValue("");
				proto.leftAction.statsModification = new StatsModification(
					cardRowData[i].values[4].IntValue,
					cardRowData[i].values[5].IntValue,
					cardRowData[i].values[6].IntValue,
					cardRowData[i].values[7].IntValue);
				proto.rightAction.text = cardRowData[i].values[8].GetStringValue("");
				proto.rightAction.statsModification = new StatsModification(
					cardRowData[i].values[9].IntValue,
					cardRowData[i].values[10].IntValue,
					cardRowData[i].values[11].IntValue,
					cardRowData[i].values[12].IntValue);

				var cardPrerequisites = JsonUtility.FromJson<JsonArray<ProtoCardPrerequisite>>(
						cardRowData[i].values[13].StringValue);
				var specialCardPrerequisites = JsonUtility.FromJson<JsonArray<ProtoSpecialCardPrerequisite>>(
						cardRowData[i].values[14].StringValue);

				if (cardPrerequisites?.array != null) {
					proto.cardPrerequisites =
							new List<ProtoCardPrerequisite>(cardPrerequisites.array);
				}
				if (specialCardPrerequisites?.array != null) {
					proto.specialCardPrerequisites =
							new List<ProtoSpecialCardPrerequisite>(specialCardPrerequisites.array);
				}

				proto.leftAction.followup = new List<Followup>();
				proto.leftAction.specialFollowup = new List<SpecialFollowup>();
				proto.rightAction.followup = new List<Followup>();
				proto.rightAction.specialFollowup = new List<SpecialFollowup>();

				if (cardRowData[i].values[16].IntValue > 0) {
					if (cardRowData[i].values[15].StringValue == null) {
						proto.leftAction.followup.Add(new Followup(
								cardRowData[i].values[15].IntValue,
								cardRowData[i].values[16].IntValue));
					}
					else {
						proto.leftAction.specialFollowup.Add(new SpecialFollowup(
								cardRowData[i].values[15].StringValue,
								cardRowData[i].values[16].IntValue));
					}
				}
				if (cardRowData[i].values[18].IntValue > 0) {
					if (cardRowData[i].values[17].StringValue == null) {
						proto.rightAction.followup.Add(new Followup(
								cardRowData[i].values[17].IntValue,
								cardRowData[i].values[18].IntValue));
					}
					else {
						proto.rightAction.specialFollowup.Add(new SpecialFollowup(
								cardRowData[i].values[17].StringValue,
								cardRowData[i].values[18].IntValue));
					}
				}

				cards.Add(proto);
			}

			// Parse SpecialCards sheet
			var specialCards = new List<ProtoSpecialCard>();
			RowData[] specialCardRowData = specialCardSheet.data[0].rowData;
			for (int i = 1; i < specialCardRowData.Length; i++) {
				string id = specialCardRowData[i].values[0].StringValue;
				int characterId = specialCardRowData[i].values[1].IntValue;
				string cardText = specialCardRowData[i].values[2].GetStringValue("");
				ProtoSpecialCard proto = new ProtoSpecialCard(
					id,
					characterId,
					cardText);

				proto.leftAction.text = specialCardRowData[i].values[3].GetStringValue("");
				proto.rightAction.text = specialCardRowData[i].values[4].GetStringValue("");

				proto.leftAction.followup = new List<Followup>();
				proto.leftAction.specialFollowup = new List<SpecialFollowup>();
				proto.rightAction.followup = new List<Followup>();
				proto.rightAction.specialFollowup = new List<SpecialFollowup>();

				if (cardRowData[i].values[6].IntValue > 0) {
					if (cardRowData[i].values[5].StringValue == null) {
						proto.leftAction.followup.Add(new Followup(
								cardRowData[i].values[5].IntValue,
								cardRowData[i].values[6].IntValue));
					}
					else {
						proto.leftAction.specialFollowup.Add(new SpecialFollowup(
								cardRowData[i].values[5].StringValue,
								cardRowData[i].values[6].IntValue));
					}
				}
				if (cardRowData[i].values[8].IntValue > 0) {
					if (cardRowData[i].values[7].StringValue == null) {
						proto.rightAction.followup.Add(new Followup(
								cardRowData[i].values[7].IntValue,
								cardRowData[i].values[8].IntValue));
					}
					else {
						proto.rightAction.specialFollowup.Add(new SpecialFollowup(
								cardRowData[i].values[7].StringValue,
								cardRowData[i].values[8].IntValue));
					}
				}

				specialCards.Add(proto);
			}

			Debug.Log("[GoogleSheetsCollection] Loaded " + cards.Count + " cards");
			Debug.Log("[GoogleSheetsCollection] Loaded " + specialCards.Count + " special cards");
			Debug.Log("[GoogleSheetsCollection] Loaded " + characters.Count + " characters");
			Debug.Log("[GoogleSheetsCollection] Loaded " + images.Count + " images");

			return new ProtoCollection(cards, specialCards, characters, images);
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
					&& headerRow.values[15].StringValue == "leftActionFollowupCardId"
					&& headerRow.values[16].StringValue == "leftActionFollowupCardDelay"
					&& headerRow.values[17].StringValue == "rightActionFollowupCardId"
					&& headerRow.values[18].StringValue == "rightActionFollowupCardDelay") {
				return true;
			}
			Debug.LogError("[GoogleSheetsCollection] Invalid card format encountered in "
			               + sheet.properties.title
			               + " sheet");
			return false;
		}

		private static bool CheckSpecialCardSheetFormat(Sheet sheet) {
			RowData headerRow = sheet.data[0].rowData[0];
			if (headerRow.values[0].StringValue == "id"
					&& headerRow.values[1].StringValue == "characterId"
					&& headerRow.values[2].StringValue == "cardText"
					&& headerRow.values[3].StringValue == "leftActionText"
					&& headerRow.values[4].StringValue == "rightActionText"
					&& headerRow.values[5].StringValue == "leftActionFollowupCardId"
					&& headerRow.values[6].StringValue == "leftActionFollowupCardDelay"
					&& headerRow.values[7].StringValue == "rightActionFollowupCardId"
					&& headerRow.values[8].StringValue == "rightActionFollowupCardDelay") {
				return true;
			}
			Debug.LogError("[GoogleSheetsCollection] Invalid special card format encountered in "
			               + sheet.properties.title
			               + " sheet");
			return false;
		}

		private static bool CheckCharacterSheetFormat(Sheet sheet) {
			RowData headerRow = sheet.data[0].rowData[0];
			if (headerRow.values[0].StringValue == "id"
					&& headerRow.values[1].StringValue == "name"
					&& headerRow.values[2].StringValue == "imageId") {
				return true;
			}
			Debug.LogError("[GoogleSheetsCollection] Invalid character format encountered in "
			               + sheet.properties.title
			               + " sheet");
			return false;
		}

		private static bool CheckImageSheetFormat(Sheet sheet) {
			RowData headerRow = sheet.data[0].rowData[0];
			if (headerRow.values[0].StringValue == "id"
					&& headerRow.values[1].StringValue == "url") {
				return true;
			}
			Debug.LogError("[GoogleSheetsCollection] Invalid image format encountered in "
			               + sheet.properties.title
			               + " sheet");
			return false;
		}

		private static bool RequireMetadata(string key, Dictionary<string, CellData> metadata) {
			if (!metadata.ContainsKey(key)) {
				Debug.LogError("[GoogleSheetsCollection] " + key + " not found in Metadata sheet");
				return false;
			}
			return true;
		}

	}

}
