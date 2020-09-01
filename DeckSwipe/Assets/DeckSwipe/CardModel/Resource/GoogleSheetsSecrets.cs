using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DeckSwipe.CardModel.Resource {

	[Serializable]
	public class GoogleSheetsSecrets {

		private static readonly string _secretsDirectory = Application.persistentDataPath + "/secrets";
		private static readonly string _path = _secretsDirectory + "/google_sheets.json";

		public string apiKey;

		private static async Task Create() {
			Directory.CreateDirectory(_secretsDirectory);
			string json = JsonUtility.ToJson(new GoogleSheetsSecrets(), true);
			using (FileStream fileStream = File.Create(_path)) {
				StreamWriter writer = new StreamWriter(fileStream);
				await writer.WriteAsync(json);
				await writer.WriteAsync('\n');
				await writer.FlushAsync();
			}
		}

		public static async Task<GoogleSheetsSecrets> Load() {
			if (!File.Exists(_path)) {
				await Create();
			}

			string json;
			using (FileStream fileStream = File.OpenRead(_path)) {
				StreamReader reader = new StreamReader(fileStream);
				json = await reader.ReadToEndAsync();
			}
			GoogleSheetsSecrets secrets;
			try {
				secrets = JsonUtility.FromJson<GoogleSheetsSecrets>(json);
			}
			catch (ArgumentException e) {
				Debug.LogError("[GoogleSheetsConfig] Bad format in " + _path + ": " + e.Message);
				secrets = new GoogleSheetsSecrets();
			}

			if (secrets.apiKey == null || secrets.apiKey == "") {
				Debug.LogWarning("[GoogleSheetsConfig] No API key provided");
				Debug.Log("[GoogleSheetsConfig] The file " + _path + " has been created. "
						+ "Please edit its content and provide a Google Sheets API key.");
			}

			return secrets;
		}

	}

}
