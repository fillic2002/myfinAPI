using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.DTO;

namespace myfinAPI.Controller
{
	[Route("[controller]")]
	[ApiController]
	public class TransactionController : ControllerBase
	{
		[HttpGet("getfolio/{portfolioId}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetPortfolio(int portfolioId)
		{
			return ComponentFactory.GetMySqlObject().GetTransaction(portfolioId).ToArray();
		}
		[HttpGet("tran/details")]
		public void GetTransaction()
		{
			ComponentFactory.GetWebScrapperObject().GetTransaction();
		}
		[HttpGet("tran/{folio}/{month}/{year}/{asstType}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetTransaction(int folio, int month, int asstType, int year)
		{
			return ComponentFactory.GetMySqlObject().GetTransaction(folio, month, asstType,year).ToArray();
		}
		[HttpGet("getInvestment/{flag}")]
		public ActionResult<IEnumerable<AssetHistory>> GetInvestmentPerYear(string flag)
		{
			if (flag == "Yearly")
			{
				return ComponentFactory.GetTranObject().GetYearlyInvestment().ToArray();
			}
			else
			{
				return ComponentFactory.GetTranObject().GetMonthlyInvestment().ToArray();
			}
		}
		[HttpGet("tran/{portfolioId}/{equity}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetTransaction(int portfolioId, string equity)
		{
			return ComponentFactory.GetMySqlObject().GetTransaction(portfolioId, equity).ToArray();
		}
		[HttpPost("updatefolio")]
		public ActionResult<bool> PostPortfolio(EquityTransaction tran)
		{
			if (tran.assetType == 1 || tran.assetType==2 ||  tran.assetType == 5)
			{
				return ComponentFactory.GetMySqlObject().PostEquityTransaction(tran);
			}
			else if (tran.assetType == 12 || tran.assetType == 7 || tran.assetType == 8)
			{
				return ComponentFactory.GetMySqlObject().postGoldTransaction(tran);
			}
			return false;
		}

		[HttpPost("AddBankTransaction")]
		public ActionResult<bool> PostBankTransaction(BankTransaction tran)
		{
			  
			return ComponentFactory.GetTranObject().AddTransaction(tran);
			  
		}

		[HttpPost("deletetransction")]
		public ActionResult<bool> PostTransaction(EquityTransaction tran)
		{
			return ComponentFactory.GetMySqlObject().RemoveTransaction(tran);
		}

	}
}

