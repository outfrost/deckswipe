using System;

namespace Outfrost.GoogleSheets {
	
	[Serializable]
	public struct Sheet {
		
		public SheetProperties properties;
		public GridData[] data;
		
	}
	
}
