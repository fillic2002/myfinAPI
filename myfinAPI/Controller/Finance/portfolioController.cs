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
						//update
						finalFolio[indx].qty = finalFolio[indx].qty - eq.qty;
						finalFolio[indx].avgprice -= eq.price * eq.qty;
					}
					else
					{
						finalFolio[indx].dividend=GetDividend(eq.equityId, eq.tranDate, finalFolio[indx].trandate, eq.qty);
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
			//var tasks = new List<Task>();
			//var response = Task.Factory.StartNew(ComponentFactory.GetWebScrapperObject().GetLivePriceAsync(n));
			
			finalFolio.ForEach(
				async n => {
					n.avgprice = n.avgprice / n.qty;
					n.dividend = GetDividend(n.EquityId, DateTime.Now, n.trandate, n.qty);
				}) ;
		 
			return finalFolio;
		}

		public double GetDividend(string id, DateTime before,DateTime after, double qty)
		{
			return qty*ComponentFactory.GetMySqlObject().GetDividend(id, after, before);
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
		[HttpGet("GetFolioSnapshot/{portfolioId}")]
		public ActionResult<IEnumerable<AssetHistory>> GetFolioSnapshot(int portfolioId)
		{	
			return ComponentFactory.GetMySqlObject().GetAssetSnapshot(portfolioId).ToArray();
		}
	}
}
