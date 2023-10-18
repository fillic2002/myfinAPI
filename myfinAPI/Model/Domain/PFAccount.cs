using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class PFAccount :IComparable<PFAccount>
	{
		public int Folioid { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public double Balance { get; set; }
		public TranType TypeOfTransaction { get; set; }
		public decimal InvestmentEmp { get; set; }
		public decimal InvestmentEmplr { get; set; }
		public decimal Pension { get; set; }
		public DateTime DateOfTransaction{ get; set; }
		public AssetType AccountType { get; set; }

		public int CompareTo([AllowNull] PFAccount other)
		{
			if (other == null)
			{
				return 1;
			}

			return Comparer<int>.Default.Compare(this.Year, other.Year);
		}
	}
	
}
