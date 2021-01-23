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
						finalFolio[indx].qty = finalFolio[indx].qty + eq.qty;
						finalFolio[indx].avgprice += eq.price * eq.qty; ;
					}
				}				
				else
				{
					//add
					finalFolio.Add(new portfolio() { 
					EquityName = eq.equityName,
					qty = eq.qty,
					avgprice=eq.price*eq.qty,
					EquityId = eq.equityId
					
					});
				}
			}
			finalFolio.ForEach(
				n => {
					n.avgprice = n.avgprice / n.qty;
					n.livePrice = Convert.ToDouble(ComponentFactory.GetWebScrapperObject().GetLivePrice(n.EquityId));
				}) ;
			
			return finalFolio;
		}

		[HttpGet("GetAllfolio")]
		public ActionResult<IEnumerable<Ibasefolio>> GetUserFolio()
		{
			mysqlContext obj = new mysqlContext();
			
			return obj.getUserfolio().ToArray();

		}

	}
}
