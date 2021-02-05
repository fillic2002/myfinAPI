using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Model;

namespace myfinAPI.Controller
{
	 
	[Route("[controller]")]
	[ApiController]
	public class SharesController : ControllerBase
	{
		//private static readonly string[] Summaries = new[]
		//{
		//	"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		//};

		//private readonly ILogger<WeatherForecastController> _logger;

		//public WeatherForecastController(ILogger<WeatherForecastController> logger)
		//{
		//	_logger = logger;
		//}
		 
		[HttpGet]
		public ActionResult<IEnumerable<ShareInfo>> Get()
		{
			 
			mysqlContext obj = new mysqlContext();	

			return obj.getShare().ToArray();
		}

		//[HttpGet("GetLivePrice")]
		//public ActionResult<IEnumerable<EquityTransaction>> GetLivePrice(IList<EquityTransaction> listOfShare)
		//{
		//	WebScrapper obj = new WebScrapper();

		//	obj.GetLivePriceAsync(listOfShare[0].symbol ,listOfShare[0].equityId);

		//	return listOfShare.ToArray();

		//}


	}
}
