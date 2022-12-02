using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;

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
		}
		private void GetEquityHoldingAtDate(int folioId, DateTime dt)
		{
			IList<EquityTransaction> tranDetails = new List<EquityTransaction>();
			GetAllTransaction(folioId, tranDetails);


		}
	}
}
