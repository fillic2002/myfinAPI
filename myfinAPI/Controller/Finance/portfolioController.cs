using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;
using ExpType = myfinAPI.Model.DTO.ExpType;

namespace myfinAPI.Controller
{

	[Route("[controller]")]
	[ApiController]
	public class portfolioController : ControllerBase
	{
		[HttpGet("Getfolio/{portfolioId}")]
		public ActionResult<IEnumerable<Investment>> GetPortfolio(int portfolioId)
		{
			List<Investment> instmts = new List<Investment>();			
			ComponentFactory.GetInvestHelperObj().GetFolioInvestments(portfolioId, instmts,DateTime.UtcNow.Year);
			return instmts.Where(x=>x.qty>0).ToArray();		
		}
		[HttpGet("AssetDistribution/{portfolioId}")]
		public ActionResult<IEnumerable<Investment>> AssetDistribution(int portfolioId)
		{
			List<Investment> PortFolios = new List<Investment>();
			ComponentFactory.GetInvestHelperObj().GetFolioInvestments(portfolioId, PortFolios,DateTime.UtcNow.Year);
			return PortFolios.Where(x => x.qty > 0).ToArray();
		}
		[HttpGet("SectorWiseAssetDistribution/{portfolioId}")]
		public ActionResult<IEnumerable<SectorAssetDistribution>> SectorWiseAssetDistribution(int portfolioId)
		{	 
			return ComponentFactory.GetPortfolioObject().SectorWiseAssetDistribution(portfolioId).ToArray(); 
		}

		/// <summary>
		/// Calculate dividend details till today since the date of first purchase
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="before"></param>
		/// <param name="after"></param>
		/// <param name="qty"></param>
		/// <returns></returns>
		//public double CalculateDividend(string companyId, IList<EquityTransaction> t)
		//{
		//	IList<dividend> divDetails = new List<dividend>();
		//	ComponentFactory.GetMySqlObject().GetCompanyDividend(companyId, divDetails);
			
		//	double dividend = 0;
		//	foreach (dividend div in divDetails)
		//	{
		//		double q = 0;
		//		foreach (EquityTransaction tran in t.Where(x=>x.equity.assetId==div.eqt.assetId && x.tranDate<div.dt))
		//		{					
		//				if (tran.tranType == TranType.Buy)
		//					q += tran.qty;
		//				else
		//					q -= tran.qty;					
		//		}

		//		if (q > 0)
		//		{					 
		//			dividend += q * div.divValue;
		//			//if(p.folioId==2)
		//			//	Console.WriteLine("EQUITY:"+div.companyid +" Dividend:"+ equities[div.companyid]);
		//		}
		//	}
		//	return dividend;
		//}
		public decimal getLivePrice(Investment n)
		{
			Task<decimal> task = new Task<decimal>(() =>
			{
				return ComponentFactory.GetWebScrapperObject().GetLivePriceAsync(n);
			});
			task.Start();
			return task.Result;
		}

		[HttpGet("GetAllfolio")]
		public ActionResult<IEnumerable<Ibasefolio>> GetUserFolio()
		{
			mysqlContext obj = new mysqlContext();
			
			return obj.GetUserfolio().ToArray();

		}
		[HttpGet("getAssetHistory/{portfolioId}/{isShare}")]
		public ActionResult<IEnumerable<AssetHistory>> GetFolioSnapshot(int portfolioId,int isShare)
		{	
			return ComponentFactory.GetPortfolioObject().GetAssetHistory(portfolioId, isShare).ToArray();
		}
		[HttpGet("getAssetsHistory")]
		public ActionResult<IEnumerable<AssetHistory>> GetUsersSnapshot(int userid)
		{
			return ComponentFactory.GetDashboardObject().GetAllAssetHistory(userid).ToArray();
		}
		[HttpGet("GetCashFlowStatment/{portfolioId}/{pastMonth}")]
		public ActionResult<IEnumerable<CashflowDTO>> GetFolioCashFlow(int portfolioId, int pastMonth)
		{
			return ComponentFactory.GetPortfolioObject().GetCashFlowStm(portfolioId,pastMonth).ToArray();
		}
		[HttpGet("GetCashFlowOutStatment/{portfolioId}/{pastMonth}")]
		public ActionResult<IEnumerable<CashflowDTO>> GetFolioCashFlowOut(int portfolioId, int pastMonth)
		{
			return ComponentFactory.GetPortfolioObject().GetCashFlowOutStm(portfolioId, pastMonth).ToArray();
		}
		[HttpGet("getAssetsReturn/{portfolioId}/{assetId}")]
		public ActionResult<IEnumerable<AssetReturn>> GetAssetReturn(int portfolioId, int assetId)
		{
			return ComponentFactory.GetPortfolioObject().GetAssetReturn(portfolioId, (AssetType)assetId).ToArray();
		}
		[HttpGet("getNetAssetsReturn/{portfolioId}/{assetId}")]
		public ActionResult<double> GetNetAssetReturn(int portfolioId, int assetId)
		{
			if((AssetType)assetId == AssetType.Bonds)
			{
				return ComponentFactory.GetPortfolioObject().GetnetXirrReturnBonds(portfolioId, (AssetType)assetId);
			}
			else
			{
				return ComponentFactory.GetPortfolioObject().GetnetXirrReturn(portfolioId, (AssetType)assetId);
			}			
		}
		[HttpGet("getAssetsReturn/{assetId}")]
		public ActionResult<IEnumerable<AssetReturn>> GetAssetReturn(int assetId)
		{
			return ComponentFactory.GetPortfolioObject().GetYearWiseAssetReturn((AssetType)assetId).ToArray();
		}
		[HttpPost("AddComment")]
		public ActionResult<bool> ReplaceComment(Investment p)
		{
			return ComponentFactory.GetPortfolioObject().ReplaceComment(p.folioID,p.Comment);
			//return true;
		}
		[HttpGet("GetfolioComment/{folioid}")]
		public ActionResult<Investment> GetComment(int folioid)
		{
			return ComponentFactory.GetPortfolioObject().GetFolioComment(folioid);
		}
		[HttpGet("GetfolioExpense/{folioid}")]
		public ActionResult<IList<ExpenseDTO>> GetExpense(int folioid)
		{
			return ComponentFactory.GetPortfolioObject().GetExpense(folioid).ToArray();
		}
		[HttpGet("GetExpenseType")]
		public ActionResult<IList<ExpType>> GetExpenseType()
		{
			return ComponentFactory.GetPortfolioObject().GetExpenseType().ToArray();
		}
		[HttpPost("AddExpenseType")]
		public ActionResult<bool> AddExpenseType(ExpType t)
		{
			return ComponentFactory.GetPortfolioObject().AddExpenseType(t);
		}
		[HttpPost("AddExpense")]
		public ActionResult<bool> AddExpense(ExpenseDTO t)
		{
			return ComponentFactory.GetPortfolioObject().AddExpense(t);
		}

		[HttpGet("GetMonthlyFolioExpense/{folioid}/{my}")]
		public ActionResult<IList<ExpenseDTO>> GetMonthlyExpense(int folioid,string my)
		{
			return ComponentFactory.GetPortfolioObject().GetMonthlyExpense(folioid,my).ToArray();
		}
		[HttpGet("GetMonthlyFolioExpenseHistory/{folioid}/{pastMonth}")]
		public ActionResult<IList<MonthlyExpenseDTO>> GetMonthlyExpenseHistory(int folioid, int pastMonth)
		{
			return ComponentFactory.GetPortfolioObject().GetMonthlyExpenseHistory(folioid, pastMonth).ToArray();
		}

		[HttpPost("DeleteExpense")]
		public ActionResult<bool> DeleteExpense(ExpenseDTO expId)
		{
			return ComponentFactory.GetPortfolioObject().DeleteExpense(expId.expId);
		}
		[HttpGet("GetMonthlyInvestment/{folioid}/{lastmonths}")]
		public ActionResult<IList<Invstmnt>> GetMonthlyInvestment(int folioid, int lastmonths)
		{
			return ComponentFactory.GetPortfolioObject().GetMonthlyInvestment(folioid, lastmonths).ToArray();
		}
		[HttpGet("GetAssetAllocationBySize/{folioid}")]
		public ActionResult<IList<Model.DTO.AssetClass>> GetAsstAllocation(int folioid)
		{
			return ComponentFactory.GetPortfolioObject().GetAssetAllocationBySize(folioid).ToArray();
		}

	}
}
