using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class portfolio : Ibasefolio
	{
		public EquityBase eq { get; set; }		
		public int trasactionId { get; set; }
		public double avgprice { get; set; }
		public double qty { get; set; }	
		public AssetType equityType { get; set; }
		public double dividend{ get; set; }
		public DateTime trandate { get; set; }	
		public double xirr { get; set; }
		public double DivReturnXirr { get; set; }

	}
	public class Ibasefolio
	{
		public string folioName { get; set; }
		public int folioID { get; set; }
		public string Comment{ get; set; }
	}
}
