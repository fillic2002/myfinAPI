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
		[HttpGet]
		public ActionResult<IEnumerable<portfolio>> GetPortfolio()
		{

			mysqlContext obj = new mysqlContext();

			return obj.getPortfolio().ToArray();
		}
	}
}
