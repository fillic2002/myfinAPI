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
			IList<AssetHistory> asstHistory = ComponentFactory.GetMySqlObject().GetAssetSnapshot(folioid,0);
			IList<CashFlow> cashFlow = new List<CashFlow>();
			IList<CashFlow> cashFlowMonths = new List<CashFlow>();
			int j=0; int k = 0; int l = 0;

			double netInvestAdded=0, prevMonthInvst=0,  cumMonthlyAsst=0, preMonthlyAsstMF=0, preMonthInvstMFD=0;
			double currentMonthCashflow = 0, cumDiv = 0, preMonthInvstMF=0, currentMonthCashflowMF=0, currentMonthCashflowMFD=0;
			double preMonthlyAsstMFD = 0;
			IList<double> monthlyDiv = new List<double>();
			IList<double> monthlyCashflow = new List<double>();
			 
			IList<string> timeLine = new List<string>();
			IList<string> timeLineMF = new List<string>();
	 
			foreach (AssetHistory item in asstHistory)
			{
				if(item.Assettype == 1)
				{
					netInvestAdded = item.Investment - prevMonthInvst;
					currentMonthCashflow = item.AssetValue - cumMonthlyAsst - netInvestAdded;

					cashFlow.Add(
						new CashFlow()
						{
							Cashflow = currentMonthCashflow,
							qtr = item.qtr,
							year = item.year,
							Dividend= item.Dividend - cumDiv,
							Assettype = item.Assettype
						});					 

					cumDiv = item.Dividend;
					cumMonthlyAsst = item.AssetValue;
					prevMonthInvst = item.Investment;
				}
				else if(item.Assettype==2)
				{
					netInvestAdded = item.Investment-preMonthInvstMF;
					currentMonthCashflowMF = item.AssetValue-preMonthlyAsstMF-netInvestAdded;

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
				else if (item.Assettype==5)
				{
					netInvestAdded = item.Investment-preMonthInvstMFD;
					currentMonthCashflowMFD = item.AssetValue-preMonthlyAsstMFD-netInvestAdded;					 
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

			
			for(int i=cashFlow.Count; i > 0; i--)
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
	}
}

