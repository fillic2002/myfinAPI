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
			CalculateYTMXirr(bondObj);
			return bondObj;
		}
		public void UpdateBondPrice(IList<Bond> bondDetails)
		{
			foreach(Bond b in bondDetails)
				ComponentFactory.GetBondDataObj().UpdateBondLivePrice(b);
		}
		public void SaveBondDetails(IList<Bond> bondDetails)
		{
			//CalculateYTMXirr(bondDetails);
			ComponentFactory.GetBondDataObj().SaveliveBondDetails(bondDetails);
		}
		public void CalculateBondYTM()
		{
			IList<Bond> bondObj = new List<Bond>();
			ComponentFactory.GetBondDataObj().GetBondDetails(bondObj);
			CalculateYTMXirr(bondObj);
			ComponentFactory.GetBondDataObj().SaveliveBondDetails(bondObj);
		}
		public void CalculateYTMXirr(IList<Bond> bondObj)
		{

			ParallelLoopResult parallelLoopResult = Parallel.ForEach(bondObj, b =>
			{
				try
				{
					IList<CashItem> invstYear = new List<CashItem>();
					DateTime intrestPayDate = b.dateOfMaturity;
					//final facevalue payment return
					invstYear.Add(new CashItem()
					{
						Date = b.dateOfMaturity,
						Amount = b.faceValue
					});
					//final intrest return
					invstYear.Add(new CashItem()
					{
						Date = b.dateOfMaturity,
						Amount = (b.faceValue * b.couponRate) / 100
					});
					//Current payment
					invstYear.Add(new CashItem()
					{
						Date = DateTime.Now,
						Amount = -b.LivePrice
					});
					//intrest return in case any
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
			});
		}
		private DateTime GetIntrestPeriod(DateTime intrestperiod,Bond b)
		{
			DateTime newDate= new DateTime();
			if (b.intrestCycle==null)
				newDate = intrestperiod.AddMonths(-12); //In case yearly payment
			else if(b.intrestCycle=="")
				newDate = intrestperiod.AddMonths(-12); //In case yearly payment
			else if (b.intrestCycle.ToUpper() == "ANNUALLY" || b.intrestCycle.ToUpper() == "YEARLY")
				newDate = intrestperiod.AddMonths(-12); //In case yearly payment
			else if(b.intrestCycle.ToUpper() == "MONTHLY")
				newDate = intrestperiod.AddMonths(-1); //In case monthly payment
			else if (b.intrestCycle.ToUpper() == "Q")
				newDate = intrestperiod.AddMonths(-3); //In case quarterly payment
			else if (b.intrestCycle.ToUpper() == "HALFYEARLY")
				newDate = intrestperiod.AddMonths(-6); //Half yearly payment
			return newDate;
		}
		public void GetBondTransaction(int folioID, IList<BondTransaction> bondTran)
		{
			//IList<BondTransaction> bondTran = new List<BondTransaction>();
			ComponentFactory.GetBondDataObj().GetBondTransaction(folioID, bondTran);
			
			//return bondTran;
		}
		public bool AddBondTransaction(EquityTransaction tran)
		{
			BondTransaction tr = new BondTransaction()
			{
				BondId = tran.equity.assetId,
				purchaseDate = tran.tranDate,
				folioId = tran.portfolioId,
				InvstPrice = tran.price,
				Qty = tran.qty,
				TranType = tran.tranType
			};
			return ComponentFactory.GetBondDataObj().PostBondTransaction(tr);
		}
		public bool AddBondDetails(Bond bondDetails)
		{
			if (bondDetails.symbol == null)
				bondDetails.symbol = bondDetails.BondName;
			bondDetails.YTM = 0;
			return ComponentFactory.GetAdminObj().AddBondDetails(bondDetails);
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
					equity = new EquityBase() { assetType = AssetType.Bonds, assetId= b.BondId, livePrice=b.LivePrice },
					//assetTypeId	= AssetType.Bonds,
					price = b.InvstPrice,
					qty = b.Qty,
					tranDate = b.purchaseDate,
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
				asstHistory = ComponentFactory.GetSnapshotObj().GetYearlySnapshot(assetId);
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
