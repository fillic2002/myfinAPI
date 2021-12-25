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
		public void GetTransaction()
		{
			ComponentFactory.GetWebScrapperObject().GetTransaction();
		}
		[HttpGet("getInvestment")]
		public ActionResult<IEnumerable<AssetHistory>> GetInvestmentPerYear()
		{
			return ComponentFactory.GetTranObject().GetYearlyInvestment().ToArray();
		}
		[HttpGet("tran/{portfolioId}/{equity}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetTransaction(int portfolioId, string equity)
		{
			return ComponentFactory.GetMySqlObject().getTransaction(portfolioId, equity).ToArray();
		}
		[HttpPost("updatefolio")]
		public ActionResult<bool> PostPortfolio(EquityTransaction tran)
		{
			if (tran.typeAsset == 1 || tran.typeAsset==2 ||  tran.typeAsset == 5)
			{
				return ComponentFactory.GetMySqlObject().postEquityTransaction(tran);
			}
			else if (tran.typeAsset == 12 || tran.typeAsset ==7 || tran.typeAsset == 8)
			{
				return ComponentFactory.GetMySqlObject().postGoldTransaction(tran);
			}
			return false;
		}

		[HttpPost("deletetransction")]
		public ActionResult<bool> PostTransaction(EquityTransaction tran)
		{
			return ComponentFactory.GetMySqlObject().RemoveTransaction(tran);
		}

	}
}

