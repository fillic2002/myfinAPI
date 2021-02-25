using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class EquityBase
	{
		public string equityName { get; set; }
		public string equityId { get; set; }
		public string symbol { get; set; }
		public double livePrice { get; set; }
		public string description { get; set; }
	}
	public class AssetBase
	{
		public int AssetClass;

	}

}
