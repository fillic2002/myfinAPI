using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class BankDetail
	{
		public int acctId { get; set; }
		public double amt { get; set; }
		public double roi { get; set; }
		public DateTime transactionDate { get; set; }
		public string acctName { get; set; }
		public string acctType { get; set; }
		public int userid { get; set; }

	}

	public class TotalBankAsset
	{
		 
		public double amt { get; set; }
		 
		 
	}
}
