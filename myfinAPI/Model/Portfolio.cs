using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class portfolio : Ibasefolio
	{
		public int trasactionId { get; set; }
		public double avgprice { get; set; }
		public double qty { get; set; }
		public double livePrice { get; set; }
		public string EquityId {get; set;}
		public string EquityName { get; set; }

	}
	public class Ibasefolio
	{
		public string folioName { get; set; }
		public int folioID { get; set; }
	}
}
