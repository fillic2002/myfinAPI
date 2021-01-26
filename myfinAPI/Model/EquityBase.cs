using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class EquityBase
	{
		public string equityName { get; set; }
		public string equityId { get; set; }
		public string symbol { get; set; }
		public string livePrice { get; set; }
	}
}
