using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model.Domain
{
	public class dividend
	{
		public DateTime dt { get; set; }		
		public decimal divValue { get; set; }

		public TranType creditType { get; set; }
		//public EquityBase eqt{get;set;}
	}
}
