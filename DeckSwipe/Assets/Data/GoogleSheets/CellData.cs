using System;

namespace GoogleSheets {
	
	[Serializable]
	public struct CellData {
		
		public ExtendedValue userEnteredValue;
		public ExtendedValue effectiveValue;
		public string formattedValue;
		public string hyperlink;
		
		public int IntValue {
			get { return (int) effectiveValue.numberValue; }
		}
		
	}
	
}
