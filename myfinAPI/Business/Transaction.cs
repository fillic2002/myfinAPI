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
			IList<EquityTransaction> transactions = ComponentFactory.GetMySqlObject().GetTransaction(0).ToArray();
			int Year = DateTime.Now.Year;

			while (Year > 2015)
			{
				AssetHistory yearlyHistory = new AssetHistory();
				var shareTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetType == (int)AssetType.Shares);
				AddHistory(shareTranList, AssetType.Shares, Year, result);
				var debtTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetType == (int)AssetType.Debt_MF);
				AddHistory(debtTranList, AssetType.Debt_MF, Year, result);
				var eqtyTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetType == (int)AssetType.Equity_MF);
				AddHistory(eqtyTranList, AssetType.Equity_MF, Year, result);
				//var pfTranList = transactions.Where(x => x.tranDate.Year == Year && x.assetType == (int)AssetType.PF);
				//AddHistory(pfTranList, yearlyHistory, AssetType.Shares, Year, result);

				Year--;
			}			  
			return result;
		}
		public IList<AssetHistory> GetMonthlyInvestment()
		{
			IList<AssetHistory> result = new List<AssetHistory>();
			var transactions = ComponentFactory.GetMySqlObject().GetTransaction(0).ToArray();
			foreach (EquityTransaction tran in transactions)
			{
				AssetHistory monthlyInvestment = result.FirstOrDefault(x => x.year == tran.tranDate.Year && x.month == tran.tranDate.Month
				&& x.Assettype == tran.assetType);
				if (monthlyInvestment == null)
				{
					result.Add(new AssetHistory()
					{
						Assettype = tran.assetType,
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
			yearlyHistory.Assettype = (int)assetType;
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

	}
}
