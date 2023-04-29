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
	public class BondsHelper
	{
		public IEnumerable<Bond> GetBondDetails()
		{
			IList<Bond> bondObj = new List<Bond>();
			ComponentFactory.GetBondDataObj().GetBondDetails(bondObj);
			//CalculateYTMXirr(bondObj);
			return bondObj;
		}		 
		public IList<Bond> SearchBondDetails(Bond b)
		{
			return ComponentFactory.GetBondDataObj().SearchBond(b); 	 
		}
		public void UpdateBondPrice(IList<Bond> bondDetails)
		{
			foreach(Bond b in bondDetails)
				ComponentFactory.GetBondDataObj().UpdateBondLivePrice(b);
		}
		public bool SaveBondDetails(Bond bondDetails)
		{	
			if(bondDetails.updateDate ==null)
			{
				bondDetails.updateDate = DateTime.UtcNow;
			}
			return ComponentFactory.GetBondDataObj().SaveliveBondDetails(bondDetails);
		}
		public bool SaveBondDetails(IList<Bond> bondDetails)
		{
			//CalculateYTMXirr(bondDetails);
			foreach(Bond b in bondDetails)
				 ComponentFactory.GetBondDataObj().SaveliveBondDetails(b);

			return true;
		}
		public void CalculateBondYTM()
		{
			IList<Bond> bondObj = new List<Bond>();
			IList<CashItem> invstYear = new List<CashItem>();
			ComponentFactory.GetBondDataObj().GetBondDetails(bondObj);
			CalculateYTMXirr(bondObj, invstYear);

			//foreach(Bond b in bondObj)
				//ComponentFactory.GetBondDataObj().SaveliveBondDetails(b);
		}		 
		public void CalculateYTMXirr(IList<Bond> bondObj, IList<CashItem> invstYear)
		{
			ParallelLoopResult parallelLoopResult = Parallel.ForEach(bondObj, b =>
			{
				CalculateYTMXirr(b, invstYear);
				//try
				//{				
				//	DateTime intrestPayDate = b.dateOfMaturity;
				//	//final facevalue payment returned
				//	invstYear.Add(new CashItem()
				//	{
				//		Date = b.dateOfMaturity,
				//		Amount = b.faceValue
				//	});					 
				//	//Current payment
				//	invstYear.Add(new CashItem()
				//	{
				//		Date = DateTime.Now,
				//		Amount = -b.LivePrice
				//	});
				//	//intrest payment pending until maturity in case any
				//	intrestPayDate = GetIntrestPeriod(intrestPayDate, b);

				//	while (intrestPayDate > DateTime.Now)
				//	{
				//		invstYear.Add(new CashItem()
				//		{
				//			Date = intrestPayDate,
				//			Amount = (b.faceValue * b.couponRate) / 100
				//		});
				//		intrestPayDate = GetIntrestPeriod(intrestPayDate, b);
				//	}
				//	double s = Xirr.RunScenario(invstYear);
				//	b.YTM = Convert.ToDouble(Xirr.RunScenario(invstYear)) * 100;
				//}
				//catch (Exception ex)
				//{
				//	Console.ForegroundColor = ConsoleColor.Red;
				//	Console.WriteLine("Error::" + ex.Message);
				//	Console.ResetColor();
				//}
			});
		}
		public void CalculateYTMXirr(Bond b, IList<CashItem> invstYear)
		{
			try
			{
				DateTime intrestPayDate = b.dateOfMaturity;
				//final facevalue payment returned
				invstYear.Add(new CashItem()
				{
					Date = b.dateOfMaturity,
					Amount = b.faceValue
				});
				//Current paid price based on live price
				invstYear.Add(new CashItem()
				{
					Date = DateTime.Now,
					Amount = -b.LivePrice
				});
				//intrest payment pending until maturity in case any
				intrestPayDate = GetIntrestPeriod(intrestPayDate, b);

				while (intrestPayDate > DateTime.Now)
				{
					invstYear.Add(new CashItem()
					{
						Date = intrestPayDate,
						Amount = (b.faceValue * b.couponRate) / 100
					});
					intrestPayDate = GetIntrestPeriod(intrestPayDate, b);
				}
				double s = Xirr.RunScenario(invstYear);
				b.YTM = Convert.ToDouble(Xirr.RunScenario(invstYear)) * 100;
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Error::" + ex.Message);
				Console.ResetColor();
			}
		}
		public void GetYearWiseIntrest(Dictionary<int,double> yearWiseIntrest, int folioId)
		{
			IList<AssetHistory> assetSnapshot = new List<AssetHistory>();
			ComponentFactory.GetSnapshotObj().GetMonthlyAssetSnapshot(0,AssetType.Bonds, assetSnapshot);

			foreach (AssetHistory ah in assetSnapshot)
			{				
				if (!yearWiseIntrest.ContainsKey(ah.year))
				{
					yearWiseIntrest.Add(ah.year, ah.Dividend);						
				}
				else
				{
					yearWiseIntrest[ah.year] += ah.Dividend;
				}					 
			}		
		}
		public IList<BondIntrest> GetBondIntrest(int year, int folioId)
		{
			IList<BondTransaction> bondTran = new List<BondTransaction>();
			IList<BondIntrest> bondIntrest = new List<BondIntrest>();
			//year++;  // This would yield future 1 year intrest details as well
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, bondTran);
			foreach (BondTransaction tran in bondTran.Where(x => x.purchaseDate <= new DateTime(year, 12, 31)
			 && x.BondDetail.dateOfMaturity > new DateTime(year, 1, 31)))
			{
				ComponentFactory.GetBondhelperObj().
					getyYearlyIntrest(tran, new DateTime(year, 12, 31), bondIntrest);
			};
			return bondIntrest;

		}
		public IOrderedEnumerable<BondIntrestYearly> GetMonthlyBondIntrest(int year, int folioId)
		{
			IList<BondTransaction> bondTran = new List<BondTransaction>();
			IList<BondIntrest> bondIntrest = new List<BondIntrest>();
			IList<BondIntrestYearly> BondIntrestMonthly = new List<BondIntrestYearly>();
			 
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, bondTran);
			foreach (BondTransaction tran in bondTran.Where(x => x.purchaseDate <= new DateTime(year, 12, 31)
			 && x.BondDetail.dateOfMaturity > new DateTime(year, 1, 1)))
			{
				ComponentFactory.GetBondhelperObj().
					getyYearlyIntrest(tran, new DateTime(year, 12, 31), bondIntrest);
			};
			foreach(BondIntrest intr in bondIntrest.Where(x=>x.intrestPaymentDate.Year==year))
			{
				if (BondIntrestMonthly.Where(x => x.month == intr.intrestPaymentDate.Month).Count() > 0)
				{
					BondIntrestMonthly.First(x => x.month == intr.intrestPaymentDate.Month).Intrest += intr.amt;
				}
				else{
					BondIntrestMonthly.Add(new BondIntrestYearly()
					{
						Intrest = intr.amt,
						month = intr.intrestPaymentDate.Month,
						Year = intr.intrestPaymentDate.Year,
					});
				}
			}
			var m=BondIntrestMonthly.OrderBy(x => x.month);
			return m;
		}
		public void getyYearlyIntrest(BondTransaction bt, DateTime atCurrentDate, IList<BondIntrest> bondIntrest)
		{
			bool flag = false;
			 
			DateTime lastIntrestPayDate = bt.BondDetail.dateOfMaturity;
			if (bt.purchaseDate > atCurrentDate)
				return;

			while (lastIntrestPayDate.Year > atCurrentDate.Year && lastIntrestPayDate.Year > bt.purchaseDate.Year)
			{
				lastIntrestPayDate = GetIntrestPeriod(lastIntrestPayDate, bt.BondDetail);
			}
			do
			{	
				//Add intrest credited during current year
				if (bt.purchaseDate <= lastIntrestPayDate && (lastIntrestPayDate.Year==atCurrentDate.Year))
				{
					bondIntrest.Add(new BondIntrest() {
					amt = (bt.Qty * bt.BondDetail.faceValue * bt.BondDetail.couponRate / 100) / GetFactor(bt.BondDetail),
					intrestPaymentDate = lastIntrestPayDate,
					folioId = bt.folioId,
					BondDetail = new Bond()
					{
						BondId = bt.BondDetail.BondId,
						BondName = bt.BondDetail.BondName,
						dateOfMaturity =bt.BondDetail.dateOfMaturity
					}
				});
					
				}
				lastIntrestPayDate = GetIntrestPeriod(lastIntrestPayDate, bt.BondDetail);
			} while (lastIntrestPayDate.Year == atCurrentDate.Year);
			 
		}
		public IList<BondIntrestYearly> GetBondIntrestYearly(int folioId)
		{
			IList<BondTransaction> bondTran = new List<BondTransaction>();
			IList<BondIntrestYearly> bondIntrestYearly = new List<BondIntrestYearly>();

			int year = 2015;
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, bondTran);
			while (year <= DateTime.Now.Year+1)
			{
				foreach (BondTransaction tran in bondTran.Where(x => x.purchaseDate <= new DateTime(year, 12, 31)))
				{
					IList<BondIntrest> bondIntrest = new List<BondIntrest>();
					ComponentFactory.GetBondhelperObj().
						getyYearlyIntrest(tran, new DateTime(year, 12, 31), bondIntrest);
					double amt = bondIntrest.Where(x => x.intrestPaymentDate.Year == year).Sum(y => y.amt);
					IEnumerable<BondIntrestYearly> yearInts = bondIntrestYearly.Where(x => x.Year == year);
					if (yearInts.Count() > 0)
					{
						yearInts.First().Intrest += amt;
					}
					else
					{
						bondIntrestYearly.Add(new BondIntrestYearly
						{
							Year = year,
							Intrest = amt
						});
					}
				}

				year++;
			}
			return bondIntrestYearly;
		}
		//public void CalculateYTMXirr(Bond b)
		//{

		//	//ParallelLoopResult parallelLoopResult = Parallel.ForEach(bondObj, b =>
		//	//{
		//		try
		//		{
		//			IList<CashItem> invstYear = new List<CashItem>();
		//			DateTime intrestPayDate = b.dateOfMaturity;
		//			//final facevalue payment returned
		//			invstYear.Add(new CashItem()
		//			{
		//				Date = b.dateOfMaturity,
		//				Amount = b.faceValue
		//			});
		//			//Current payment
		//			invstYear.Add(new CashItem()
		//			{
		//				Date = DateTime.Now,
		//				Amount = -b.LivePrice
		//			});
		//			//intrest payment pending until maturity in case any
		//			intrestPayDate = GetIntrestPeriod(intrestPayDate, b);

		//			while (intrestPayDate > DateTime.Now)
		//			{
		//				invstYear.Add(new CashItem()
		//				{
		//					Date = intrestPayDate,
		//					Amount = (b.faceValue * b.couponRate) / 100
		//				});
		//				intrestPayDate = GetIntrestPeriod(intrestPayDate, b);
		//			}
		//			double s = Xirr.RunScenario(invstYear);
		//			b.YTM = Convert.ToDouble(Xirr.RunScenario(invstYear)) * 100;
		//		}
		//		catch (Exception ex)
		//		{
		//			Console.ForegroundColor = ConsoleColor.Red;
		//			Console.WriteLine("Error::" + ex.Message);
		//			Console.ResetColor();
		//		}
		//	//});
		//}
		private DateTime GetIntrestPeriod(DateTime intrestperiod,Bond b)
		{
			DateTime newDate= new DateTime();
			if (b.intrestCycle==null)
				newDate = intrestperiod.AddMonths(-12); //In case yearly payment
			else if(b.intrestCycle=="")
				newDate = intrestperiod.AddMonths(-12); //In case yearly payment
			else if (b.intrestCycle.ToUpper() == "ANNUALLY" || b.intrestCycle.ToUpper() == "YEARLY" || b.intrestCycle.ToUpper() == "ONCE A YEAR")
				newDate = intrestperiod.AddMonths(-12); //In case yearly payment
			else if(b.intrestCycle.ToUpper() == "MONTHLY")
				newDate = intrestperiod.AddMonths(-1); //In case monthly payment
			else if (b.intrestCycle.ToUpper() == "Q"||b.intrestCycle.ToUpper() == "FOUR TIMES A YEAR")
				newDate = intrestperiod.AddMonths(-3); //In case quarterly payment
			else if (b.intrestCycle.ToUpper() == "HALFYEARLY"|| b.intrestCycle.ToUpper() == "TWICE A YEAR")
				newDate = intrestperiod.AddMonths(-6);
			else if (b.intrestCycle.ToUpper() == "ON MATURITY")
				newDate = b.dateOfMaturity;
			return newDate;
		}
		private int GetFactor(Bond b)
		{
			
			if (b.intrestCycle == null)
				return 1; //In case yearly payment
			else if (b.intrestCycle == "")
				return 1; //In case yearly payment
			else if (b.intrestCycle.ToUpper() == "ANNUALLY" || b.intrestCycle.ToUpper() == "YEARLY" || b.intrestCycle.ToUpper() == "ONCE A YEAR")
				return 1; //In case yearly payment
			else if (b.intrestCycle.ToUpper() == "MONTHLY")
				return 12; //In case monthly payment
			else if (b.intrestCycle.ToUpper() == "Q" || b.intrestCycle.ToUpper() == "FOUR TIMES A YEAR")
				return 4; //In case quarterly payment
			else if (b.intrestCycle.ToUpper() == "HALFYEARLY" || b.intrestCycle.ToUpper() == "TWICE A YEAR")
				return 2;
			else if (b.intrestCycle.ToUpper() == "ON MATURITY")
				return 0;
			return 0;
		}
		public void GetBondTransaction(int folioID, IList<BondTransaction> bondTran)
		{	
			ComponentFactory.GetBondDataObj().GetBondTransaction(folioID, bondTran);
			//IList<CashItem> invstYear = new List<CashItem>();
			//foreach (BondTransaction bt in bondTran)
			//{
			//	//bt.AccuredIntrest = new List<BondIntrest>();
			//	//GetAccuredIntrest(bt,DateTime.UtcNow);
			//}
		}
		public void GetBondHoldings(int folioID, IList<BondHolding> bondHld)
		{
			IList<BondTransaction> bondTran = new List<BondTransaction>();
			ComponentFactory.GetBondDataObj().GetBondTransaction(folioID, bondTran);
			//IList<CashItem> invstYear = new List<CashItem>();
			foreach (BondTransaction bt in bondTran.Where(x=>x.BondDetail.dateOfMaturity>= DateTime.Now).ToList())
			{
				if (bondHld.Count > 0)
				{
					BondHolding x = bondHld.ToList().Where(x => x.BondDetail.BondId == bt.BondDetail.BondId && x.folioId == bt.folioId).FirstOrDefault();
					if (x!=null)
					{
						x.Investment += bt.Qty * bt.InvstPrice;						
					}
					else
					{
						bondHld.Add(new BondHolding() { BondDetail = bt.BondDetail, folioId = bt.folioId, Investment = bt.InvstPrice * bt.Qty });
					}	
				}
				else{
					bondHld.Add(new BondHolding() { BondDetail=bt.BondDetail, folioId=bt.folioId, Investment=bt.InvstPrice*bt.Qty });
				}

				
			}
		}
		public bool AddBondTransaction(EquityTransaction tran)
		{
			BondTransaction tr = new BondTransaction()
			{
				BondDetail = new Bond() { BondId = tran.equity.assetId, },				
				purchaseDate = tran.tranDate,
				folioId = tran.portfolioId,
				InvstPrice = tran.price,
				Qty = tran.qty,
				TranType = tran.tranType
			};
			return ComponentFactory.GetBondDataObj().PostBondTransaction(tr);
		}
		public bool DeleteBondTransaction(BondTransaction tran)
		{
			return ComponentFactory.GetBondDataObj().DeleteBondTransaction(tran);
		}
		public bool AddBondDetails(Bond bondDetails)
		{
			if (bondDetails.symbol == null)
				bondDetails.symbol = bondDetails.BondName;
			bondDetails.YTM = 0;
			return ComponentFactory.GetBondDataObj().AddBondDetails(bondDetails);
		}
		public void GetBondTransaction(int folioId, IList<EquityTransaction> t)
		{
			IList<BondTransaction> bt = new List<BondTransaction>();
			ComponentFactory.GetBondDataObj().GetBondTransaction(folioId, bt);
			foreach (BondTransaction b in bt)
			{
				t.Add(new EquityTransaction()
				{
					tranType = b.TranType,
					equity = new EquityBase() { 
						equityName=b.BondDetail.BondName, 
						assetType = AssetType.Bonds, 
						assetId= b.BondDetail.BondId, 
						livePrice=b.BondDetail.LivePrice 
					},
					//assetTypeId	= AssetType.Bonds,
					price = b.InvstPrice,
					qty = b.Qty,
					tranDate = b.purchaseDate,
	
				});
			}
		}
		public void GetBondReturns(int folioid, IList<AssetReturn> astReturn)
		{
			IList<BondTransaction> eqtTran = new List<BondTransaction>();
			Dictionary<int, double> yearWiseReturn = new Dictionary<int, double>();
			ComponentFactory.GetBondhelperObj().GetYearWiseIntrest(yearWiseReturn, folioid);
			 
			foreach (int key in ((Dictionary<int, double>)yearWiseReturn).Keys)
			{			 
				astReturn.Add(new AssetReturn()
				{
					year = key,
					dividend = yearWiseReturn[key]
				});
			}
		}
		private IList<AssetReturn> GetBondReturn(int folioid, AssetType assetId)
		{
			IList<EquityTransaction> t = new List<EquityTransaction>();
			IList<AssetReturn> astReturn = new List<AssetReturn>();
			IList<dividend> yearlyDivDetails = new List<dividend>();
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioid, t);
			t = t.Where(x => x.equity.assetType == assetId).ToList();
			IList<AssetHistory> asstHistory;
			if (folioid > 0)
			{
				asstHistory = ComponentFactory.GetSnapshotObj().GetYearlySnapShot(folioid, assetId, false);
			}
			else
			{
				asstHistory = ComponentFactory.GetSnapshotObj().GetYearlySnapShot(assetId);
			}
			CashItem PreviousYearInvst = new CashItem();
			ComponentFactory.GetMySqlObject().GetYearlyDividend(assetId, yearlyDivDetails, folioid);
			int year = 2017;
			while (year <= DateTime.Now.Year)
			{

				IList<CashItem> invstYear = new List<CashItem>();
				var astreturn = Xirr.RunScenario(invstYear);
			}

			return astReturn;
		}
	}
}
