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
	public class BankAssetController
	{
		[HttpGet("GetTotalAmt")]
		public ActionResult<TotalBankAsset> GetTotalBankAmt()
		{
			TotalBankAsset obj = new TotalBankAsset();
			IList<TotalBankAsset> bankDetails= ComponentFactory.GetMySqlObject().GetBankAssetDetails();
			foreach(TotalBankAsset item in bankDetails)
			{
				obj.totalAmt += item.totalAmt;
			}
			return obj;
		}
		[HttpGet("GetDetailedAmt")]
		public ActionResult<IEnumerable<BankDetail>> GetBankAssetDetail()
		{
			mysqlContext obj = new mysqlContext();
			return obj.GetBankAssetDetail().ToArray();
		}
		[HttpPost("SaveAcctStatus")]
		public ActionResult<bool> PostBankTransaction(BankDetail transactionDetail)
		{
			mysqlContext obj = new mysqlContext();
			return obj.postBankTransaction(transactionDetail);
		}
	}
}
