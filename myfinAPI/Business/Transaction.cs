using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Business
{
	public class Transaction
	{
		private Dictionary<int, double> yearlyIntrestOnBonds;
		 
		public IList<AssetHistory> GetYearlyInvestment(int folioid)
		{
			yearlyIntrestOnBonds = new Dictionary<int, double>();
			IList<AssetHistory> result = new List<AssetHistory>();
			IList<EquityTransaction> transactions = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().GetAllTransaction(folioid, transactions);
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioid, transactions);
			ComponentFactory.GetBankObject().GetYearlyPFTransaction(folioid, AssetType.PPF, transactions);
			ComponentFactory.GetBondhelperObj().GetYearWiseIntrest(yearlyIntrestOnBonds,folioid);
			int Year = DateTime.Now.Year;

			while (Year > 2015)
			{
				AssetHistory yearlyHistory = new AssetHistory();
				var shareTranList = transactions.Where(x => x.tranDate.Year == Year && x.equity.assetType== AssetType.Shares);
				GetHistoricAssetValue(shareTranList, AssetType.Shares, Year, result);
				var debtTranList = transactions.Where(x => x.tranDate.Year == Year && x.equity.assetType == AssetType.Debt_MF);
				GetHistoricAssetValue(debtTranList, AssetType.Debt_MF, Year, result);
				var eqtyTranList = transactions.Where(x => x.tranDate.Year == Year && x.equity.assetType == AssetType.Equity_MF);
				GetHistoricAssetValue(eqtyTranList, AssetType.Equity_MF, Year, result);
				var bondsTranList = transactions.Where(x => x.tranDate.Year == Year && x.equity.assetType == AssetType.Bonds);
				GetHistoricAssetValue(bondsTranList, AssetType.Bonds, Year, result);
				var pfTranList = transactions.Where(x => x.tranDate.Year == Year && x.equity.assetType == AssetType.PF);
				GetHistoricAssetValue(pfTranList, AssetType.PF, Year, result);
				var ppfTranList = transactions.Where(x => x.tranDate.Year == Year && x.equity.assetType == AssetType.PPF);
				GetHistoricAssetValue(ppfTranList, AssetType.PPF, Year, result);

				Year--;
			}			  
			return result;
		}
		public IList<AssetHistory> GetMonthlyInvestment(int folioID)
		{
			IList<AssetHistory> result = new List<AssetHistory>();
			IList<EquityTransaction> transactions = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().GetAllTransaction(folioID, transactions);

			foreach (EquityTransaction tran in transactions)
			{
				AssetHistory monthlyInvestment = result.FirstOrDefault(x => x.year == tran.tranDate.Year && x.month == tran.tranDate.Month
				&& x.Assettype == tran.equity.assetType);
				if (monthlyInvestment == null)
				{
					result.Add(new AssetHistory()
					{
						Assettype = tran.equity.assetType,
						Investment = tran.qty * tran.price,
						year = tran.tranDate.Year,
						month = tran.tranDate.Month
					});
				}
				else if (tran.tranType == TranType.Buy)
				{
					monthlyInvestment.Investment += tran.qty * tran.price;
				}
				else if (tran.tranType == TranType.Sell)
				{
					monthlyInvestment.Investment -= tran.qty * tran.price;
				}
			}
			return result;
		}

		public IList<EquityTransaction> GetYearlyInvestment(int folioId, string eqtId)
		{
			IList<EquityTransaction> yearlyTrans = new List<EquityTransaction>();
			IList<EquityTransaction> yrNewEqtInvst = new List<EquityTransaction>();
			
			ComponentFactory.GetMySqlObject().GetYearlyInvstPerEqt(folioId, eqtId, yearlyTrans);

			for (int year = yearlyTrans[0].tranDate.Year; year <= DateTime.Now.Year; year++)
			{
				IList<EquityTransaction> listofTran = yearlyTrans.Where(x => x.tranDate.Year == year).ToList();
				if (listofTran.Count == 0)
				{
					yrNewEqtInvst.Add(new EquityTransaction()
					{
						tranDate = new DateTime(year, 1, 1),
						equity = new EquityBase() { assetId = eqtId}						
					});
				}
				else
				{
					EquityTransaction yrTran = new EquityTransaction();
					foreach (EquityTransaction t in listofTran)
					{
						if(t.tranType==TranType.Buy ||t.tranType==TranType.Bonus)
						{
							yrTran.qty += t.qty;
							yrTran.tranDate = t.tranDate;
						}
						else
						{
							yrTran.qty -= t.qty;
							yrTran.tranDate = t.tranDate;
						}							
					};
					yrNewEqtInvst.Add(yrTran);
				}
			}
			return yrNewEqtInvst;
		}
		private void GetHistoricAssetValue(IEnumerable<EquityTransaction> eqtTranList,
			AssetType assetType, int year, IList<AssetHistory> result)
		{
			double pAstValue = 0;
			double cAstValue = 0;
			double cInvstValue = 0;
			double pInvstValue = 0;
			IList<dividend> div = new List<dividend>();

			AssetHistory yearlyHistory = new AssetHistory();
			ComponentFactory.GetMySqlObject().GetAssetWiseNetDividend(div, assetType);
			yearlyHistory.Assettype = assetType;
			yearlyHistory.year = year;
			 
			IList<AssetHistory> currentYearSnapshots = ComponentFactory.GetSnapshotObj().GetYearlySnapShot(assetType);
			IList<AssetHistory> previousYearSnapshots = currentYearSnapshots;
			if (year== DateTime.Now.Year)
			{
				//yearlyHistory.profitCurrentyear = currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == DateTime.Now.Month).AssetValue - previousYearSnapshots.First<AssetHistory>(x => x.year == year - 1 && x.month == 12).AssetValue;
				cAstValue = currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == DateTime.Now.Month).AssetValue+
					currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == DateTime.Now.Month).Dividend;
				pAstValue = previousYearSnapshots.First<AssetHistory>(x => x.year == year-1 && x.month ==12).AssetValue+ 
					previousYearSnapshots.First<AssetHistory>(x => x.year == year - 1 && x.month == 12).Dividend;
				cInvstValue = currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == DateTime.Now.Month).Investment;
				pInvstValue = previousYearSnapshots.First<AssetHistory>(x => x.year == year-1 && x.month == 12).Investment;				
			}
			else
			{
				try
				{
					cAstValue = currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == 12).AssetValue+ currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == 12).Dividend;
					pAstValue = previousYearSnapshots.First<AssetHistory>(x => x.year == year - 1 && x.month == 12).AssetValue+ previousYearSnapshots.First<AssetHistory>(x => x.year == year - 1 && x.month == 12).Dividend;
					cInvstValue = currentYearSnapshots.First<AssetHistory>(x => x.year == year && x.month == 12).Investment;
					pInvstValue = previousYearSnapshots.First<AssetHistory>(x => x.year == year - 1 && x.month == 12).Investment;
					 
				}
				catch(Exception ex)
				{
					string msg = ex.Message;
				}				
				//yearlyHistory.profitCurrentyear = cAstValue - pAstValue;
			}
			yearlyHistory.Investment = cInvstValue - pInvstValue;
			yearlyHistory.AssetValue= cAstValue- pAstValue;
			double netDiv = 0;
			var element = div.ToList().Find(x => x.dt.Year == year);
			if (element != null)
			{
				netDiv = div.First(x => x.dt.Year == year).divValue;
			}

			yearlyHistory.profitCurrentyear = (cAstValue - pAstValue) - (cInvstValue - pInvstValue) + netDiv;
						
			//if (assetType == AssetType.Bonds)
			//{
			//	double Intrest;
			//	yearlyIntrestOnBonds.TryGetValue(year, out Intrest);
			//	yearlyHistory.profitCurrentyear += Intrest;
			//}
			result.Add(yearlyHistory);
		}

		public bool AddPFTransaction(BankTransaction tran)
		{
			
			
			if (tran.Description == "PPF-Int" || tran.Description == "PPF-Deposit")
			{
				 
				 
				PFAccount pfobj = new PFAccount()
				{
					Month = tran.tranDate.Month,
					Year = tran.tranDate.Year,
					DateOfTransaction = tran.tranDate,
					Folioid = tran.folioId,
					InvestmentEmp = tran.Amt,
					InvestmentEmplr = 0,
					Pension = 0,
					TypeOfTransaction = tran.Description.Contains("Int")==true? TranType.Intrest:TranType.Deposit,
					AccountType=tran.AcctId
				};
				return ComponentFactory.GetMySqlObject().PostPF_PPFTransaction(pfobj);
			}
			else
				return ComponentFactory.GetMySqlObject().PostBankTransaction(tran);
		}

		//public bool AddEqtyTransaction(EquityTransaction tran)
		//{
		//	//ComponentFactory.GetWebScrapperObject().GetEqtDetail(tran);
		//	return ComponentFactory.GetMySqlObject().PostEquityTransaction(tran);
		//}

		public IList<EquityTransaction> GetTransaction(int folioId, string EqtId)
		{
			IList <EquityTransaction> tranDetails = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().GetTransaction(folioId, EqtId, tranDetails);
			return tranDetails;
		}

		//public void GetAllTransaction(int folioId, IList<EquityTransaction> tranDetails)
		//{	
		//	ComponentFactory.GetMySqlObject().GetAllTransaction(folioId, tranDetails);
		//}

		//public void GetBondTransaction(int folioId, IList<EquityTransaction> t)
		//{
		//	IList<BondTransaction> bt = new List<BondTransaction>();
		//	ComponentFactory.GetBondDataObj().GetBondTransaction(folioId, bt);
		//	foreach (BondTransaction b in bt)
		//	{
		//		t.Add(new EquityTransaction() { 
		//			tranType = b.TranType,
		//			equity= new EquityBase() { assetType= AssetType.Bonds },
		//			//assetTypeId	= AssetType.Bonds,
		//			price = b.InvstPrice,
		//			qty = b.Qty,
		//			tranDate = b.purchaseDate
		//		});
		//	}
		//}
		//public bool AddBondDetails(Bond bondDetails)
		//{
		//	if (bondDetails.symbol == null)
		//		bondDetails.symbol = bondDetails.BondName;
		//	bondDetails.YTM = 0;
		//	return ComponentFactory.GetAdminObj().AddBondDetails(bondDetails);
		//}
				 
	}
}

