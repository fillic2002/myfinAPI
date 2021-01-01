using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Model;

namespace myfinAPI.Controller
{

	[Route("[controller]")]
	[ApiController]
	public class portfolioController : ControllerBase
	{
		[HttpGet("Getfolio")]
		public ActionResult<IEnumerable<portfolio>> GetPortfolio(int portfolioId)
		{

			mysqlContext obj = new mysqlContext();
			portfolioId = 0;
			List<portfolio> finalFolio = new List<portfolio>();
			IList<EquityTransaction> tranDetails = obj.getPortfolio(portfolioId).ToArray();
			
			foreach(EquityTransaction eq in tranDetails)
			{
				int indx = finalFolio.FindIndex(x => x.equityname == eq.equityName);
				if(indx>=0)
				{
					if (eq.tranType == "S")
						//update
						finalFolio[indx].qty = finalFolio[indx].qty - eq.qty;
					else
						finalFolio[indx].qty = finalFolio[indx].qty + eq.qty;
				}				
				else
				{
					//add
					finalFolio.Add(new portfolio() { 
					equityname = eq.equityName,
					qty = eq.qty,				
					});
				}
			}

			return finalFolio;
		}

		[HttpGet("GetAllfolio")]
		public ActionResult<IEnumerable<basefolio>> GetUserFolio()
		{
			mysqlContext obj = new mysqlContext();
			
			return obj.getUserfolio().ToArray();

		}

	}
}
