using System;

namespace GoogleSheets {
	
	[Serializable]
	public struct GridData {
		
		public int startRow;
		public int startColumn;
		public RowData[] rowData;
		
	}
	
}
