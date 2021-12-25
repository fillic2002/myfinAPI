using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.DTO;
 

namespace myfinAPI.Business
{
	public class Portfolio
	{
		public IList<CashFlow> GetCashFlowStm(int folioid,int months)
		{
			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetAssetSnapshot(0,3);
			IList<CashFlow> cashFlow = new List<CashFlow>();
			IList<CashFlow> cashFlowMonths = new List<CashFlow>();
			int j=0; int k = 0; int l = 0;

			 
			IList<double> monthlyDiv = new List<double>();
			IList<double> monthlyCashflow = new List<double>();
			 
			IList<string> timeLine = new List<string>();
			IList<string> timeLineMF = new List<string>();
			IList<AssetHistory> asstHstr = asstHistory.Where(x => x.portfolioId == folioid).ToList();
			GetCashFlowForPortfolio(asstHstr, cashFlow);
			//GetCashFlowForPortfolio(asstHistory.Where(x => x.portfolioId == 1).ToList(), cashFlow);


			for (int i=cashFlow.Count; i > 0; i--)
			{
				switch (cashFlow[i - 1].Assettype)
				{
					case 1:
						if (j < months)
						{
							cashFlowMonths.Add(cashFlow[i - 1]);
							j++;
						}
						break;
					case 2:
						if (k < months)
						{
							cashFlowMonths.Add(cashFlow[i - 1]);
							k++;
						}
						break;
					case 5:
						if (l < months)
						{
							cashFlowMonths.Add(cashFlow[i - 1]);
							l++;
						}
						break;
				}
				if(j==k && k==l && j==months)
				{
					break;
				}
				 
			}
			return cashFlowMonths;			
		}
		public IList<AssetReturn> GetAssetReturn(int assetId)
		{
			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetAssetSnapshotByYear(assetId);
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			double startYrAsset = 0;
			double startYrInvst = 0;
			double netCurr = 0;
			double netInvt = 0;
			foreach (AssetHistory asset in asstHistory)
			{
				if (startYrAsset == 0 || asset.qtr==1)
				{
					startYrAsset = asset.AssetValue;
					startYrInvst = asset.Investment;
				}
				else if(asset.qtr==12 || (asset.qtr==DateTime.Now.Month && asset.year==DateTime.Now.Year))
				{
					astReturn.Add(new AssetReturn()
					{
						year = asset.year,
						Return = ((asset.AssetValue - startYrAsset) - (asset.Investment - startYrInvst)) * 100 / (startYrAsset + (asset.Investment - startYrInvst)),
					});
					startYrAsset = asset.AssetValue;
					startYrInvst = asset.Investment;
				}				
			}
			
			return astReturn;
		}
		public IList<AssetReturn> GetAssetReturn(int folioid, int assetId)
		{
			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetYearlySnapShot(folioid, assetId, false);
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			if (asstHistory.Count == 0)
				return astReturn;
			double startYrAsset = 0;
			double startYrInvst = 0;
			double netCurr;
			double netInvt;
			foreach (AssetHistory asset in asstHistory)
			{
				if (startYrAsset == 0 || asset.qtr == 1)
				{
					startYrAsset = asset.AssetValue;
					startYrInvst = asset.Investment;
				}
				else if (asset.qtr == 12 || (asset.qtr == DateTime.Now.Month && asset.year == DateTime.Now.Year))
				{
					astReturn.Add(new AssetReturn()
					{
						PortfolioId= folioid,
						year = asset.year,
						Return = ((asset.AssetValue - startYrAsset) - (asset.Investment - startYrInvst)) * 100 / (startYrAsset ),
					});
					startYrAsset = asset.AssetValue;
					startYrInvst = asset.Investment;
				}
			}
			//IList<AssetHistory> asstHistoryNew = ComponentFactory.GetMySqlObject().GetYearlySnapShot(folioid, assetId, true);
			// netCurr = asstHistoryNew[0].AssetValue - previousYrValue;
			// netInvt = asstHistoryNew[0].Investment - previousYrInvst;

			//astReturn.Add(new AssetReturn()
			//{
			//	year = asstHistoryNew[0].year,				
			//	Return = ((netCurr - netInvt) * 100) / (previousYrValue + netInvt),
			//	PortfolioId = folioid
			//});
			
			return astReturn;

		}
		private void GetCashFlowForPortfolio(IList<AssetHistory> asstHstry, IList<CashFlow> cashFlow)
		{
			double netInvestAdded = 0, prevMonthInvst = 0, cumMonthlyAsst = 0, preMonthAsst = 0, preMonthlyAsstMF = 0, preMonthInvstMFD = 0;
			double currentMonthCashflow = 0, preMonthDivCum = 0, preMonthInvstMF = 0, currentMonthCashflowMF = 0, currentMonthCashflowMFD = 0;
			double preMonthlyAsstMFD = 0;

			foreach (AssetHistory item in asstHstry)
			{
				if (item.Assettype == 1)
				{
					currentMonthCashflow = (item.AssetValue - preMonthAsst) - (item.Investment - prevMonthInvst);
					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = currentMonthCashflow,
							qtr = item.qtr,
							year = item.year,
							Dividend = item.Dividend - preMonthDivCum,
							Assettype = item.Assettype
						});

					preMonthDivCum = item.Dividend;
					cumMonthlyAsst = item.AssetValue;
					prevMonthInvst = item.Investment;
					preMonthAsst = cumMonthlyAsst;
				}
				else if (item.Assettype == 2)
				{
					netInvestAdded = item.Investment - preMonthInvstMF;
					currentMonthCashflowMF = item.AssetValue - preMonthlyAsstMF - netInvestAdded;

					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = currentMonthCashflowMF,
							qtr = item.qtr,
							year = item.year,
							Dividend = 0,
							Assettype = item.Assettype
						});

					preMonthlyAsstMF = item.AssetValue;
					preMonthInvstMF = item.Investment;
				}
				else if (item.Assettype == 5)
				{
					netInvestAdded = item.Investment - preMonthInvstMFD;
					currentMonthCashflowMFD = (item.AssetValue - preMonthlyAsstMFD) - netInvestAdded;
					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = currentMonthCashflowMFD,
							qtr = item.qtr,
							year = item.year,
							Dividend = 0,
							Assettype = item.Assettype
						});
					preMonthlyAsstMFD = item.AssetValue;
					preMonthInvstMFD = item.Investment;
				}
			}
		}
	}
}

