using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using myfinAPI.Model;
using MySqlConnector;

namespace myfinAPI.Data
{
	public class mysqlContext :DbContext
	{
		string connString = "Server = localhost; Database = myfin; Uid = root; Pwd = Welcome@1; ";
		 
		public IList<ShareInfo> getShare()
		{
			using (MySqlConnection _conn= new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand("SELECT * FROM myfin.shareinfo;", _conn);
				using var reader = command.ExecuteReader();
				IList<ShareInfo> sharesList = new List<ShareInfo>();
				while (reader.Read())
				{
					sharesList.Add(new ShareInfo()
					{
						id = reader.GetValue(0).ToString(),
						fullName = reader.GetValue(1).ToString(),
						shortName = reader.GetValue(2).ToString()
					});
				}
				return sharesList;
			}			
		}

		public IList<EquityTransaction> getPortfolio(int portfolioID)
		{
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string query;
				if (portfolioID != 0)
				{
					query = @"SELECT ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN,ed.assettypeid,ed.liveprice,st.transactiondate
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.isin
						where st.portfolioId=" + portfolioID;
				}
				else
				{
					query= @"SELECT ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN,ed.assettypeid,ed.liveprice,st.transactiondate
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.isin";
				}
				using var command = new MySqlCommand(query, _conn);
				

				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						tranList.Add(new EquityTransaction()
						{
							equityName = reader["name"].ToString(),
							qty = Convert.ToDouble(reader["qty"]),
							tranType = reader["action"].ToString(),
							price = Convert.ToDouble(reader["price"]),
							equityId = reader["ISIN"].ToString(),
							symbol = reader["symbol"].ToString(),
							typeAsset = Convert.ToInt32(reader["assettypeid"]),
							livePrice= Convert.ToDouble(reader["liveprice"]),
							tranDate = Convert.ToDateTime(reader["transactiondate"])

						}); 
					}
				}
			}
			return tranList;
			
		}

		public IList<portfolio> getUserfolio()
		{
			IList<portfolio> folioList = new List<portfolio>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"SELECT folioname, portfolioid FROM myfin.portfolio;", _conn);
				using var reader = command.ExecuteReader();
				
				while (reader.Read())
				{
					folioList.Add(new portfolio()
					{
						folioName = reader["folioname"].ToString(),
						folioID = (int)reader["portfolioid"]
					});
				}
			}
			return folioList;

		}

		public IList<EquityTransaction> getTransaction(int portfolioId)
		{
			 
				try
				{
					IList<EquityTransaction> transactionList = new List<EquityTransaction>();
				using (MySqlConnection _conn = new MySqlConnection(connString))
				{
					_conn.Open();
					using var command = new MySqlCommand(@"SELECT *
								FROM myfin.equitytransactions as et
								join myfin.equitydetails ed
								on et.isin=ed.isin Where et.portfolioid=" + portfolioId + " Order by ed.name, et.transactiondate;", _conn);

					
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							transactionList.Add(new EquityTransaction()
							{
								equityId = reader["isin"].ToString(),
								portfolioId = Convert.ToInt32(reader["portfolioId"]),
								tranDate = Convert.ToDateTime(reader["transactiondate"]),
								qty = Convert.ToInt32(reader["qty"]),
								price = Convert.ToDouble(reader["price"]),
								equityName = reader["Name"].ToString(),
								tranType = reader["action"].ToString()
							});
						}
					}
				}
				return transactionList;
			}
			catch(Exception ex)
			{
				string msg = ex.Message;
				return null;
			} 

		}

		public bool postEquityTransaction(EquityTransaction tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.equitytransactions ( price, action,isin,qty,portfolioid,transactiondate) 
												VALUES ( " + tran.price + ",'" + tran.tranType + "','" + tran.equityId + "'," + tran.qty + "," + tran.portfolioId + ",'" + dt + "');", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool RemoveTransaction(EquityTransaction tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"DELETE FROM myfin.equitytransactions WHERE ISIN='"+tran.equityId+"' AND transactiondate='"+dt+"';", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}

		public bool postGoldTransaction(EquityTransaction tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.propertytransaction ( assettype, dtofpurchase,assetvalue,qty,portfolioid) 
												VALUES ( " + tran.typeAsset + ",'" + dt + "','" + tran.price + "'," + tran.qty + "," + tran.portfolioId + ");", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}

		public bool postBankTransaction(BankDetail tran)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				 
				_conn.Open();
				string dt = tran.transactionDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"REPLACE INTO myfin.bankdetail ( amt, useracctid,roi,datetotransaction,userid) 
												VALUES ( " + tran.amt + ",'" + tran.acctId + "'," + tran.roi + ",'" + dt + "'," + tran.userid + ");", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;

		}

		public bool UpdateLivePrice(string isin,double liveprice)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"UPDATE myfin.equitydetails SET liveprice = " + liveprice + "," +
									" dtupdated ='" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "' WHERE (ISIN = '" + isin + "');", _conn);

				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public void GetSharesDetails(IList<DashboardDetail> assetTypeList)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"select SUM(et.qty*et.price) as total,aty.name
								from myfin.equitytransactions as et
								join myfin.equitydetails ed
								on ed.isin = et.isin
                                join myfin.assettype aty
                                on aty.idassettype=ed.assettypeid
                                group by assettypeid;", _conn);
				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					assetTypeList.Add(new DashboardDetail()
					{
						Total = Convert.ToDouble(reader["total"]),
						AssetName = reader["name"].ToString()

					});
				}
			}
			 
		}
		public void GetPropertyCurrentValue(IList<DashboardDetail> assetTypeList)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"select sum(currentvalue) as total,ast.name
									from myfin.propertytransaction pro
									join myfin.assettype ast
									on ast.idassettype=pro.assettype
									group by assettype;", _conn);
				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					assetTypeList.Add(new DashboardDetail()
					{
						Total = Convert.ToDouble(reader["total"]),
						AssetName = reader["name"].ToString()

					});
				}
			}			 
		}
		private string getAssetName(int type)
		{
			if (type == 12)
				return "Gold";
			return "";
		}
		//public IList<BankDetail> PostBankTransaction()
		//{
		//	if (connection.State != ConnectionState.Open)
		//		connection.Open();


		//	using var command = new MySqlCommand(@"select * from bankdetail", connection);
		//	using var reader = command.ExecuteReader();
		//	IList<BankDetail> acctDetail = new List<BankDetail>();
		//	while (reader.Read())
		//	{
		//		acctDetail.Add(new BankDetail()
		//		{
		//			acctId = Convert.ToInt32(reader["acctId"]),
		//			amt = Convert.ToDouble(reader["amt"]),
		//			roi = Convert.ToDouble(reader["roi"])

		//		});
		//	}
		//	return acctDetail;
		//}

		public IList<TotalBankAsset> GetBankAssetDetails()
		{
			IList<TotalBankAsset> assetTypeList = new List<TotalBankAsset>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"SELECT sum(amt) as total,ast.name FROM myfin.bankdetail bd
							join myfin.bankaccountinfo bi 
							on bi.acctid =bd.useracctId
							join myfin.assettype ast
							on ast.idAssetType = bi.accttype
							WHERE isActive=1
							group by ast.Name;", _conn);
			 
				var reader = command.ExecuteReader();
				while (reader.Read())

				{
					assetTypeList.Add(new TotalBankAsset()
					{
						totalAmt = Convert.ToDouble(reader["total"]),
						actType = reader["name"].ToString()
					});
				}
			} 
			return assetTypeList;
		}

		public IList<BankDetail> GetBankAssetDetail()
		{
			IList<BankDetail> assetTypeList = new List<BankDetail>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"select * from myfin.bankdetail bd
													join myfin.bankaccountinfo ba
													ON bd.useracctid=ba.acctid
													where isactive=1;", _conn);
					 

				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					assetTypeList.Add(new BankDetail()
					{
						acctName = reader["Bank Name"].ToString(),
						acctId = (int)reader["useracctid"],
						acctType = reader["assettype"].ToString(),
						amt = Convert.ToDouble(reader["amt"]),
						roi = Convert.ToDouble(reader["roi"]),
						transactionDate = Convert.ToDateTime(reader["datetotransaction"]),
						userid= (int)reader["userid"]
					});
				}
			}
			return assetTypeList;
		}

		public IList<ShareInfo> searchshare(string name)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				IList<ShareInfo> _eqs = new List<ShareInfo>();
				using var command = new MySqlCommand(@"select * from myfin.equitydetails where name like '" + name + "%';", _conn);
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					_eqs.Add(new ShareInfo()
					{
						 fullName  = reader["Name"].ToString(),
						id = reader["isin"].ToString()
					});
				}
				return _eqs;
			}
		}
		public double GetDividend(string assetId, DateTime after, DateTime before)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				using var command = new MySqlCommand(@"select sum(dividend) from myfin.dividend
							where isin='"+assetId+"' AND dtupdated >= '"+after.ToString("yyyy-MM-dd") +"' AND dtupdated<='"+ before.ToString("yyyy-MM-dd") +"' group by  isin;", _conn);
				var result = command.ExecuteScalar();
				if (result is null)
					return 0;
				return (double)result;
			}
		}
		public EquityBase GetLivePrice (string assetId)
		{
			EquityBase _eq = new EquityBase();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{			
				_conn.Open();
				using var command = new MySqlCommand(@"SELECT dtUpdated,liveprice,description
											FROM myfin.equitydetails ed
											where ed.isin='" + assetId + "';", _conn);

				using var reader = command.ExecuteReader();
				 

				while (reader.Read())
				{
					var dt = reader["dtUpdated"];
					if (dt != DBNull.Value)
					{
						if (Convert.ToDateTime(dt).Date == DateTime.Today)
						{
							_eq.livePrice = Convert.ToDouble(reader["liveprice"]);							
						}
						else
							_eq.description = reader["description"].ToString();
					}
				}
			}
			return _eq;

		}
	}
}
