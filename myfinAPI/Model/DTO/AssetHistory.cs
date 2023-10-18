using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class AssetHistory
	{
		public int portfolioId { get; set; }
		public decimal AssetValue { get; set; }
		public decimal Dividend { get; set; }
		public decimal Investment { get; set; }
		public int month { get; set; }
		public int year { get; set; }
		public decimal qty { get; set; }
		public AssetType Assettype { get; set; }
		public decimal profitCurrentyear { get; set; }

	}
}
