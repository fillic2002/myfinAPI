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
		public decimal livePrice { get; set; }
		//public TranType tranType { get; set; }
		public string sector { get; set; }
		public decimal PB { get; set; }
		public decimal MarketCap { get; set; }
		public decimal freefloat { get; set; }
		public DateTime lastUpdated { get; set; }
		public string sourceurl { get; set; }
		public string divUrl { get; set; }
		public AssetType assetType { get; set; }
		public CompanySize category { get; set; }
		public IList<dividend> div {
			get
			{ 
				if(lazyLoadedField==null)
				{
					lazyLoadedField=ComponentFactory.GetMySqlObject().GetCompanyDividend(assetId).ToList();					 
				}
				return lazyLoadedField;
			}
			//set { _dividend = List<dividend>value; }
		}

		//private Lazy<List<dividend>> _dividend = null;
		private IList<dividend> lazyLoadedField;
		public EquityBase()
		{
			//_dividend = new Lazy<List<dividend>>(() => ComponentFactory.GetMySqlObject().GetCompanyDividend(assetId).ToList());
			lazyLoadedField = null;
		}

	}
	public class AssetBase
	{
		public int AssetClass;

	}
	public class EquityComparer : IEqualityComparer<EquityBase>
	{
		public bool Equals(EquityBase x, EquityBase y)
		{
			return x.assetId == y.assetId;
		}

		public int GetHashCode(EquityBase obj)
		{
			return HashCode.Combine(obj.assetId);
		}
	}

}
