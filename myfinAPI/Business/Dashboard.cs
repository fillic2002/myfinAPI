using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using static myfinAPI.Business.Xirr;

namespace myfinAPI.Business
{
	public class Dashboard
	{
		IList<CashItem> tranDetails;

		public  Dashboard()
		{
			tranDetails = new List<CashItem>();
		}
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
		public IList<DashboardDetail> GetAssetSnapshot()
		{
			IList<DashboardDetail> dashBoard = new List<DashboardDetail>();
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			IList<AssetHistory> assetReturn = new List<AssetHistory>();
			//IList<CashItem> tranDetails = new List<CashItem>();

			ComponentFactory.GetMySqlObject().getTransactionDetails(0, tranList);

			ComponentFactory.GetMySqlObject().GetCurrentMonthSnapShot(1, assetReturn);
			ComponentFactory.GetMySqlObject().GetCurrentMonthSnapShot(2, assetReturn);
			ComponentFactory.GetMySqlObject().GetCurrentMonthSnapShot(3, assetReturn);
			ComponentFactory.GetMySqlObject().GetCurrentMonthSnapShot(4, assetReturn);
			ComponentFactory.GetMySqlObject().GetCurrentMonthSnapShot(5, assetReturn);

			foreach (AssetHistory asset in assetReturn)
			{
				if (asset.year == DateTime.Now.Year && asset.month == DateTime.Now.Month)
				{
					var AstName = (AssetClass.AssetType)Enum.Parse(typeof(AssetClass.AssetType), asset.Assettype.ToString());
					var AstExist = dashBoard.FirstOrDefault(x => x.AssetName == AstName.ToString());
					if (AstExist != null)
					{
						AstExist.Invested += asset.Investment;
						AstExist.CurrentValue += asset.AssetValue;
					}
					else
					{
						//double xirrReturn = 0;
						//if (asset.Assettype == 1 || asset.Assettype == 2|| asset.Assettype == 5)
						//{
						//	CreateCashItemList(tranList,asset.Assettype);
						//	tranDetails.Add(new CashItem()
						//	{
						//		Amount = 1*asset.AssetValue,
						//		Date = new DateTime(DateTime.Now.Year, 12, 25)
						//	}); 
						//	xirrReturn = Xirr.RunScenario(tranDetails)*100;

						//}
						dashBoard.Add(new DashboardDetail()
						{
							Id= asset.Assettype,
							AssetName = AstName.ToString(),
							Invested = asset.Investment,
							CurrentValue = asset.AssetValue
							 
						});
					}
				}
			}
			foreach (DashboardDetail asset in dashBoard)
			{
				double xirrReturn = 0;

				if (asset.Id ==  1|| asset.Id == 2 || asset.Id == 5)
				{
					CreateCashItemList(tranList, asset.Id);
					//AddDividend(asset.Id, );
					tranDetails.Add(new CashItem()
					{
						Amount = 1 * asset.CurrentValue,
						Date = new DateTime(DateTime.Now.Year, 12, 25)
					});
					xirrReturn = Xirr.RunScenario(tranDetails) * 100;
					asset.XirrReturn = xirrReturn;
				}
			}
			return dashBoard;
		}
		private void CreateCashItemList(IList<EquityTransaction> eqtTranDetails,int astType)
		{
			tranDetails = new List<CashItem>();
			foreach (EquityTransaction eqtT in eqtTranDetails.Where(x=>x.assetType==astType))
			{
				tranDetails.Add(new CashItem()
				{
					Amount =Math.Round(eqtT.price * eqtT.qty*(eqtT.tranType=="B"?-1:1)),
					Date= new DateTime(eqtT.tranDate.Year, eqtT.tranDate.Month, eqtT.tranDate.Day)  
				}) ;				
			}
		}

		private void GetDividentTransaction()
		{

		}

	}
}
