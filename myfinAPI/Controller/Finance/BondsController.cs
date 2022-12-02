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
		public ActionResult<IEnumerable<Bond>> GetTotalBankAmt()
		{
			 return ComponentFactory.GetBondhelperObj().GetBondDetails().ToArray();			
		}
		[HttpPost("AddBond")]
		public bool AddBondDetails(Bond bondDetail)
		{
			return ComponentFactory.GetBondhelperObj().AddBondDetails(bondDetail);
		}
		[HttpGet("GetBondPerFolio/{folioId}")]
		public ActionResult<IEnumerable<BondTransaction>> AddBondDetails(int folioId)
		{
			IList<BondTransaction> bondTran = new List<BondTransaction>();
			ComponentFactory.GetBondhelperObj().GetBondTransaction(folioId, bondTran);
			return bondTran.ToArray();
		}
	}
}
