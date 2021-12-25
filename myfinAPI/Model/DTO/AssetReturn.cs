using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class AssetReturn
	{
		public int PortfolioId { get; set; }
		public double Return { get; set; }
		public int year { get; set; }
	}
}
