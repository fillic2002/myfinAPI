using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class EquityTransaction:EquityBase
	{
		public DateTime tranDate { get; set; }
		public double qty { get; set; }
		public double price { get; set; }
		public int portfolioId { get; set; }
		public string tranType { get; set; }
		public int assetType { get; set; }
	}
}
