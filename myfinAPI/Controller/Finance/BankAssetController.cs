using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using myfinAPI.Data;
using myfinAPI.Factory;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using static myfinAPI.Model.AssetClass;

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
		[HttpGet("GetBankAcDetails")]
		public ActionResult<IEnumerable<BankDetail>> GetBankAssetDetail()
		{
			return ComponentFactory.GetBankObject().GetAcctDetails().ToArray();
		}
		[HttpPost("SaveAcctStatus")]
		public ActionResult<bool> PostBankTransaction(BankDetail transactionDetail)
		{		
			return ComponentFactory.GetMySqlObject().postBankTransaction(transactionDetail);			
		}
		[HttpGet("GetPfYearlyDetails/{folioid}/{typeofAct}")]
		public ActionResult<IEnumerable<PFAccount>> GetPFAcTransaction(int folioid,int typeofAct)
		{
			return ComponentFactory.GetBankObject().GetPFYearWiseDetails(folioid, Enum.Parse<AssetType>(typeofAct.ToString())).ToArray();
		}
		[HttpGet("GetMonthlyPFDetails/{folioid}/{typeofAct}/{year}")]
		public ActionResult<IEnumerable<PFAccount>> GetMonthlyDividend(int folioid, int typeofAct, int Year)
		{
			return ComponentFactory.GetBankObject().GetMonthlyPFDetails(folioid, typeofAct, Year).ToArray();
		}

		[HttpGet("GetAccoutType")]
		public ActionResult<IEnumerable<AcctType>> GetAccountType(int folioid)
		{
			return ComponentFactory.GetBankObject().GetAcctType(folioid).ToArray();
		}
	}
}
