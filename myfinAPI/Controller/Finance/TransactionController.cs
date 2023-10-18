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
		[HttpGet("getAllTransaction/{portfolioId}")]
		public ActionResult<IEnumerable<EquityTransaction>> GetAllTransaction(int portfolioId)
		{
			IList<EquityTransaction> transactionList = new List<EquityTransaction>();
			 ComponentFactory.GetEquityHelperObj().GetAllTransaction(portfolioId, transactionList);
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
		[HttpGet("getInvestment/{flag}/{folioId}")]
		public ActionResult<IEnumerable<AssetHistory>> GetInvestmentPerYear(string flag, int folioId)
		{
			if (flag == "Yearly")
			{
				return ComponentFactory.GetTranObject().GetYearlyInvestment(folioId).ToArray();
			}
			else
			{
				return ComponentFactory.GetTranObject().GetMonthlyInvestment(0).ToArray();
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
			if (tran.equity.assetType== AssetType.Shares || tran.equity.assetType == AssetType.Equity_MF || tran.equity.assetType == AssetType.Debt_MF)
			{
				return ComponentFactory.GetEquityHelperObj().AddEqtyTransaction(tran);
					 
			}
			else if (tran.equity.assetType == AssetType.Gold || tran.equity.assetType == AssetType.Plot || tran.equity.assetType == AssetType.Flat)
			{
				return ComponentFactory.GetMySqlObject().postPropertyTransaction(tran);
			}else if(tran.equity.assetType == AssetType.Bonds)
			{				
				return ComponentFactory.GetBondhelperObj().AddBondTransaction(tran);
			}
			return false;
		}
		[HttpPost("verifyTransaction")]
		public ActionResult<bool> VerifyTransaction(EquityTransaction tran)
		{
			return ComponentFactory.GetEquityHelperObj().VerifyTransaction(tran);
		}

		[HttpPost("AddBankTransaction")]
		public ActionResult<bool> PostBankTransaction(BankTransaction tran)
		{			  
			return ComponentFactory.GetTranObject().AddBankTran(tran);			
		}
		[HttpPost("AddPFTransaction")]
		public ActionResult<bool> PostPFTransaction(PFAccount tran)
		{			
			return ComponentFactory.GetTranObject().AddPFTran(tran);
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
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, tran);
			return tran.ToArray();
		}
		[HttpPost("UploadTransactionFile/{folioId}")]
		public ActionResult<bool> UploadTranFile(int folioId, Microsoft.AspNetCore.Http.IFormFile file)
		{
			IList<EquityTransaction> tran = new List<EquityTransaction>();
			ComponentFactory.GetTranObject().UploadTranFile(file, folioId);
			return true;
		}
	}
}

