using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using myfinAPI.Factory;
using myfinAPI.Model;

namespace myfinAPI.Business
{
	public class Banking
	{
		public IEnumerable<AssetHistory> GetPFDetails(int folioid)
		{
			IList<AssetHistory> astHrty = new List<AssetHistory>();
			ComponentFactory.GetMySqlObject().GetPFMonthlyDetails(astHrty, folioid);
			return astHrty;
		}
	}
}
