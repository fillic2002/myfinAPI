using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Controller
{
	[Route("[controller]")]
	[ApiController]
	public class TransactionController : ControllerBase
	{
		[HttpGet("getfolio/{portfolioId}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetPortfolio(int portfolioId)
		{
			IList<EquityTransaction> transactionList = new List<EquityTransaction>();
			 ComponentFactory.GetTranObject().GetAllTransaction(portfolioId, transactionList);
			return transactionList.ToArray();
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
		[HttpGet("getYrlyEqtInvst/{portfolioId}/{equity}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetYrlyEqtTransaction(int portfolioId, string equity)
		{		
			return ComponentFactory.GetTranObject().GetYearlyInvestment(portfolioId, equity).ToArray();			
		}
		[HttpGet("tran/{portfolioId}/{equity}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetTransaction(int portfolioId, string equity)
		{
			return ComponentFactory.GetTranObject().GetTransaction(portfolioId, equity).ToArray();
		}
		[HttpPost("postTransaction")]
		public ActionResult<bool> PostPortfolio(EquityTransaction tran)
		{
			if (tran.assetTypeId == AssetType.Shares || tran.assetTypeId==AssetType.Equity_MF ||  tran.assetTypeId == AssetType.Debt_MF)
			{
				return ComponentFactory.GetTranObject().AddEqtyTransaction(tran);
					 
			}
			else if (tran.assetTypeId == AssetType.Gold || tran.assetTypeId == AssetType.Plot || tran.assetTypeId == AssetType.Flat)
			{
				return ComponentFactory.GetMySqlObject().postPropertyTransaction(tran);
			}
			return false;
		}

		[HttpPost("AddBankTransaction")]
		public ActionResult<bool> PostBankTransaction(BankTransaction tran)
		{
			  
			return ComponentFactory.GetTranObject().AddPFTransaction(tran);
			  
		}

		[HttpPost("deletetransction")]
		public ActionResult<bool> PostTransaction(EquityTransaction tran)
		{
			return ComponentFactory.GetMySqlObject().RemoveTransaction(tran);
		}

		[HttpPost("GetBonds/{folioId}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetBondTransactionDetails(int folioId)
		{
			IList<EquityTransaction> tran = new List<EquityTransaction>();
			ComponentFactory.GetTranObject().GetBondTransaction(folioId, tran);
			return tran.ToArray();
		}
	}
}

