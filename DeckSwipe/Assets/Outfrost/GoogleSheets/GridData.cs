using System;

namespace Outfrost.GoogleSheets {
	
	[Serializable]
	public struct GridData {
		
		public int startRow;
		public int startColumn;
		public RowData[] rowData;
		
	}
	
}
