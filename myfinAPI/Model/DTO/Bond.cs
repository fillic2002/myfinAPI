﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static myfinAPI.Model.AssetClass;

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
		public DateTime firstIPDate { get; set; }
		public double LivePrice { get; set; }
		/// <summary>
		/// Yield to maturity
		/// </summary>
		public double YTM { get; set; }
		public double faceValue { get; set; }
		public string BondLink { get; set; }
		public string intrestCycle { get; set; }
		public string rating { get; set; }
		public string symbol { get; set; }
	}
	public class BondTransaction:Bond
	{
		public Bond BondDetail { get; set; }
		//public string BondName { get; set; }
		public double InvstPrice { get; set; }
		public double Qty { get; set; }
		public int folioId{ get; set; }
		public DateTime purchaseDate { get; set; }
		public TranType TranType { get; set; }

	}
}
