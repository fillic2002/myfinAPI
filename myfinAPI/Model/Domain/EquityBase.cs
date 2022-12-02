using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class EquityBase
	{
		public string equityName { get; set; }
		public string assetId { get; set; }
		public string symbol { get; set; }
		public double livePrice { get; set; }
		//public TranType tranType { get; set; }
		public string sector { get; set; }
		public double PB { get; set; }
		public double MarketCap{ get; set; }
		public UInt64 freefloat { get; set; }
		public DateTime lastUpdated { get; set; }
		public string sourceurl { get; set; }
		public string divUrl { get; set; }
		public AssetType assetType { get; set; }
	}
	public class AssetBase
	{
		public int AssetClass;

	}

}
