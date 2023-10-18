using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using static myfinAPI.Business.Xirr;
using static myfinAPI.Model.AssetClass;

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
			astHistory=ComponentFactory.GetSnapshotObj().GetAssetSnapshot();
			 
			return astHistory.Where(x=>x.year>=2015).ToArray();
			 
		}
		public IList<DashboardDetail> GetAssetSnapshot(int year, int month)
		{
			IList<DashboardDetail> dashBoard = new List<DashboardDetail>();
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			IList<AssetHistory> assetReturn = new List<AssetHistory>();

			ComponentFactory.GetMySqlObject().getTransactionDetails(0, tranList);

			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(1, assetReturn, month,year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(2, assetReturn, month, year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(3, assetReturn, month, year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(4, assetReturn, month, year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(5, assetReturn, month, year);
			
			foreach (AssetHistory asset in assetReturn)
			{
			if (asset.year == year && asset.month == month)
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
					dashBoard.Add(new DashboardDetail()
					{
						Id = asset.Assettype,
						AssetName = AstName.ToString(),
						Invested = asset.Investment,
						CurrentValue = asset.AssetValue
					});
				}
			}
		}
		return dashBoard;
		}
		public IList<DashboardDetail> GetAssetSnapshot()
		{
			IList<DashboardDetail> dashBoard = new List<DashboardDetail>();
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			IList<AssetHistory> assetReturn = new List<AssetHistory>();			

			ComponentFactory.GetMySqlObject().getTransactionDetails(0, tranList);

			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(1, assetReturn, DateTime.Today.Month, DateTime.Today.Year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(2, assetReturn, DateTime.Today.Month, DateTime.Today.Year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(3, assetReturn, DateTime.Today.Month, DateTime.Today.Year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(4, assetReturn, DateTime.Today.Month, DateTime.Today.Year);
			ComponentFactory.GetSnapshotObj().GetMonthSnapShot(5, assetReturn, DateTime.Today.Month, DateTime.Today.Year);

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

				if (asset.Id ==  AssetType.Shares|| asset.Id == AssetType.Equity_MF || asset.Id == AssetType.Debt_MF)
				{
					CreateCashItemList(tranList, asset.Id);
					//AddDividend(asset.Id, );
					tranDetails.Add(new CashItem()
					{
						Amount = 1 * Convert.ToDouble(asset.CurrentValue),
						Date = new DateTime(DateTime.Now.Year, 12, 25)
					});
					xirrReturn = Xirr.RunScenario(tranDetails) * 100;
					asset.XirrReturn =Convert.ToDecimal(xirrReturn);
				}
			}
			return dashBoard;
		}
		private void CreateCashItemList(IList<EquityTransaction> eqtTranDetails,AssetType astType)
		{
			tranDetails = new List<CashItem>();
			foreach (EquityTransaction eqtT in eqtTranDetails.Where(x=>x.equity.assetType==astType))
			{
				tranDetails.Add(new CashItem()
				{
					Amount =Convert.ToDouble(Math.Round(eqtT.price * eqtT.qty*(eqtT.tranType==TranType.Buy?-1:1))),
					Date= new DateTime(eqtT.tranDate.Year, eqtT.tranDate.Month, eqtT.tranDate.Day)  
				}) ;				
			}
		}

		private void GetDividentTransaction()
		{

		}

	}
}
