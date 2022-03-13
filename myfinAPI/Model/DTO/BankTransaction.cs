using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class BankTransaction
	{
		public string tranDate { get; set; }
		public double Amt { get; set; }
		public int folioId { get; set; }
		public string tranType { get; set; }
		public string Description { get; set; }
		public int AcctId { get; set; }
	}
}
