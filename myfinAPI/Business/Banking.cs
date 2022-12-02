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
	public class Banking
	{
		public IEnumerable<AcctType> GetAcctType(int folioId)
		{
			return ComponentFactory.GetMySqlObject().GetBankActType().ToArray();
			
		}
		public IEnumerable<PFAccount> GetPFYearWiseDetails(int folioid,AssetType type)
		{
			List<PFAccount> pfDetails = new List<PFAccount>();
			ComponentFactory.GetMySqlObject().GetPFYearlyDetails(pfDetails, folioid, type);
			int year = 2005;
			while(year <= DateTime.Now.Year)
			{
				IList<PFAccount> detail=pfDetails.Where(x=>x.Year==year).ToList();
				if (detail.Count() == 1 )
				{
					pfDetails.Add(new PFAccount() { 
						Folioid = folioid, 
						InvestmentEmp = 0, 
						InvestmentEmplr = 0, 
						Pension = 0,
						Year=year,
						TypeOfTransaction =  detail[0].TypeOfTransaction==TranType.Intrest?TranType.Deposit:TranType.Intrest,
						DateOfTransaction =detail[0].DateOfTransaction
					});
				}else if(detail.Count==0)
				{
					pfDetails.Add(new PFAccount()
					{
						Folioid = folioid,
						InvestmentEmp = 0,
						InvestmentEmplr = 0,
						Pension = 0,
						Year = year,
						TypeOfTransaction = TranType.Deposit,
						
					});
					pfDetails.Add(new PFAccount()
					{
						Folioid = folioid,
						InvestmentEmp = 0,
						InvestmentEmplr = 0,
						Pension = 0,
						Year = year,
						TypeOfTransaction = TranType.Intrest,
						 
					});
				}

				year++;
			}
			
			pfDetails.Sort();			 
			return pfDetails;
		}
		public void GetSalaryAndRental(int pastmonths, IList<CashFlow> cashFlow)
		{
			
			  ComponentFactory.GetMySqlObject().GetSalaryAndRental(pastmonths, cashFlow);			
		}
		public void GetDividend(int pastmonths, IList<CashFlow> cashFlow)
		{
			IList<dividend> div = new List<dividend>();
			ComponentFactory.GetMySqlObject().GetNetDividend(cashFlow, div);
		}
		public void GetRental(int month, int year)
		{

		}
		public IList<PFAccount> GetMonthlyDividend(int folioId, int astType,int year)
		{
			IList<PFAccount> pfDetails = new List<PFAccount>();
			ComponentFactory.GetMySqlObject().GetMonthlyPFContribution(folioId,astType,year, pfDetails);
			return pfDetails;
		}
		public IList<BankDetail> GetAcctDetails()
		{
			IList<BankDetail> banAcdetails = new List<BankDetail>();
			ComponentFactory.GetMySqlObject().GetBankAssetDetail(banAcdetails);
			return banAcdetails.Where(x => x.isActive ==true).ToList();
		}
	
		public void GetYearlyPFTransaction(int folioId, AssetType type, IList<EquityTransaction> tran)
		{
			List<PFAccount> pfDetails = new List<PFAccount>();
			ComponentFactory.GetMySqlObject().GetPFYearlyDetails(pfDetails, folioId, type);
			foreach (PFAccount pf in pfDetails)
			{
				EquityTransaction ast = new EquityTransaction()
				{
					tranDate = pf.DateOfTransaction,
					tranType = pf.TypeOfTransaction,
					qty=1, 
					price= pf.InvestmentEmp+pf.InvestmentEmplr+pf.Pension,
					equity = new EquityBase() { assetType = type},

				};
				tran.Add(ast);
			}			
		}
		
	}

	 

}
