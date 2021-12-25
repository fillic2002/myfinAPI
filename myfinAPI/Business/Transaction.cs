using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;

namespace myfinAPI.Business
{
	public class Transaction
	{
		public IList<AssetHistory> GetYearlyInvestment()
		{
			IList<AssetHistory> result = new List<AssetHistory>();
			var transactions=ComponentFactory.GetMySqlObject().getTransaction(0).ToArray();
			foreach(EquityTransaction tran in transactions)
			{
				
					AssetHistory yearlyInvst = result.FirstOrDefault(x => x.year == tran.tranDate.Year && x.Assettype == tran.assetType);
					if (yearlyInvst == null)
					{
						result.Add(new AssetHistory()
						{
							Assettype = tran.assetType,
							Investment = tran.qty * tran.price,
							year = tran.tranDate.Year
						});
					}
					else if(tran.tranType == "B" )
					{
						yearlyInvst.Investment += tran.qty * tran.price;
					}
					else if (tran.tranType == "S")
					{
					yearlyInvst.Investment -= tran.qty * tran.price;
				}
				
			}
			return result;
		}
	}
}
