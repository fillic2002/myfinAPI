using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class portfolio
	{
		public int trasactionId { get; set; }
		public string equityname { get; set; }
		public double avgprice { get; set; }
		public int qty { get; set; }

	}
}
