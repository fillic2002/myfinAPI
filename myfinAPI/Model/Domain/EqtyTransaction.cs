using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class EquityTransaction
	{
		public DateTime tranDate { get; set; }
		public double qty { get; set; }
		public EquityBase equity { get; set; }
		public double price { get; set; }
		public int portfolioId { get; set; }
		public TranType tranType { get; set; }
		//public AssetType assetTypeId { get; set; }
		public double ownership { get; set; }
		public double coupon { get; set; }
		public double PB_Tran { get; set; }
		public double MarketCap_Tran { get; set; }
		public bool verified { get; set; }

	}

}
