using System;

namespace GoogleSheets {
	
	[Serializable]
	public struct Sheet {
		
		public SheetProperties properties;
		public GridData[] data;
		
	}
	
}
