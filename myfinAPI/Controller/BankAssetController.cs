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
	public class BankAssetController
	{
		[HttpGet]
		public ActionResult<IEnumerable<BankDetail>> GetBankAcDetail()
		{
			mysqlContext obj = new mysqlContext();
			return obj.GetBankDetails().ToArray();
		}
		[HttpPost]
		public ActionResult<bool> PostPortfolio(BankDetail transactionDetail)
		{
			mysqlContext obj = new mysqlContext();
			return obj.postBankTransaction(transactionDetail);
		}
	}
}
