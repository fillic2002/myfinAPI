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
			List<portfolio> finalFolio = new List<portfolio>();
			IList<EquityTransaction> tranDetails = ComponentFactory.GetMySqlObject().getPortfolio(portfolioId).ToArray();
			 

			foreach(EquityTransaction eq in tranDetails)
			{
				int indx = finalFolio.FindIndex(x => x.EquityName== eq.equityName);
				if(indx>=0)
				{
					if (eq.tranType == "S")
					{						
						finalFolio[indx].qty = finalFolio[indx].qty - eq.qty;
						finalFolio[indx].avgprice -= eq.price * eq.qty;
					}
					else
					{
						finalFolio[indx].qty = finalFolio[indx].qty + eq.qty;
						finalFolio[indx].avgprice += eq.price * eq.qty;						 
					}					
				}				
				else
				{
					//add
					finalFolio.Add(new portfolio() { 
					EquityName = eq.equityName,
					qty = eq.qty,
					avgprice=eq.price*eq.qty,
					EquityId = eq.equityId,
					symobl =eq.symbol,
					equityType = eq.typeAsset,
					livePrice =eq.livePrice,
					trandate =eq.tranDate,
					sector=eq.sector				
					});
				}
			}
			int inde = finalFolio.FindIndex(x => x.qty == 0);
			if (inde > 0)
			{
				finalFolio.RemoveAt(inde);
			}

			finalFolio.ForEach(
				 n => {
					if (n.qty >= 1)
					{
						n.avgprice = n.avgprice / n.qty;
						n.dividend = CalculateDividend(n.EquityId, tranDetails);
					}
				});
		 
			return finalFolio;
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
			
			return obj.getUserfolio().ToArray();

		}
		[HttpGet("getAssetHistory/{portfolioId}/{isShare}")]
		public ActionResult<IEnumerable<AssetHistory>> GetFolioSnapshot(int portfolioId,int isShare)
		{	
			return ComponentFactory.GetMySqlObject().GetAssetSnapshot(portfolioId, isShare).ToArray();
		}
		[HttpGet("getAssetsHistory")]
		public ActionResult<IEnumerable<AssetHistory>> GetUsersSnapshot(int userid)
		{
			return ComponentFactory.GetDashboardObject().GetAllAssetHistory(userid).ToArray();
		}
		[HttpGet("GetCashFlowStatment/{portfolioId}/{pastMonth}")]
		public ActionResult<IEnumerable<CashFlow>> GetFolioCashFlow(int portfolioId, int pastMonth)
		{
			return ComponentFactory.GetPortfolioObject().GetCashFlowStm(portfolioId,pastMonth).ToArray();
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
