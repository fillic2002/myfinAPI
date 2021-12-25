using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;

namespace myfinAPI.Business
{
	public class Dashboard
	{
		public IList<AssetHistory> GetAllAssetHistory(int userid)
		{
			IList<AssetHistory> astHistory = new List<AssetHistory>();
			astHistory=ComponentFactory.GetMySqlObject().GetAssetSnapshot();
			//ComponentFactory.GetMySqlObject().GetPropertyHistoricalValue(astHistory);
			//var bankdetails = ComponentFactory.GetMySqlObject().GetBankAssetDetails().ToArray();

			return astHistory.Where(x=>x.year>=2012).ToArray();
			//foreach (TotalBankAsset asset in bankdetails)
			//{
			//	dashBoard.Add(new DashboardDetail()
			//	{
			//		AssetName = asset.actType,
			//		Invested = asset.totalAmt,
			//		CurrentValue = asset.totalAmt


			//	}); ;
			//}
			//return dashBoard.ToArray();
		}

	}
}
