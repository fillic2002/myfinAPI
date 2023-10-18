using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model.DTO
{
	public class CashflowDTO
	{
		public int portfolioId { get; set; }		 
		public int month { get; set; }
		public int year { get; set; }
		public IList<AssetClassFlow> flow { get; set; }
	}

	public class AssetClassFlow
	{
		public AssetType Assettype { get; set; }
		public decimal Cashflow { get; set; }
		public decimal Dividend { get; set; }
	}
}
