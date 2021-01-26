using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Factory;
using myfinAPI.Model;

namespace myfinAPI.Controller
{
	[Route("[controller]")]
	[ApiController]
	public class TransactionController: ControllerBase
	{
		[HttpGet("transaction/{portfolioId}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetPortfolio(int portfolioId)
		{
			return ComponentFactory.GetMySqlObject().getTransaction(portfolioId).ToArray();
			//return obj.getTransaction(portfolioId).ToArray();
		}
		[HttpGet("transaction")]
		public ActionResult<bool> PostPortfolio(EquityTransaction tran)
		{
			mysqlContext obj = new mysqlContext();
			return obj.postTransaction(tran);
		}
	}
}

