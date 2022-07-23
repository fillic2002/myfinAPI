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
using ExpType = myfinAPI.Model.DTO.ExpType;

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
			GetCashFlow(folioid, months, AssetType.Bank, asstHistory);       //  6		

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

			IList<AssetHistory> astHistory = new List<AssetHistory>();
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(0, (int)AssetType.Bank,astHistory);

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
					if(tran.tranType=="Deposit" && tran.description!="Salary")
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
			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetYearlySnapshot(assetId);
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			double startYrAsset = 0;
			double startYrInvst = 0;
			
			foreach (AssetHistory asset in asstHistory)
			{
				if (startYrAsset == 0)
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
			 
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			IList<dividend> yearlyDivDetails = new List<dividend>();
			IList<dividend> monthlyDivDetails = new List<dividend>();
			IList<EquityTransaction> t= ComponentFactory.GetMySqlObject().GetTransaction(folioid);
			t = t.Where(x => x.assetType == assetId).ToList();
			IList<AssetHistory> asstHistory;
			if (folioid > 0)
			{
				asstHistory = ComponentFactory.GetMySqlObject().GetYearlySnapShot(folioid, assetId, false);
			}
			else
			{
				asstHistory = ComponentFactory.GetMySqlObject().GetYearlySnapshot(assetId);
			}
			CashItem PreviousYearInvst= new CashItem();
			ComponentFactory.GetMySqlObject().GetYearlyDividend(assetId, yearlyDivDetails, folioid);
			if(assetId ==1)
			{
				ComponentFactory.GetMySqlObject().GetMonthlyDividend(folioid, monthlyDivDetails);
			}
			

			int year = 2017;
			while(year <= DateTime.Now.Year)
			{
				IList<CashItem> invstYear = new List<CashItem>();
				double netReturn = 0;
				IList<EquityTransaction> yearTransaction = t.Where(x => x.tranDate.Year == year).ToList();
				if (yearTransaction.Count==0 && astReturn.Count==0)
				{
					year++;
					continue;
				}
				IList<AssetHistory> yearSnapshot = asstHistory.Where(y => y.year == year).ToList();
				foreach(EquityTransaction eqtT in yearTransaction)
				{
					if (eqtT.tranType == "B")
					{
						invstYear.Add(new CashItem() { Date = eqtT.tranDate, Amount = eqtT.price * eqtT.qty });
						netReturn -= eqtT.price * eqtT.qty;
					}
					else
					{
						invstYear.Add(new CashItem() { Date = eqtT.tranDate, Amount = -eqtT.price * eqtT.qty });
						netReturn += eqtT.price * eqtT.qty;
					}
				}
				if (yearSnapshot.Count == 1)
				{	 
					invstYear.Add(new CashItem()
						{
							Date = new DateTime(year, 12, 31),
							Amount = -yearSnapshot[0].AssetValue							
						});
					netReturn += yearSnapshot[0].AssetValue;
					if (PreviousYearInvst.Amount>0)
					{
						invstYear.Add(PreviousYearInvst);
						netReturn -= PreviousYearInvst.Amount;
					}
					PreviousYearInvst = new CashItem()
					{
						Date = new DateTime(year, 12, 31),
						Amount = yearSnapshot[0].AssetValue
					};				 				
				}
				else
				{				 
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

				foreach (dividend d in monthlyDivDetails.Where(x=>x.dt.Year == year))
				{
					invstYear.Add(new CashItem() {
						Date = d.dt,
						Amount =-d.value
					});
				}
				

				var astreturn = Xirr.RunScenario(invstYear);				
				astReturn.Add(new AssetReturn() { Return=netReturn ,year=year,xirr=astreturn*100,PortfolioId=folioid,dividend=Math.Round(yearlyDivDetails.First(x=>x.dt.Year==year).value,2)});
				year++;
			}		 		 
			
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
							Dividend = item.Assettype==(int)AssetType.Shares? item.Dividend:0,
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
		public bool ReplaceComment(int folioid, string comment)
		{
		
			return ComponentFactory.GetMySqlObject().ReplaceFolioComment(folioid, comment);			
		}
		public portfolio GetFolioComment(int folioid)
		{
			portfolio p = new portfolio()
			{
				Comment = ComponentFactory.GetMySqlObject().GetFolioComment(folioid)
			};
			return p;
		}
		public IList<ExpenseDTO> GetExpense(int folioid)
		{
			IList <ExpenseDTO> Exp = new List<ExpenseDTO>();
			ComponentFactory.GetMySqlObject().GetFolioExpense(folioid, Exp);
			return Exp;
		}
		public IList<MonthlyExpenseDTO> GetMonthlyExpenseHistory(int folioid, int pastMonth)
		{
			IList<MonthlyExpenseDTO> Exp = new List<MonthlyExpenseDTO>();
			ComponentFactory.GetMySqlObject().GetMontlyFolioExpenseHistory(folioid, Exp, pastMonth);
			return Exp;
		}
		public IList<Invstmnt> GetMonthlyInvestment(int folioid, int pastMonth)
		{
			IList<AssetHistory> Exp = new List<AssetHistory>();
			IList<Invstmnt> monthlyInv = new List<Invstmnt>();
			DateTime currentDt = DateTime.UtcNow;
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.Shares, Exp);
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.Equity_MF,Exp);

			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.Debt_MF, Exp);
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.Gold, Exp);

			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.PF, Exp);
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.PPF, Exp);
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.Flat, Exp);
			ComponentFactory.GetMySqlObject().GetMonthlyAssetSnapshot(folioid, (int)AssetType.Plot, Exp);

			double prvMonthShrInv = 0;double prvMonthEqtMFInv = 0; double prvMonthDbtMFInv = 0;
			double prvMonthPFInv = 0; double prvMonthPPFInv = 0; double prvMonthGoldInv = 0;
			var curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.Shares);
			prvMonthShrInv = curMonh.ToList()[0].Investment;

			curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.Equity_MF);
			prvMonthEqtMFInv = curMonh.ToList()[0].Investment;

			curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.Debt_MF);
			prvMonthDbtMFInv = curMonh.ToList()[0].Investment;

			curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.PF);
			prvMonthPFInv = curMonh.ToList()[0].Investment;

			curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.PPF);
			prvMonthPPFInv = curMonh.ToList()[0].Investment;

			currentDt =currentDt.AddMonths(-1);

			bool flag = false;
			while (pastMonth>0)
			{
				curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.Shares);
				monthlyInv.Add(new Invstmnt() { AssetId = (int)AssetType.Shares, Invested = prvMonthShrInv - curMonh.ToList()[0].Investment, Month=currentDt.Month,Year=currentDt.Year });
				prvMonthShrInv = curMonh.ToList()[0].Investment;


				curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.Equity_MF);
				monthlyInv.Add(new Invstmnt() { AssetId = (int)AssetType.Equity_MF, Invested = prvMonthEqtMFInv - curMonh.ToList()[0].Investment, Month = currentDt.Month, Year = currentDt.Year });
				prvMonthEqtMFInv = curMonh.ToList()[0].Investment;

				curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.Debt_MF);
				monthlyInv.Add(new Invstmnt() { AssetId = (int)AssetType.Debt_MF, Invested = prvMonthDbtMFInv - curMonh.ToList()[0].Investment, Month = currentDt.Month, Year = currentDt.Year });
				prvMonthDbtMFInv = curMonh.ToList()[0].Investment;

				curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.PF);
				monthlyInv.Add(new Invstmnt() { AssetId = (int)AssetType.PF, Invested = prvMonthPFInv - curMonh.ToList()[0].Investment, Month = currentDt.Month, Year = currentDt.Year });
				prvMonthPFInv = curMonh.ToList()[0].Investment;

				curMonh = Exp.Where(x => x.month == currentDt.Month && x.year == currentDt.Year && x.Assettype == (int)AssetType.PPF);
				monthlyInv.Add(new Invstmnt() { AssetId = (int)AssetType.PPF, Invested = prvMonthPPFInv - curMonh.ToList()[0].Investment, Month = currentDt.Month, Year = currentDt.Year });
				prvMonthPPFInv = curMonh.ToList()[0].Investment;

				currentDt =currentDt.AddMonths(-1);
				pastMonth--;
			}
  
			return monthlyInv;
		}
		public IList<ExpenseDTO> GetMonthlyExpense(int folioid,string my)
		{
			IList<ExpenseDTO> Exp = new List<ExpenseDTO>();
			ComponentFactory.GetMySqlObject().GetMontlyFolioExpense(folioid, Exp, my);
			return Exp;
		}
		public IList<ExpType> GetExpenseType()
		{
			IList<ExpType> Exp = new List<ExpType>();
			ComponentFactory.GetMySqlObject().GetExpenseType(Exp);
			return Exp;
		}
		public bool AddExpenseType(ExpType t)
		{
			
			return ComponentFactory.GetMySqlObject().AddExpenseType(t);
			
		}
		public bool AddExpense( ExpenseDTO exp)
		{			
			return ComponentFactory.GetMySqlObject().AddExpense( exp);			
		}
		public bool DeleteExpense(int expId)
		{
			return ComponentFactory.GetMySqlObject().DeleteExpense(expId);
		}

		public IList<AssetDistribution> GetAssetClassDistribution(int folioId)
		{
			IList<AssetDistribution> tranDetails = new List<AssetDistribution>();
			List<portfolio> finalFolio = new List<portfolio>();
			GetFolio(folioId, finalFolio);
			return null;
		}

		public IList<SectorAssetDistribution> SectorWiseAssetDistribution(int folioId)
		{
			List<SectorAssetDistribution> tranDetails = new List<SectorAssetDistribution>();
			List<portfolio> finalFolio = new List<portfolio>();
			GetFolio(folioId, finalFolio);
			foreach(portfolio p in finalFolio.Where(x=>x.equityType==1)) //only for shares
			{
				var item = tranDetails.Where(x => x.SectorName == p.eq.sector).ToList();
				if(item != null && item.Count>0)
				{
					item[0].CurrentValue += p.eq.livePrice * p.qty;
					item[0].Invested += p.avgprice * p.qty;
					item[0].Dividend += p.dividend;
				}
				else
				{
					tranDetails.Add(new SectorAssetDistribution()
					{
						CurrentValue = p.eq.livePrice * p.qty,
						SectorName = p.eq.sector,
						Invested = p.qty * p.avgprice,
						Dividend =p.dividend
					});
				}				
			}
			//tranDetails.ToList().Sort();
			tranDetails.Sort(delegate (SectorAssetDistribution x, SectorAssetDistribution y) {
				return x.CurrentValue.CompareTo(y.Invested);
			});
			return tranDetails;
		}
		public void GetFolio(int portfolioId, List<portfolio> finalFolio)
		{			
			IList<EquityTransaction> tranDetails = new List<EquityTransaction>();
			IList<CashItem> invstYear;
			ComponentFactory.GetMySqlObject().getTransactionDetails(portfolioId, tranDetails);
			int counter = 0;
			foreach (EquityTransaction eq in tranDetails)
			{
				var indx=finalFolio.FindIndex(x => x.eq.equityName == eq.equityName);
				IEnumerable<EquityTransaction> selectedEqtTran = tranDetails.Where(x=>x.equityName==eq.equityName);
				double qty = 0, avgprice=0;
				if (indx < 0)
				{
					invstYear = new List<CashItem>();
					foreach (EquityTransaction eqt in selectedEqtTran)
					{
						 
						if (eqt.tranType == "S")
						{
							qty -= eqt.qty;
							avgprice -= eqt.price * eqt.qty;
							invstYear.Add(new CashItem()
							{
								Date = new DateTime(eqt.tranDate.Year, eqt.tranDate.Month, eqt.tranDate.Day),
								Amount = eqt.price * eqt.qty
							});
						}
						else
						{
							qty += eqt.qty;
							avgprice += eqt.price * eqt.qty;
							invstYear.Add(new CashItem()
							{
								Date = new DateTime(eqt.tranDate.Year, eqt.tranDate.Month, eqt.tranDate.Day),
								Amount = -eqt.price * eqt.qty
							});
						}
					}
					//Today's Value
					invstYear.Add(new CashItem()
					{
						Date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day),
						Amount = eq.livePrice * qty
					});
					var astreturn = Xirr.RunScenario(invstYear);
					if (qty > 0)
					{
						finalFolio.Add(new portfolio()
						{ 
							eq=new EquityBase()
							{
								equityName = eq.equityName,
								equityId = eq.equityId,
								symbol = eq.symbol,
								livePrice = eq.livePrice,
								sector = eq.sector,
								MarketCap =eq.MarketCap,
								PB=eq.PB
							},
							qty = qty,
							avgprice = avgprice / qty,
							equityType = eq.assetType,
							trandate = eq.tranDate,
							xirr = astreturn*100,
							dividend = CalculateDividend(eq.equityId, selectedEqtTran.ToList())
						});
					}
					counter++;
				}				 
			}
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
				}
			}
			return dividend;
		}

		public IList<dividend> GetDividend(string eqtName)
		{
			IList<dividend> eqtDetails = new List<dividend>();
			IList<dividend> eqtNewDetails = new List<dividend>();
			ComponentFactory.GetMySqlObject().GetYrlyDividend(eqtName, eqtDetails);
			for(int yr=eqtDetails[0].dt.Year;yr<= DateTime.Now.Year;yr++ )
			{
				var item = eqtDetails.Where(x=>x.dt.Year==yr).ToList();
				if(item.Count > 0)
				{
					eqtNewDetails.Add(item[0]);
				}
				else
				{
					eqtNewDetails.Add(new dividend()
					{
						dt = new DateTime(yr, 1, 1),
						companyid = eqtName,
						value = 0
					});
				}				
			}
			return eqtNewDetails;
		}
	}
}

