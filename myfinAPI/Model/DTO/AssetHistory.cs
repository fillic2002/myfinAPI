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
		public double AssetValue { get; set; }
		public double Dividend { get; set; }
		public double Investment { get; set; }
		public int month { get; set; }
		public int year { get; set; }
		public double qty { get; set; }
		public AssetType Assettype { get; set; }
		public double profitCurrentyear { get; set; }

	}
}
