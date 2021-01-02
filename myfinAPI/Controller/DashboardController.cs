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
	public class DashboardController
	{
		[HttpGet]
		public ActionResult<IEnumerable<DashboardDetail>> GetDashboard()
		{
			mysqlContext obj = new mysqlContext();
			IList<DashboardDetail> dashBoard = new List<DashboardDetail>();
			return obj.GetShareAndMFDetails(dashBoard).ToArray();
		}
	}
}

