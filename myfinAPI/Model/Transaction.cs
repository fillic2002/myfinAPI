using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class EquityTransaction
	{
		public DateTime tranDate { get; set; }
		public string equityName { get; set; }
		public int equityId { get; set; }
		public int qty { get; set; }
		public double price { get; set; }
		public int portfolioId { get; set; }
	}
}
