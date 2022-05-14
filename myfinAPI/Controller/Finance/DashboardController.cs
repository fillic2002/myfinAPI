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
	public class DashboardController
	{
		
		[HttpGet("getDashboard")]
		public ActionResult<IEnumerable<DashboardDetail>> GetDashboardSnapshot()
		{			
			return ComponentFactory.GetDashboardObject().GetAssetSnapshot().ToArray();
		}
		[HttpGet("getDashboard/{month}/{year}")]
		public ActionResult<IEnumerable<DashboardDetail>> GetDashboardSnapshot(int year, int month)
		{
			return ComponentFactory.GetDashboardObject().GetAssetSnapshot(year, month).ToArray();
		}
		[HttpGet("getDashboard/History")]
		public ActionResult<IEnumerable<AssetHistory>> GetAssetHistory(int userid)
		{			 
			return ComponentFactory.GetDashboardObject().GetAllAssetHistory(userid).ToArray();				
		}
	}
}

