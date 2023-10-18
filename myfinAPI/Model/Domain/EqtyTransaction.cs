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
		public decimal qty { get; set; }
		public EquityBase equity { get; set; }
		public decimal price { get; set; }
		public int portfolioId { get; set; }
		public TranType tranType { get; set; }
		//public AssetType assetTypeId { get; set; }
		public decimal freefloat_tran { get; set; }
		public decimal Ownership { get; set; }
		public double coupon { get; set; }
		public decimal PB_Tran { get; set; }
		public decimal MarketCap_Tran { get; set; }
		public bool? verified { get; set; }
		public int id { get; set; }
		public Guid tranId { get; set; }

	}

}
