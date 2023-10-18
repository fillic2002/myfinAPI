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
	public class Equity
	{
		public bool AddEqtyTransaction(EquityTransaction tran)
		{
			var eqt=ComponentFactory.GetMySqlObject().GetAssetDetail(tran.equity.assetId);
			tran.freefloat_tran = eqt.freefloat;
			return ComponentFactory.GetMySqlObject().PostEquityTransaction(tran);
		}
		public void GetAllTransaction(int folioId, IList<EquityTransaction> tranDetails)
		{
			ComponentFactory.GetMySqlObject().GetAllTransaction(folioId, tranDetails);
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, tranDetails);
		}
		private void GetEquityHoldingAtDate(int folioId, DateTime dt)
		{
			IList<EquityTransaction> tranDetails = new List<EquityTransaction>();
			GetAllTransaction(folioId, tranDetails);
		}
		public bool VerifyTransaction(EquityTransaction tran)
		{
			if(tran.tranId !=null)
			return ComponentFactory.GetMySqlObject().TransactionVerified(tran);

			return false;
		}
		public EquityBase GetDividend(string eqtName)
		{
			IList<dividend> eqtDetails = new List<dividend>();
			IList<dividend> eqtNewDetails = new List<dividend>();

			EquityBase eq = new EquityBase() { assetId = eqtName };
			
			if (eq.div.Count > 0)
				return eq;
			return eq;

			//ComponentFactory.GetMySqlObject().GetYrlyDividend(eqtName, eqtDetails);
			//if (eqtDetails.Count > 0)
			//{
			//	for (int yr = eqtDetails[0].dt.Year; yr <= DateTime.Now.Year; yr++)
			//	{
			//		var item = eqtDetails.Where(x => x.dt.Year == yr).ToList();
			//		if (item.Count > 0)
			//		{
			//			eqtNewDetails.Add(item[0]);
			//		}
			//		else
			//		{
			//			eqtNewDetails.Add(new dividend()
			//			{
			//				dt = new DateTime(yr, 1, 1),
			//				//eqt = new EquityBase { assetId = eqtName },
			//				divValue = 0
			//			});
			//		}
			//	}
			//}
			//return eqtNewDetails;
		}
		//public decimal GetNetAssetQty(DateTime dt, IList<EquityTransaction> eqtTran, string assetId,int folioId)
		//{
		//	decimal qty = 0;			
		    
		//	foreach (EquityTransaction t in eqtTran.OrderBy(x=>x.tranDate))
		//	{
		//		if (t.tranDate < dt)
		//		{
		//			if(t.tranType == TranType.Buy || t.tranType == TranType.Bonus)
		//				qty += t.qty;
		//			else if (t.tranType == TranType.Sell)
		//				qty -= t.qty;
		//		}
		//		else
		//		{
		//			break;
		//		}					  	 
		//	}
		//	return qty;
		//}
		public IList<Investment> GetCompanyWiseDiv(int year, int folioId)
		{

			IList<dividend> eqtDivDetails = new List<dividend>();
			IList<EquityTransaction> eqtTran = new List<EquityTransaction>();
			List<Investment> invstments = new List<Investment>();
			 

			ComponentFactory.GetInvestHelperObj().GetFolioInvestments(0, invstments, year);
			ComponentFactory.GetMySqlObject().getTransactionDetails(0, eqtTran);
			 
			foreach(Investment invst in invstments.Where(x=>x.equityType == AssetType.Shares))
			{
				decimal divi = 0;
				foreach (dividend d in invst.eq.div.Where(x=>x.dt.Year ==year) )
				{
					if (d.creditType == TranType.FinalDividend || d.creditType == TranType.InterDividend || d.creditType == TranType.SpclDividend)
					{
						divi += Convert.ToDecimal(d.divValue * ComponentFactory.GetInvestHelperObj().GetNetAssetQty(d.dt, eqtTran.Where(x=>x.equity.assetId==invst.eq.assetId).ToList(),invst.eq.assetId,invst.folioID));
						invst.dividend = divi;
						//p.trandate = d.dt;
					}
				}
			}
			 
			return invstments;
		}
	}
}
