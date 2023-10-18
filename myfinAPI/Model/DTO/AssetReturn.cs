using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class AssetReturn
	{
		public int PortfolioId { get; set; }
		public decimal Return { get; set; }
		public decimal xirr { get; set; }
		public int year { get; set; }
		public decimal dividend { get; set; }
	}
}
