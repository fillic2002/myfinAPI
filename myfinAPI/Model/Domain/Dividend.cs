using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.Domain
{
	public class dividend
	{
		public DateTime dt { get; set; }		
		public double divValue { get; set; }
		public EquityBase eqt{get;set;}
	}
}
