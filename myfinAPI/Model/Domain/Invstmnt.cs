using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class Invstmnt
	{
		public int AssetId { get; set; }
		public int Year{ get; set; }
		public int Month { get; set; }
		public double Invested { get; set; }
		public double CurrentValue { get; set; }
	}
}
