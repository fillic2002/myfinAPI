using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using myfinAPI.Model.DTO;

namespace myfinAPI.Business
{
	public class Banking
	{
		public IEnumerable<AcctType> GetAcctType(int folioId)
		{
			return ComponentFactory.GetMySqlObject().GetBankActType().ToArray();
			
		}
		public IEnumerable<PFAccount> GetPFYearWiseDetails(int folioid,int type)
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
						TypeOfTransaction =  detail[0].TypeOfTransaction=="int"?"deposit":"int"
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
						TypeOfTransaction = "deposit" 
					});
					pfDetails.Add(new PFAccount()
					{
						Folioid = folioid,
						InvestmentEmp = 0,
						InvestmentEmplr = 0,
						Pension = 0,
						Year = year,
						TypeOfTransaction = "int"
					});
				}

				year++;
			}
			//double PreviousBalance = 0;
			//foreach(PFAccount act in pfDetails)
			//{
			//	act.Balance = PreviousBalance + act.InvestmentEmp + act.InvestmentEmplr + act.Pension;
			//	PreviousBalance = act.Balance;
			//}
			pfDetails.Sort();			 
			return pfDetails;
		}

		public void GetSalaryAndRental(int pastmonths, IList<CashFlow> cashFlow)
		{
			
			  ComponentFactory.GetMySqlObject().GetSalaryAndRental(pastmonths, cashFlow);			
		}
		public void GetDividend(int pastmonths, IList<CashFlow> div)
		{
			 
			  ComponentFactory.GetMySqlObject().GetNetDividend(pastmonths,  div);
		}
		public void GetRental(int month, int year)
		{

		}		
	}

	 

}
