using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model.Domain;
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model
{
	public class EquityBase
	{
		public string equityName { get; set; }
		public string assetId { get; set; }
		public string symbol { get; set; }
		public double livePrice { get; set; }
		//public TranType tranType { get; set; }
		public string sector { get; set; }
		public double PB { get; set; }
		public double MarketCap { get; set; }
		public UInt64 freefloat { get; set; }
		public DateTime lastUpdated { get; set; }
		public string sourceurl { get; set; }
		public string divUrl { get; set; }
		public AssetType assetType { get; set; }
		public CompanySize category { get; set; }
		public IList<dividend> div {
			get
			{ 
				return _dividend.Value; 
			}
			set { _dividend = (Lazy<List<dividend>>)value; }
		}

		private Lazy<List<dividend>> _dividend = null;

		public EquityBase()
		{
			_dividend = new Lazy<List<dividend>>(() => ComponentFactory.GetMySqlObject().GetCompanyDividend(assetId).ToList());
		}

	}
	public class AssetBase
	{
		public int AssetClass;

	}

}
