using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class DashboardDetail
	{
		public int Id { get; set; }
		public string AssetName { get; set; }
		public double Invested { get; set; }
		public double CurrentValue { get; set; }
		public double XirrReturn { get; set; }

	}
}
