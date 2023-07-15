﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model.DTO
{
	public class AssetClass
	{
		public string AssetClassName { get; set; }
		public double Investment { get; set; }
		public double Profit { get; set; }
		public double percent{ get; set; }
		public CompanySize cmpSize { get; set; }
	}
	public enum CompanySize
	{
		Small,
		Mid,
		Large,
		Enterprise
	}
}
