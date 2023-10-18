using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model.DTO
{
	public class CashFlow
	{
		public int portfolioId { get; set; }
		public decimal Cashflow { get; set; }
		public decimal Dividend { get;set; }		
		public int month { get; set; }
		public int year { get; set; }		
		public AssetType Assettype { get; set; }
	}
}
