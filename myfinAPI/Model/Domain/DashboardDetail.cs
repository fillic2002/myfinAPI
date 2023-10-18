using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class DashboardDetail
	{
		public AssetType Id { get; set; }
		public string AssetName { get; set; }
		public decimal Invested { get; set; }
		public decimal CurrentValue { get; set; }
		public decimal XirrReturn { get; set; }

	}
}
