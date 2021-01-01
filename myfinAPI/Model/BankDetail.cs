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
	}
}
