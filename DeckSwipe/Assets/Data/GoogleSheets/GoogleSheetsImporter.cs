using System.IO;
using System.Net;
using UnityEngine;

namespace GoogleSheets {
	
	public class GoogleSheetsImporter {
		
		public async void FetchCardData() {
			HttpWebRequest request = WebRequest.CreateHttp(
					"https://sheets.googleapis.com/v4/spreadsheets/1olhKo6JFItKpDU9Qd7X4cJgaAlFIChhB-P0rI48gNLs?key=AIzaSyAzWqJRSu7Q3p3EfuwFYdtzQql7ygu1pv4");
			WebResponse response = await request.GetResponseAsync();
			Debug.Log((int)((HttpWebResponse)response).StatusCode + " " + ((HttpWebResponse)response).StatusDescription);
			
			if (!response.ContentType.Contains("application/json")) {
				Debug.LogError("Google sheets API returned unrecognised data format");
			}
			
			Stream responseStream;
			if ((responseStream = response.GetResponseStream()) == null) {
				Debug.LogError("Google sheets API returned empty response");
			}
			else {
				Spreadsheet spreadsheet = JsonUtility.FromJson<Spreadsheet>(
						new StreamReader(responseStream).ReadToEnd());
				Debug.Log(spreadsheet.spreadsheetId);
				Debug.Log(spreadsheet.sheets);
				/*foreach (RowData row in spreadsheet.sheets[0].data[0].rowData) {
					Debug.Log(row.values[0].formattedValue
					          + ", "
					          + row.values[1].formattedValue
					          + ", "
					          + row.values[2].formattedValue);
				}*/
			}
		}
		
	}
	
}
