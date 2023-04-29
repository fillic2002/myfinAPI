using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using myfinAPI.Factory;
using myfinAPI.Model.DTO;

namespace myfinAPI.Controller
{
	[Route("[controller]")]
	[ApiController]
	public class BondsController
	{
		[HttpGet("GetBondsDetails")]
		public ActionResult<IEnumerable<Bond>> GetBondDetails()
		{
			 return ComponentFactory.GetBondhelperObj().GetBondDetails().ToArray();			
		}
		[HttpPost("AddBond")]
		public bool AddBondDetails(Bond bondDetail)
		{
			return ComponentFactory.GetBondhelperObj().AddBondDetails(bondDetail);
		}
		[HttpPost("UpdateBondDetails")]
		public bool UpdateBondDetails(Bond bondDetail)
		{
			return ComponentFactory.GetBondhelperObj().SaveBondDetails(bondDetail);
		}
		[HttpGet("GetBondTrasaction/{folioId}")]
		public ActionResult<IEnumerable<BondTransaction>> GetBondPerFolio(int folioId)
		{
			IList<BondTransaction> bondTran = new List<BondTransaction>();
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, bondTran);
			return bondTran.ToArray();
		}
		[HttpGet("GetBondHolding/{folioId}")]
		public ActionResult<IEnumerable<BondHolding>> GetBondHolding(int folioId)
		{
			IList<BondHolding> bondHld = new List<BondHolding>();
			ComponentFactory.GetBondhelperObj().GetBondHoldings(folioId, bondHld);
			return bondHld.ToArray();
		}
		[HttpPost("SearchBond")]
		public ActionResult<IEnumerable<Bond>> SearchBond(Bond bondDetail)
		{
			return ComponentFactory.GetBondhelperObj().SearchBondDetails(bondDetail).ToArray(); 
		}
		[HttpPost("DeleteBondTransaction")]
		public bool DeletetBondTrans(BondTransaction bondTran)
		{
			return ComponentFactory.GetBondhelperObj().DeleteBondTransaction(bondTran);
		}
		[HttpGet("getBondIntrest/{year}")]
		public ActionResult<IEnumerable<BondIntrest>> getBondIntrest(int year, int folioId)
		{	
			return ComponentFactory.GetBondhelperObj().GetBondIntrest(year, folioId).ToArray();
		}
		[HttpGet("getMonthlyBondIntrest/{year}")]
		public ActionResult<IEnumerable<BondIntrestYearly>> getMonthlyBondIntrest(int year, int folioId)
		{
			return ComponentFactory.GetBondhelperObj().GetMonthlyBondIntrest(year, folioId).ToArray();
		}
		[HttpGet("getYearlyBondIntrest")]
		public ActionResult<IEnumerable<BondIntrestYearly>> getYearwiseBondIntrest(int year, int folioId)
		{	
			return ComponentFactory.GetBondhelperObj().GetBondIntrestYearly(folioId).ToArray();
		}
	}
}
