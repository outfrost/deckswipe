using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DeckSwipe.CardModel.Resource {

	[Serializable]
	public class GoogleSheetsConfig {

		private static readonly string _path = Application.persistentDataPath + "/google_sheets_config.json";

		public string spreadsheetId;

		private static async Task Create() {
			string json = JsonUtility.ToJson(new GoogleSheetsConfig(), true);
			using (FileStream fileStream = File.Create(_path)) {
				StreamWriter writer = new StreamWriter(fileStream);
				await writer.WriteAsync(json);
				await writer.WriteAsync('\n');
				await writer.FlushAsync();
			}
		}

		public static async Task<GoogleSheetsConfig> Load() {
			if (!File.Exists(_path)) {
				await Create();
			}

			string json;
			using (FileStream fileStream = File.OpenRead(_path)) {
				StreamReader reader = new StreamReader(fileStream);
				json = await reader.ReadToEndAsync();
			}
			GoogleSheetsConfig config;
			try {
				config = JsonUtility.FromJson<GoogleSheetsConfig>(json);
			}
			catch (ArgumentException e) {
				Debug.LogError("[GoogleSheetsConfig] Bad format in " + _path + ": " + e.Message);
				config = new GoogleSheetsConfig();
			}

			if (config.spreadsheetId == null || config.spreadsheetId == "") {
				Debug.LogWarning("[GoogleSheetsConfig] No spreadsheet ID provided");
				Debug.Log("[GoogleSheetsConfig] The file " + _path + " has been created. "
						+ "Please edit its content and provide a Google Sheets spreadsheet ID.");
			}

			return config;
		}

	}

}
