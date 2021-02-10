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
	public class TransactionController : ControllerBase
	{
		[HttpGet("getfolio/{portfolioId}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetPortfolio(int portfolioId)
		{
			return ComponentFactory.GetMySqlObject().getTransaction(portfolioId).ToArray();
		}
		[HttpGet("tran/details")]
		public void GetTransaction(int portfolioId)
		{
			ComponentFactory.GetWebScrapperObject().GetTransaction();
		}
		[HttpPost("updatefolio")]
		public ActionResult<bool> PostPortfolio(EquityTransaction tran)
		{
			if (tran.typeAsset == 1 || tran.typeAsset==2)
			{
				return ComponentFactory.GetMySqlObject().postEquityTransaction(tran);
			}
			else if (tran.typeAsset == 12 || tran.typeAsset ==7 || tran.typeAsset == 8)
			{
				return ComponentFactory.GetMySqlObject().postGoldTransaction(tran);
			}
			return false;
		}
	}
}

