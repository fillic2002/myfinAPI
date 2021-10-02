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
			IList<DashboardDetail> dashBoard = new List<DashboardDetail>();			
 
			ComponentFactory.GetMySqlObject().GetSharesDetails(dashBoard);
			ComponentFactory.GetMySqlObject().GetPropertyCurrentValue(dashBoard);
			var bankdetails = ComponentFactory.GetMySqlObject().GetBankAssetDetails().ToArray();
			foreach (TotalBankAsset asset in bankdetails)
			{
				dashBoard.Add(new DashboardDetail()
				{
					AssetName = asset.actType,
					Invested = asset.totalAmt,
					CurrentValue = asset.totalAmt
				}); ;
			}
			return dashBoard.ToArray();
		}
		[HttpGet("getDashboard/History")]
		public ActionResult<IEnumerable<AssetHistory>> GetAssetHistory(int userid)
		{			 
			return ComponentFactory.GetDashboardObject().GetAllAssetHistory(userid).ToArray();
				
		}
	}
}

