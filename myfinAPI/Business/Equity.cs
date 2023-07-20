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
	public class Equity
	{
		public bool AddEqtyTransaction(EquityTransaction tran)
		{
			//ComponentFactory.GetWebScrapperObject().GetEqtDetail(tran);
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
		public IList<dividend> GetDividend(string eqtName)
		{
			IList<dividend> eqtDetails = new List<dividend>();
			IList<dividend> eqtNewDetails = new List<dividend>();
			ComponentFactory.GetMySqlObject().GetYrlyDividend(eqtName, eqtDetails);
			if (eqtDetails.Count > 0)
			{
				for (int yr = eqtDetails[0].dt.Year; yr <= DateTime.Now.Year; yr++)
				{
					var item = eqtDetails.Where(x => x.dt.Year == yr).ToList();
					if (item.Count > 0)
					{
						eqtNewDetails.Add(item[0]);
					}
					else
					{
						eqtNewDetails.Add(new dividend()
						{
							dt = new DateTime(yr, 1, 1),
							//eqt = new EquityBase { assetId = eqtName },
							divValue = 0
						});
					}
				}
			}
			return eqtNewDetails;
		}

		public IList<portfolio> GetCompanyWiseDiv(int year)
		{

			IList<dividend> eqtDivDetails = new List<dividend>();
			//IList<EquityBase> eqtDetails = new List<EquityBase>();
			List<portfolio> finalFolio = new List<portfolio>();
			//IList<BondTransaction> bondTran = new List<BondTransaction>();

			ComponentFactory.GetPortfolioObject().GetFolio(0, finalFolio, year);
			ComponentFactory.GetMySqlObject().GetCompanyWiseYearyDividend(eqtDivDetails, year);

			//foreach (portfolio p in finalFolio)
			//{
			//	p.dividend = 0;
			//	dividend di = p.ToList().Find(x => x.eqt.assetId == p.eq.assetId);
			//	if (di != null)
			//	{
			//		p.dividend += di.divValue;
			//	}
			//}
			//return finalFolio.Where(x => x.dividend > 0).ToList();
			return finalFolio;
		}
	}
}
