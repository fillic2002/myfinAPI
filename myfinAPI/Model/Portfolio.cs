using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	public class portfolio:basefolio
	{
		public int trasactionId { get; set; }
		public string equityname { get; set; }
		public double avgprice { get; set; }
		public double qty { get; set; }

	}
	public class basefolio
	{
		public string folioName { get; set; }
		public int folioID { get; set; }
	}
}
