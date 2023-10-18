using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Model.DTO
{
	public class Bond
	{
		public DateTime updateDate { get; set; }
		public decimal couponRate { get; set; }
		public string BondName { get; set; }
		public string BondId { get; set; }
		public decimal minInvst { get; set; }
		public DateTime dateOfMaturity { get; set; }
		public DateTime firstIPDate { get; set; }
		public decimal LivePrice { get; set; }
		/// <summary>
		/// Yield to maturity
		/// </summary>
		public double YTM { get; set; }
		public decimal faceValue { get; set; }
		public string BondLink { get; set; }
		public string intrestCycle { get; set; }
		public string rating { get; set; }
		public string symbol { get; set; }
		public string issuer { get; set; }
	}
	public class BondTransaction
	{
		public Bond BondDetail { get; set; }
		//public string BondName { get; set; }
		public decimal InvstPrice { get; set; }
		public decimal Qty { get; set; }
		public int folioId { get; set; }
		public DateTime purchaseDate { get; set; }
		public TranType TranType { get; set; }
		public decimal AccuredIntrest { get; set; }
	}
	public class BondIntrest
	{ 
		public decimal amt { get; set; }
		public DateTime intrestPaymentDate { get; set; }
		public TranType TranType { get; set; }
		public Bond BondDetail { get; set; }
		public int folioId { get; set; }
	}
	public class BondHolding
	{
		public Bond BondDetail { get; set; }
		public int folioId { get; set; }
		public decimal Investment { get; set; }
	}
	public class BondIntrestYearly
	{
		public int Year { get; set; }
		public int folioId { get; set; }
		public decimal Intrest { get; set; }
		public int month { get; set; }

	}

}
