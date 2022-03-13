using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using myfinAPI.Model.DTO;
using static myfinAPI.Business.Xirr;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Business
{
	public class Portfolio
	{
		public IList<CashflowDTO> GetCashFlowStm(int folioid,int months)
		{
			IList<AssetHistory> asstHistory = new List<AssetHistory>();			 

			ComponentFactory.GetMySqlObject().GetAssetSnapshot(asstHistory, folioid,(int)AssetType.Shares);
			ComponentFactory.GetMySqlObject().GetAssetSnapshot(asstHistory, folioid, (int)AssetType.Equity_MF);
			ComponentFactory.GetMySqlObject().GetAssetSnapshot(asstHistory, folioid, (int)AssetType.PF);
			ComponentFactory.GetMySqlObject().GetAssetSnapshot(asstHistory, folioid, (int)AssetType.PPF);
			ComponentFactory.GetMySqlObject().GetAssetSnapshot(asstHistory, folioid, (int)AssetType.Debt_MF);
			GetCashFlow(folioid,months,AssetType.Bank, asstHistory);	         //  6		

			IList<CashFlow> cashFlow = new List<CashFlow>();
			IList<CashflowDTO> cashFlowMonths = new List<CashflowDTO>();

			//IList<AssetHistory> asstHstr = asstHistory.Where(x => x.portfolioId == folioid).ToList();
			GetCashFlowForPortfolio(asstHistory, cashFlow);

			for (int i=0;i< months; i++)
			{
				CashflowDTO currentmonthflow=new CashflowDTO()
				{
					portfolioId = folioid,
					month = DateTime.Now.AddMonths(-i).Month,
					year = DateTime.Now.AddMonths(-i).Year,
					flow = new List<AssetClassFlow>()				
				};
				IList<CashFlow> monthlyCashFlow = cashFlow.Where(x => x.month == DateTime.Now.AddMonths(-i).Month && x.year == DateTime.Now.AddMonths(-i).Year).ToList();
				foreach (CashFlow cf in monthlyCashFlow)
				{
					currentmonthflow.flow.Add(new AssetClassFlow()
					{
						Assettype = cf.Assettype,
						Cashflow = cf.Cashflow,
						Dividend= cf.Dividend
					});
				}
				cashFlowMonths.Add(currentmonthflow);
			}
			int j=0; int k = 0; int l = 0;
			 
			IList<double> monthlyDiv = new List<double>();
			IList<double> monthlyCashflow = new List<double>();
			 
			IList<string> timeLine = new List<string>();
			IList<string> timeLineMF = new List<string>();			

			 
			return cashFlowMonths;			
		}
		/// <summary>
		/// Cashflow From Salary+Dividend+Any sale of shares - Net investment made during current month
		/// </summary>
		/// <param name="folioid"></param>
		/// <param name="months"></param>
		/// <returns></returns>
		public IList<CashflowDTO> GetCashFlowOutStm(int folioid, int months)
		{
			IList<AssetHistory> asstHistory = new List<AssetHistory>();			 
 			IList<CashFlow> cashFlow = new List<CashFlow>();
			IList<CashflowDTO> cashFlowMonths = new List<CashflowDTO>();
			double lastMonthBankBalance,currentMonthBankBalance;
			ComponentFactory.GetBankObject().GetSalaryAndRental(months, cashFlow);
			IList<dividend> div = new List<dividend>();
			ComponentFactory.GetBankObject().GetDividend(months, cashFlow);
			asstHistory=ComponentFactory.GetMySqlObject().GetAssetSnapshot();

			IList<AssetHistory> astHistory = ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(0, (int)AssetType.Bank);

			for (int i = 0; i < months; i++)
			{
				lastMonthBankBalance = astHistory.First<AssetHistory>(x=>x.month== DateTime.Now.AddMonths(-i-1).Month && x.year== DateTime.Now.AddMonths(-i-1).Year).AssetValue;
				currentMonthBankBalance= astHistory.First<AssetHistory>(x => x.month == DateTime.Now.AddMonths(-i).Month && x.year == DateTime.Now.AddMonths(-i).Year).AssetValue;
				CashflowDTO currentmonthflow = new CashflowDTO()
				{
					portfolioId = folioid,
					month = DateTime.Now.AddMonths(-i).Month,
					year = DateTime.Now.AddMonths(-i).Year,
					flow = new List<AssetClassFlow>()
				};
				IList<CashFlow> monthlyCashFlow = cashFlow.Where(x => x.month == DateTime.Now.AddMonths(-i).Month && x.year == DateTime.Now.AddMonths(-i).Year).ToList();
				double inVstmnt = asstHistory.First(x => x.year == DateTime.Now.AddMonths(-i).Year && x.month == DateTime.Now.AddMonths(-i).Month).Investment -
					asstHistory.First(x => x.year == DateTime.Now.AddMonths(-i - 1).Year && x.month == DateTime.Now.AddMonths(-i - 1).Month).Investment;
				
				foreach (CashFlow cf in monthlyCashFlow)
				{
					currentmonthflow.flow.Add(new AssetClassFlow()
					{
						Assettype = cf.Assettype,
						Cashflow = cf.Cashflow-inVstmnt+lastMonthBankBalance-currentMonthBankBalance,
						Dividend = cf.Dividend
					});
				}
				cashFlowMonths.Add(currentmonthflow);
			}		 

			return cashFlowMonths;
		}

		private double GetCurrentMonthInvestMent(IList<AssetHistory> asstHistory,int month,int year,AssetType astType)
		{
			DateTime t = new DateTime(year, month,1);
			var s=asstHistory.Where(y=>y.Assettype == (int)astType).ToList();
			if (s.Count==0 )
				return 0;
			return  asstHistory.First(x=>x.month==month && x.year==year && x.Assettype==(int)astType).Investment - 
				asstHistory.First(x => x.month == t.AddMonths(-1).Month && x.year == t.AddMonths(-1).Year && x.Assettype == (int)astType).Investment;
		}
		private void GetCashFlow(int folioId, int months, AssetType type, IList<AssetHistory> asstHistory)
		{
			IList<EquityTransaction> tranDetails = ComponentFactory.GetMySqlObject().GetBankCashFlow(folioId,months,type);
			if (tranDetails.Count == 0)
				return;
			for(int i=0;i<months;i++)
			{
				int year = DateTime.Now.AddMonths(-i).Year;
				int month = DateTime.Now.AddMonths(-i).Month;
				var monthTran = tranDetails.Where(x => x.tranDate.Year == year && x.tranDate.Month == month);
				double cashflow = 0;
				foreach(EquityTransaction tran in monthTran)
				{
					if(tran.description=="Deposit")
					{
						cashflow += tran.price;
					}
				}
				asstHistory.Add(new AssetHistory()
				{
					Assettype = (int)type,
					month = month,
					year = year,
					portfolioId = folioId,
					AssetValue = cashflow					
				});
			}
		}
		public IList<AssetReturn> GetAssetReturn(int assetId)
		{

		//Xirr.RunScenario(new[]
		//	{
  //              // this scenario fails with Newton's but succeeds with slower Bisection
  //              new CashItem(new DateTime(2012, 1, 1), -10),
		//		new CashItem(new DateTime(2012, 2, 1), -10),
		//		new CashItem(new DateTime(2012, 3, 1), -10),
		//		new CashItem(new DateTime(2012, 4, 1), -10),
		//		new CashItem(new DateTime(2012, 5, 1), -10),
		//		new CashItem(new DateTime(2012, 12, 1), 60),

		//	});

			 

			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetYearlySnapshot(assetId);
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			double startYrAsset = 0;
			double startYrInvst = 0;
			
			foreach (AssetHistory asset in asstHistory)
			{
				if (startYrAsset == 0 || asset.month==1)
				{
					startYrAsset = asset.AssetValue;
					startYrInvst = asset.Investment;
				}
				else if(asset.month==12 || (asset.month==DateTime.Now.Month && asset.year==DateTime.Now.Year))
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
			if (folioid == 0)
				return GetAssetReturn(assetId);
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			//IList<CashItem> invstYear = new List<CashItem>();
			//double InvstDiff = 0;

			IList<EquityTransaction> t= ComponentFactory.GetMySqlObject().getTransaction(folioid);
			t = t.Where(x => x.assetType == assetId).ToList();
			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetYearlySnapShot(folioid, assetId, false);
			
			int year = 2017;
			while(year <= DateTime.Now.Year)
			{
				IList<CashItem> invstYear = new List<CashItem>();
				IList<EquityTransaction> yearTransaction = t.Where(x => x.tranDate.Year == year).ToList();
				if (yearTransaction.Count==0 )
				{
					year++;
					continue;
				}
				IList<AssetHistory> yearSnapshot = asstHistory.Where(y => y.year == year).ToList();
				foreach(EquityTransaction eqtT in yearTransaction)
				{
					if(eqtT.tranType=="B")
						invstYear.Add(new CashItem() { Date = eqtT.tranDate, Amount = eqtT.price * eqtT.qty });
					else
						invstYear.Add(new CashItem() { Date = eqtT.tranDate, Amount = -eqtT.price * eqtT.qty });
				}
				if (yearSnapshot.Count == 1)
				{
					if (yearSnapshot[0].month == 12)
					{
						invstYear.Add(new CashItem()
						{
							Date = new DateTime(year, 12, 31),
							Amount = -yearSnapshot[0].AssetValue
						});
					}
					else
					{
						year++;
						continue;
					}						
				}
				else
				{
					//InvstDiff = yearSnapshot[0].AssetValue - yearSnapshot[1].AssetValue;
					invstYear.Add(new CashItem()
					{
						Date = new DateTime(yearSnapshot[0].year, yearSnapshot[0].month,1),
						Amount = yearSnapshot[0].AssetValue
					});
					invstYear.Add(new CashItem()
					{
						Date = new DateTime(yearSnapshot[1].year, yearSnapshot[1].month, 28),
						Amount = -yearSnapshot[1].AssetValue
					});
				}

				//invstYear.Add(new CashItem() { Date = new DateTime(year,12,31), Amount= InvstDiff });
				var astreturn = Xirr.RunScenario(invstYear);				
				astReturn.Add(new AssetReturn() { year=year,Return=astreturn*100,PortfolioId=folioid});
				year++;
			}

			//IList<AssetReturn> astReturn = new List<AssetReturn>();
			if (asstHistory.Count == 0)
				return astReturn;
			double startYrAsset = 0;
			double startYrInvst = 0;
			
			//foreach (AssetHistory asset in asstHistory)
			//{
			//	if (startYrAsset == 0 || asset.month == 1)
			//	{
			//		startYrAsset = asset.AssetValue;
			//		startYrInvst = asset.Investment;
			//	}
			//	else if (asset.month == 12 || (asset.month == DateTime.Now.Month && asset.year == DateTime.Now.Year))
			//	{
					//astReturn.Add(new AssetReturn()
					//{
					//	PortfolioId = folioid,
					//	year = asset.year,
					//	Return = ((asset.AssetValue - startYrAsset) - (asset.Investment - startYrInvst)) * 100 / (startYrAsset),
					//});
			//		startYrAsset = asset.AssetValue;
			//		startYrInvst = asset.Investment;
			//	}
			//}
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
				if (item.Assettype == (int)AssetType.Shares || item.Assettype == (int)AssetType.PF ||
					item.Assettype == (int)AssetType.PPF)
				{
					currentMonthCashflow = (item.AssetValue - preMonthAsst) - (item.Investment - prevMonthInvst);
					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = currentMonthCashflow,
							month = item.month,						
							year = item.year,
							Dividend = item.Assettype==(int)AssetType.Shares? item.Dividend - preMonthDivCum:0,
							Assettype = item.Assettype
						});

					preMonthDivCum = item.Dividend;
					cumMonthlyAsst = item.AssetValue;
					prevMonthInvst = item.Investment;
					preMonthAsst = cumMonthlyAsst;
				}
				else if (item.Assettype == (int)AssetType.Equity_MF ||  item.Assettype == (int)AssetType.Plot || 
					item.Assettype == (int)AssetType.Flat || item.Assettype == (int)AssetType.Gold || 
					item.Assettype == (int)AssetType.Debt_MF  )
				{
					netInvestAdded = item.Investment - preMonthInvstMF;
					currentMonthCashflowMF = item.AssetValue - preMonthlyAsstMF - netInvestAdded;

					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = currentMonthCashflowMF,
							month = item.month,
							year = item.year,
							Dividend = 0,
							Assettype = item.Assettype
						});

					preMonthlyAsstMF = item.AssetValue;
					preMonthInvstMF = item.Investment;
				}
				else if (item.Assettype == (int)AssetType.Bank)
				{
					//netInvestAdded = item.Investment - preMonthInvstMFD;
					//currentMonthCashflowMFD = (item.AssetValue - preMonthlyAsstMFD) - netInvestAdded;
					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = item.AssetValue,
							month = item.month,
							year = item.year,
							Dividend = 0,
							Assettype = item.Assettype
						});
					//preMonthlyAsstMFD = item.AssetValue;
					//preMonthInvstMFD = item.Investment;
				}				
			}
		}

		public IList<AssetHistory> GetAssetHistory(int folioid, int assettype)
		{
			IList<AssetHistory> astHistory = new List<AssetHistory>();
			ComponentFactory.GetMySqlObject().GetAssetSnapshot(astHistory,folioid, assettype);
			return astHistory;
		}

		public IList<portfolio> GetFolio(int portfolioId, List<portfolio> finalFolio)
		{
			//List<portfolio> finalFolio = new List<portfolio>();
			IList<EquityTransaction> tranDetails = new List<EquityTransaction>();

			ComponentFactory.GetMySqlObject().getTransactionDetails(portfolioId, tranDetails);

			foreach (EquityTransaction eq in tranDetails)
			{
				int indx = finalFolio.FindIndex(x => x.EquityName == eq.equityName);
				if (indx >= 0)
				{
					if (eq.tranType == "S")
					{
						finalFolio[indx].qty = finalFolio[indx].qty - eq.qty;
						finalFolio[indx].avgprice -= eq.price * eq.qty;
					}
					else
					{
						finalFolio[indx].qty = finalFolio[indx].qty + eq.qty;
						finalFolio[indx].avgprice += eq.price * eq.qty;
					}
				}
				else
				{
					//add
					finalFolio.Add(new portfolio()
					{
						EquityName = eq.equityName,
						qty = eq.qty,
						avgprice = eq.price * eq.qty,
						EquityId = eq.equityId,
						symobl = eq.symbol,
						equityType = eq.assetType,
						livePrice = eq.livePrice,
						trandate = eq.tranDate,
						sector = eq.sector
					});
				}
			}
			int inde = finalFolio.FindIndex(x => x.qty == 0);
			if (inde > 0)
			{
				finalFolio.RemoveAt(inde);
			}

			finalFolio.ForEach(
				 n => {
					 if (n.qty >= 1)
					 {
						 n.avgprice = n.avgprice / n.qty;
						 n.dividend = CalculateDividend(n.EquityId, tranDetails);
					 }
				 });

			return finalFolio;
		}
		public double CalculateDividend(string companyId, IList<EquityTransaction> t)
		{
			IList<dividend> divDetails = new List<dividend>();
			ComponentFactory.GetMySqlObject().GetDividend(companyId, divDetails);

			double dividend = 0;
			foreach (dividend div in divDetails)
			{
				double q = 0;
				foreach (EquityTransaction tran in t.Where(x => x.equityId == div.companyid && x.tranDate < div.dt))
				{
					if (tran.tranType == "B")
						q += tran.qty;
					else
						q -= tran.qty;
				}

				if (q > 0)
				{
					dividend += q * div.value;
					//if(p.folioId==2)
					//	Console.WriteLine("EQUITY:"+div.companyid +" Dividend:"+ equities[div.companyid]);
				}
			}
			return dividend;
		}

	}
}

