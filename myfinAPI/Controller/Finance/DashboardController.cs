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
		[HttpGet]
		public ActionResult<IEnumerable<DashboardDetail>> GetDashboard()
		{
			IList<DashboardDetail> dashBoard = new List<DashboardDetail>();			
 
			ComponentFactory.GetMySqlObject().GetSharesDetails(dashBoard);
			ComponentFactory.GetMySqlObject().GetPropertyCurrentValue(dashBoard);
			var bankdetails = ComponentFactory.GetMySqlObject().GetBankAssetDetails().ToArray();
			foreach (TotalBankAsset asset in bankdetails)
			{
				dashBoard.Add(new DashboardDetail()
				{
					assetName = asset.actType,
					total = asset.totalAmt
				}); ;
			}
			return dashBoard.ToArray();
		}
	}
}

