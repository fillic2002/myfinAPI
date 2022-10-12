using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Business
{
	public class Transaction
	{
		public IList<AssetHistory> GetYearlyInvestment()
		{
			IList<AssetHistory> result = new List<AssetHistory>();
			IList<EquityTransaction> transactions = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().GetAllTransaction(0,transactions);
			int Year = DateTime.Now.Year;

			while (Year > 2015)
			{
				AssetHistory yearlyHistory = new AssetHistory();
				var shareTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetTypeId == AssetType.Shares);
				AddHistory(shareTranList, AssetType.Shares, Year, result);
				var debtTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetTypeId == AssetType.Debt_MF);
				AddHistory(debtTranList, AssetType.Debt_MF, Year, result);
				var eqtyTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetTypeId == AssetType.Equity_MF);
				AddHistory(eqtyTranList, AssetType.Equity_MF, Year, result);
			
				Year--;
			}			  
			return result;
		}
		public IList<AssetHistory> GetMonthlyInvestment()
		{
			IList<AssetHistory> result = new List<AssetHistory>();
			IList<EquityTransaction> transactions = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().GetAllTransaction(0, transactions);

			foreach (EquityTransaction tran in transactions)
			{
				AssetHistory monthlyInvestment = result.FirstOrDefault(x => x.year == tran.tranDate.Year && x.month == tran.tranDate.Month
				&& x.Assettype == tran.assetTypeId);
				if (monthlyInvestment == null)
				{
					result.Add(new AssetHistory()
					{
						Assettype = tran.assetTypeId,
						Investment = tran.qty * tran.price,
						year = tran.tranDate.Year,
						month = tran.tranDate.Month
					});
				}
				else if (tran.tranType == "B")
				{
					monthlyInvestment.Investment += tran.qty * tran.price;
				}
				else if (tran.tranType == "S")
				{
					monthlyInvestment.Investment -= tran.qty * tran.price;
				}
			}
			return result;
		}

		public IList<EquityTransaction> GetYearlyInvestment(int folioId, string eqtId)
		{
			IList<EquityTransaction> yrEqtInvst = new List<EquityTransaction>();
			IList<EquityTransaction> yrNewEqtInvst = new List<EquityTransaction>();
			
			ComponentFactory.GetMySqlObject().GetYearlyInvstPerEqt(folioId, eqtId, yrEqtInvst);

			for (int year = yrEqtInvst[0].tranDate.Year; year <= DateTime.Now.Year; year++)
			{
				IList<EquityTransaction> listofTran = yrEqtInvst.Where(x => x.tranDate.Year == year).ToList();
				if (listofTran.Count == 0)
				{
					yrNewEqtInvst.Add(new EquityTransaction()
					{
						tranDate = new DateTime(year, 1, 1),
						equityId = eqtId,
					});
				}
				else
				{
					EquityTransaction yrTran = new EquityTransaction();
					foreach (EquityTransaction t in listofTran)
					{
						if(t.tranType=="B")
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
		private void AddHistory(IEnumerable<EquityTransaction> shareTranList,
			AssetType assetType, int year, IList<AssetHistory> result)
		{
			AssetHistory yearlyHistory = new AssetHistory();
			yearlyHistory.Assettype = assetType;
			yearlyHistory.year = year;
			foreach (EquityTransaction t in shareTranList)
			{
				if (t.tranType == "B")
				{
					yearlyHistory.Investment += t.qty * t.price;
				}
				else if (t.tranType == "S")
				{
					yearlyHistory.Investment -= t.qty * t.price;
				}
			}
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
					TypeOfTransaction = tran.Description.Split('-')[1].ToLower(),
					AccountType=tran.AcctId
				};
				return ComponentFactory.GetMySqlObject().PostPF_PPFTransaction(pfobj);
			}
			else
				return ComponentFactory.GetMySqlObject().PostBankTransaction(tran);
		}

		public bool AddEqtyTransaction(EquityTransaction tran)
		{
			//ComponentFactory.GetWebScrapperObject().GetEqtDetail(tran);
			return ComponentFactory.GetMySqlObject().PostEquityTransaction(tran);
		}

		public IList<EquityTransaction> GetTransaction(int folioId, string EqtId)
		{
			IList <EquityTransaction> tranDetails = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().GetTransaction(folioId, EqtId, tranDetails);
			return tranDetails;
		}

		public void GetAllTransaction(int folioId, IList<EquityTransaction> tranDetails)
		{	
			ComponentFactory.GetMySqlObject().GetAllTransaction(folioId, tranDetails);
		}

		public void GetBondTransaction(int folioId, IList<EquityTransaction> t)
		{		 
			ComponentFactory.GetMySqlObject().GetBondTransaction(folioId, t);			
		}
		public bool AddBondDetails(Bond bondDetails)
		{
			return ComponentFactory.GetAdminObj().AddBondDetails(bondDetails);
		}


	}
}

