using System;

namespace GoogleSheets {
	
	[Serializable]
	public struct Spreadsheet {
		
		public string spreadsheetId;
		public Sheet[] sheets;
		public string spreadsheetUrl;
		
	}
	
}
