using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Model;

namespace myfinAPI.Controller
{
	[Route("[controller]")]
	[ApiController]
	public class TransactionController: ControllerBase
	{
		[HttpGet]
		public ActionResult<IEnumerable<EquityTransaction>> GetPortfolio()
		{
			mysqlContext obj = new mysqlContext();
			return obj.getTransaction().ToArray();
		}
		[HttpPost]
		public ActionResult<bool> PostPortfolio(EquityTransaction tran)
		{
			mysqlContext obj = new mysqlContext();
			return obj.postTransaction(tran);
		}
	}
}

