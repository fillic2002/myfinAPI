using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myfinAPI.Model
{
	//need to replace this with equity base class
	public class ShareInfo
	{
		public string id { get; set; }
		public string shortName { get; set; }
		public string fullName { get; set; }
		public string livePrice { get; set; }
		public string desc { get; set; }
		public string divlink{ get; set; }
		public string sector { get; set; }

	}
}
