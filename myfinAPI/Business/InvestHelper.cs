using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Business
{
	public class InvestHelper
	{
		private void GetInvestment(IList<EquityTransaction> eqtTran, IList<Investment> invst)
		{
			//IList<Investment> invst = new List<Investment>();
			decimal netInvst = 0;
			foreach (EquityTransaction t in eqtTran)
			{
				Investment v = invst.FirstOrDefault(x => x.eq.assetId == t.equity.assetId);
				if (v != null)
				{
					if (t.tranType == TranType.Buy)
					{
						v.qty += t.qty;
						netInvst += t.qty * t.price;
					}					
					else if (t.tranType == TranType.Sell)
					{
						v.qty -= t.qty;
						netInvst -= t.qty * t.price;
					}
					if(v.qty>0)
						v.avgprice = netInvst / v.qty;
				}
				else
				{
					v= new Investment()
					{
						eq = new EquityBase()
						{
							equityName = t.equity.equityName,
							assetId = t.equity.assetId,
							symbol = t.equity.symbol,
							livePrice = t.equity.livePrice,
							sector = t.equity.sector,
							MarketCap = t.equity.MarketCap,
							PB = t.equity.PB,
							assetType = t.equity.assetType,
							category = ComponentFactory.GetPortfolioObject().GetCategory(t.equity.MarketCap)
						},
						equityType = t.equity.assetType,
						qty =t.qty
						//trandate = eq.tranDate						 
					};
					invst.Add(v);
				}
				ComponentFactory.GetPortfolioObject().GetXirrReturn(eqtTran, v);
			}			
		}
		 
		
		public decimal GetNetAssetQty(DateTime dt, IList<EquityTransaction> eqtTran, string assetId, int folioId)
		{
			decimal qty = 0;

			foreach (EquityTransaction t in eqtTran.OrderBy(x => x.tranDate))
			{
				if (t.tranDate < dt)
				{
					if (t.tranType == TranType.Buy || t.tranType == TranType.Bonus)
						qty += t.qty;
					else if (t.tranType == TranType.Sell)
						qty -= t.qty;
				}
				else
				{
					break;
				}
			}
			return qty;
		}
		public void GetFolioInvestments(int portfolioId, List<Investment> investments, int year)
		{
			IList<EquityTransaction> tranDetails = new List<EquityTransaction>();
			ComponentFactory.GetMySqlObject().getTransactionDetails(portfolioId, tranDetails);
			ComponentFactory.GetBondhelperObj().GetBondTransaction(portfolioId, tranDetails);

			//int counter = 0;
			//TODO: change this loop to uniq equitid's
			foreach (EquityTransaction t in tranDetails.Where(x => x.tranDate.Year <= year))
			{
				var invstm = investments.FirstOrDefault(x => x.eq.equityName == t.equity.equityName);

				if (invstm == null)
				{
					IEnumerable<EquityTransaction> selectedEqtTran = tranDetails.Where(x => x.equity.equityName == t.equity.equityName
					&& x.tranDate.Year <= year);
					GetInvestment(selectedEqtTran.ToList(), investments);					
				}
				 
			}
			//GetBondinFolio(finalFolio, portfolioId);
		}
	}
}
