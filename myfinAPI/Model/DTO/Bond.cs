using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class Bond
	{
		public DateTime updateDate { get; set; }
		public double couponRate { get; set; }
		public string BondName{ get; set; }
		public string BondId { get; set; }		
		public double minInvst{ get; set; }
		public DateTime dateOfMaturity { get; set; }
		/// <summary>
		/// Yield to maturity
		/// </summary>
		public double YTM { get; set; }
	}
}
