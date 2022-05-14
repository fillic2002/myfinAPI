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

namespace myfinAPI.Controller
{

	[Route("[controller]")]
	[ApiController]
	public class portfolioController : ControllerBase
	{
		[HttpGet("Getfolio/{portfolioId}")]
		public ActionResult<IEnumerable<portfolio>> GetPortfolio(int portfolioId)
		{
			List<portfolio> PortFolios = new List<portfolio>();			
			ComponentFactory.GetPortfolioObject().GetFolio(portfolioId, PortFolios).ToArray();
			return PortFolios.Where(x=>x.qty>0).ToArray();
			
		}

		/// <summary>
		/// Calculate dividend details till today since the date of first purchase
		/// </summary>
		/// <param name="companyId"></param>
		/// <param name="before"></param>
		/// <param name="after"></param>
		/// <param name="qty"></param>
		/// <returns></returns>
		public double CalculateDividend(string companyId, IList<EquityTransaction> t)
		{
			IList<dividend> divDetails = new List<dividend>();
			ComponentFactory.GetMySqlObject().GetDividend(companyId, divDetails);
			
			double dividend = 0;
			foreach (dividend div in divDetails)
			{
				double q = 0;
				foreach (EquityTransaction tran in t.Where(x=>x.equityId==div.companyid && x.tranDate<div.dt))
				{					
						if (tran.tranType == "B")
							q += tran.qty;
						else
							q -= tran.qty;					
				}

				if (q > 0)
				{					 
					dividend += q * div.value;
					//if(p.folioId==2)
					//	Console.WriteLine("EQUITY:"+div.companyid +" Dividend:"+ equities[div.companyid]);
				}
			}
			return dividend;
		}
		public double getLivePrice(portfolio n)
		{
			Task<double> task = new Task<double>(() =>
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
			return ComponentFactory.GetPortfolioObject().GetAssetReturn(portfolioId, assetId).ToArray();
		}
		[HttpGet("getAssetsReturn/{assetId}")]
		public ActionResult<IEnumerable<AssetReturn>> GetAssetReturn(int assetId)
		{
			return ComponentFactory.GetPortfolioObject().GetAssetReturn(assetId).ToArray();
		}
	}
}
