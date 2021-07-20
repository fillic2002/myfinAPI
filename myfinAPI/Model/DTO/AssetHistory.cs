using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class AssetHistory
	{
		public int portfolioId { get; set; }
		public double AssetValue { get; set; }
		public double Dividend { get; set; }
		public double Investment { get; set; }
		public int qtr { get; set; }
		public int year { get; set; }
		public int qty { get; set; }
		public int Assettype { get; set; }

	}
}
