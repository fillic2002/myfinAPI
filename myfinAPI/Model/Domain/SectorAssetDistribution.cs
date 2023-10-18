using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.Domain
{
	public class SectorAssetDistribution  
	{
		public int Id { get; set; }
		public string SectorName { get; set; }
		public decimal Invested { get; set; }
		public decimal CurrentValue { get; set; }
		public decimal Dividend { get; set; }
		//public double CompareTo([AllowNull] SectorAssetDistribution other)
		//{
		//	if (other == null)
		//	{
		//		return 1;
		//	}

		//	return Comparer<double>.Default.Compare(this.CurrentValue, other.CurrentValue);
		//}

		//int IComparable<SectorAssetDistribution>.CompareTo(SectorAssetDistribution other)
		//{
		//	if (other == null)
		//	{
		//		return 1;
		//	}

		//	return Comparer<double>.Default.Compare(this.CurrentValue, other.CurrentValue);
		//	//throw new NotImplementedException();
		//}

		//int IComparable<SectorAssetDistribution>.CompareTo(SectorAssetDistribution other)
		//{
		//	if (other == null)
		//	{
		//		return 1;
		//	}

		//	return Comparer<double>.Default.Compare(this.CurrentValue, other.CurrentValue); ;
		//}
	}
}
