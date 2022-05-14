using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class PFAccount :IComparable<PFAccount>
	{
		public int Folioid { get; set; }
		public int Year { get; set; }
		public int Month { get; set; }
		public double Balance { get; set; }
		public string TypeOfTransaction { get; set; }
		public double InvestmentEmp { get; set; }
		public double InvestmentEmplr { get; set; }
		public double Pension { get; set; }
		public DateTime DateOfTransaction{ get; set; }
		public int AccountType { get; set; }

		public int CompareTo([AllowNull] PFAccount other)
		{
			if (other == null)
			{
				return 1;
			}

			return Comparer<int>.Default.Compare(this.Year, other.Year);
		}
	}
	//public class PFComparer : IComparable<PFAccount>
	//{
	//	public int CompareTo(PFAccount x, PFAccount y)
	//	{
	//		if (object.ReferenceEquals(x, y))
	//		{
	//			return 0;
	//		}

	//		if (x == null)
	//		{
	//			return -1;
	//		}

	//		if (y == null)
	//		{
	//			return 1;
	//		}

	//		return x.Year.CompareTo(y.Year);
	//	}

	//	public int CompareTo([AllowNull] PFAccount other)
	//	{
	//		throw new NotImplementedException();
	//	}
	//}
}
