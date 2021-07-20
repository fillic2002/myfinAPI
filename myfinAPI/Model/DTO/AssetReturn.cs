using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class AssetReturn
	{
		public int PortfolioId { get; set; }
		public double MonthReturn { get; set; }
		public DateTime Monthyear { get; set; }
	}
}
