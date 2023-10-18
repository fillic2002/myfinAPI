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
using myfinAPI.Model.DTO;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Controller
{
	 
	[Route("[controller]")]
	[ApiController]
	public class SharesController : ControllerBase
	{	
		 
		[HttpGet]
		public ActionResult<IEnumerable<ShareInfo>> Get()
		{			 
			mysqlContext obj = new mysqlContext();	
			return obj.getShare().ToArray();
		}

		[HttpGet("search/{name}")]
		public ActionResult<IEnumerable<EquityBase>> search(string name)
		{
			return ComponentFactory.GetMySqlObject().SearchShare(name).ToArray();		
		}
		[HttpGet("getdividend/{name}")]
		public ActionResult<EquityBase> getdividend(string name)
		{	
			return ComponentFactory.GetEquityHelperObj().GetDividend(name);
		}
		[HttpGet("getYearlyCompDividend/{year}")]
		public ActionResult<IEnumerable<Investment>> getCompanyWisedividend(int year,int folioId)
		{	
			return ComponentFactory.GetEquityHelperObj().GetCompanyWiseDiv(year, folioId).ToArray();
		}
	 
		[HttpPost("updateequity")]
		public bool updateEquity(ShareInfo shrDetail)
		{
			return ComponentFactory.GetMySqlObject().UpdateEquityDetails(shrDetail);
			 
		}

	}
}
