using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class Investment : Ibasefolio
	{
		public EquityBase eq { get; set; }		
		public int trasactionId { get; set; }
		public decimal avgprice { get; set; }
		public decimal qty { get; set; }	
		public AssetType equityType { get; set; }
		public decimal dividend{ get; set; }
		//public DateTime trandate { get; set; }	
		public double xirr { get; set; }
		public decimal DivReturnXirr { get; set; }

	}
	public class Ibasefolio
	{
		public string folioName { get; set; }
		public int folioID { get; set; }
		public string Comment{ get; set; }
	}

	public class PortfolioNew
	{
		public List<Investment> invstment { get; set; }
		public string folioID{get; set;}
		public User owner { get; set; }
		public PortfolioNew()
		{
			invstment = new List<Investment>();
		}
		public void AddAsset(Investment invst)
		{
			invstment.Add(invst);
		}
	}

}
