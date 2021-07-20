using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;

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

		[HttpGet("search/{name}")]
		public ActionResult<IEnumerable<ShareInfo>> search(string name)
		{

			return ComponentFactory.GetMySqlObject().searchshare(name).ToArray();		

		}
		[HttpGet("getdividend/{name}")]
		public ActionResult<IEnumerable<dividend>> getdividend(string name)
		{
			return ComponentFactory.GetMySqlObject().getDividend(name).ToArray();
		}


	}
}
