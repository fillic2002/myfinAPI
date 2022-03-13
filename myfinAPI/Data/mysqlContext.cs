using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
using myfinAPI.Model.DTO;
using MySqlConnector;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Data
{
	public class mysqlContext : DbContext
	{
		string connString = "Server = localhost; Database = myfin; Uid = root; Pwd = Welcome@1; ";

		public IList<ShareInfo> getShare()
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
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

		public void getTransactionDetails(int portfolioID, IList<EquityTransaction> tranList)
		{
			//IList<EquityTransaction> tranList = new List<EquityTransaction>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string query;
				if (portfolioID != 0)
				{
					query = @"SELECT ed.sector,ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN,ed.assettypeid,ed.liveprice,st.transactiondate
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.isin
						where st.portfolioId=" + portfolioID + " order by transactiondate asc";
				}
				else
				{
					query = @"SELECT ed.sector,ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN,ed.assettypeid,ed.liveprice,st.transactiondate
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.isin order by transactiondate asc";
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
							assetType = Convert.ToInt32(reader["assettypeid"]),
							livePrice = Convert.ToDouble(reader["liveprice"]),
							tranDate = Convert.ToDateTime(reader["transactiondate"]),
							sector = reader["sector"].ToString()

						});
					}
				}
			}
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
					MySqlCommand command;
					if (portfolioId == 0)
					{
						 
						command = new MySqlCommand(@"SELECT et.isin,et.portfolioId,et.transactiondate,et.qty,et.price,et.action,ed.name,ed.assettypeid,et.pb,et.marketcap
								FROM myfin.equitytransactions as et
								join myfin.equitydetails ed
								on et.isin = ed.isin Order by et.transactiondate desc;", _conn);
					}
					else
					{
						command = new MySqlCommand(@"SELECT et.isin,et.portfolioId,et.transactiondate,et.qty,et.price,et.action,ed.name,ed.assettypeid,et.pb,et.marketcap
								FROM myfin.equitytransactions as et
								join myfin.equitydetails ed
								on et.isin=ed.isin Where et.portfolioid=" + portfolioId + " Order by et.transactiondate desc;", _conn);
					}

					 
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
								tranType = reader["action"].ToString(),
								assetType = Convert.ToInt32(reader["assettypeid"]),
								PB = Convert.ToDouble(reader["PB"]),
								MarketCap = Convert.ToDouble(reader["marketcap"]),
							});
						}
					}
				}
				return transactionList;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				return null;
			}

		}
		public IList<EquityTransaction> getTransaction(int portfolioId, string equityId)
		{
			try
			{
				IList<EquityTransaction> transactionList = new List<EquityTransaction>();
				using (MySqlConnection _conn = new MySqlConnection(connString))
				{
					_conn.Open();
					MySqlCommand command;
					if (portfolioId ==0)
					{
						command = new MySqlCommand(@"select * from myfin.equitytransactions etr
								where ISIN='" + equityId + "' Order by transactiondate desc;",_conn);
					}
					else
					{
						 command = new MySqlCommand(@"select * from myfin.equitytransactions etr
												where ISIN='" + equityId + "' and portfolioid=" + portfolioId + " Order by transactiondate desc;", _conn);
					}
 
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
								//equityName = reader["Name"].ToString(),
								tranType = reader["action"].ToString(),
								PB= Convert.ToDouble(reader["pb"])

							});
						}
					}
				}
				return transactionList;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				return null;
			}
		}
		public bool postBankTransaction(BankTransaction tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				//string dt = tran.tranDate.ToString("yyyy-MM-dd");
				//using var command = new MySqlCommand(@"INSERT INTO myfin.banktransactions (Amt,typeoftransactioin,folioid,Acctid,dtoftransaction,description) 
												//VALUES ( " + tran.Amt + ",'" + tran.tranType+ "','" + tran.folioId+ "'," + tran.AcctId+ ",'" + dt + "',"+tran.Description+");", _conn);
				//int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool postEquityTransaction(EquityTransaction tran)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.equitytransactions ( price, action,isin,qty,portfolioid,transactiondate,PB,marketcap) 
												VALUES ( " + tran.price + ",'" + tran.tranType + "','" + tran.equityId + "'," + tran.qty + "," + tran.portfolioId + ",'" + dt + "',"+tran.PB+","+tran.MarketCap+");", _conn);
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
				using var command = new MySqlCommand(@"DELETE FROM myfin.equitytransactions WHERE ISIN='" + tran.equityId + "' AND transactiondate='" + dt + "';", _conn);
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
				using var command = new MySqlCommand(@"INSERT INTO myfin.propertytransaction ( assettype, dtofpurchase,purchaseprc,qty,portfolioid) 
												VALUES ( " + tran.assetType + ",'" + dt + "','" + tran.price + "'," + tran.qty + "," + tran.portfolioId + ");", _conn);
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
				using var command = new MySqlCommand(@"REPLACE INTO myfin.AccoutBalance ( amt, accttypeid,roi,dateoftransaction,folioid) 
												VALUES ( " + tran.amt + ",'" + tran.acctId + "'," + tran.roi + ",'" + dt + "'," + tran.userid + ");", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;

		}
		public void GetPFMonthlyDetails(IList<PFAccount> hstry, int folioid,int type)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				 
				using var command = new MySqlCommand(@"SELECT sum(emp) emp,sum(employer) employer,sum(pension) pension,typeofcredit,year FROM myfin.pf 
					 where folioid="+ folioid+" AND type="+type + " group by year, typeofcredit  order by year asc;", _conn);
				 
				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					hstry.Add(new PFAccount()
					{
					 	Year = Convert.ToInt32(reader["year"]),						 
						InvestmentEmp  = Convert.ToDouble(reader["emp"]),
						TypeOfTransaction = reader["typeofcredit"].ToString(),
						InvestmentEmplr = Convert.ToDouble(reader["employer"]),
						Pension= Convert.ToDouble(reader["pension"])
					});
				}
			}			
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
		//public bool UpdateEquityDetails(EquityBase e )
		//{
		//	if (e is null)
		//	{
		//		throw new ArgumentNullException(nameof(e));
		//	}

		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		using var command = new MySqlCommand(@"UPDATE myfin.equitydetails SET description = " + des + "," +
		//							" divlink ='" + e.divURL + "' WHERE (ISIN = '" + isin + "');", _conn);

		//		int result = command.ExecuteNonQuery();
		//	}
		//	return true;
		//}
		 
		 
		private string getAssetName(int type)
		{
			if (type == 12)
				return "Gold";
			return "";
		}
		 

		public IList<TotalBankAsset> GetBankAssetDetails()
		{
			IList<TotalBankAsset> assetTypeList = new List<TotalBankAsset>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"SELECT sum(amt) as total,ast.name FROM myfin.AccoutBalance bd
							join myfin.bankaccounttype bi 
							on bi.accttypeid =bd.accttypeid
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
						actType = reader["name"].ToString(),

					});
				}
			} 
			return assetTypeList;
		}
		public IList<AcctType> GetBankActType()
		{
			IList<AcctType> assetTypeList = new List<AcctType>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"select * from myfin.bankaccounttype;", _conn);

				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					assetTypeList.Add(new AcctType()
					{
						acctTypeId = (int)reader["accttypeid"],
						BankName= reader["Bank Name"].ToString(),
						acctType = reader["assettype"].ToString()						 
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
				using var command = new MySqlCommand(@"select * from myfin.AccoutBalance bd
													join myfin.bankaccounttype ba
													ON bd.accttypeid=ba.accttypeid
													where isactive=1;", _conn);
					 

				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					assetTypeList.Add(new BankDetail()
					{
						acctName = reader["Bank Name"].ToString(),
						acctId = (int)reader["accttypeid"],
						acctType = reader["assettype"].ToString(),
						amt = Convert.ToDouble(reader["amt"]),
						roi = Convert.ToDouble(reader["roi"]),
						transactionDate = Convert.ToDateTime(reader["dateoftransaction"]),
						userid= (int)reader["folioid"]
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
		public IList<dividend> getDividend(string name)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				IList<dividend> _eqs = new List<dividend>();
				using var command = new MySqlCommand(@"select * from myfin.dividend where ISIN= '" + name + "';", _conn);
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					_eqs.Add(new dividend()
					{
						dt = Convert.ToDateTime(reader["dtupdated"]),
						companyid = reader["isin"].ToString(),
						value= Convert.ToDouble(reader["dividend"])
					});
				}
				return _eqs;
			}
		}
		public void GetDividend(string assetId,IList<dividend> d )
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				using var command = new MySqlCommand(@"select * from myfin.dividend
							where isin='" + assetId + "';", _conn);
				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					d.Add(new dividend()
					{
						value = Convert.ToDouble(reader["dividend"]),
						companyid = reader["ISIN"].ToString(),
						dt =Convert.ToDateTime(reader["dtupdated"])
					}) ;
				}				
			}
		}

		public void GetNetDividend(int pastmonths, IList<CashFlow> d)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				using var command = new MySqlCommand(@"select sum(dividend) dividend,month, year from myfin.assetsnapshot 
					group by year, month
					order by year desc,month desc limit "+ pastmonths + ";", _conn);
				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					d.First(x => x.month == Convert.ToInt32(reader["month"])).Dividend = Convert.ToDouble(reader["dividend"]);
				}
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

		public IList<AssetHistory> GetYearlySnapshot(int assetId)
		{
			IList<AssetHistory> assetReturn = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (assetId == 0)
				{
					command = new MySqlCommand(@"SELECT year,month, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
								where month in (1,12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))
								group by month,year	order by year asc;", _conn);
				}
				else {
					command = new MySqlCommand(@"SELECT year,month,assettype, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
								where (month in (1,12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) and assettype="+assetId+" " +
								"group by month,year, assettype  order by year asc; ", _conn);
					}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						assetReturn.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDouble(reader["dividend"]),
							Investment = Convert.ToDouble(reader["invstmt"]),
							AssetValue = Convert.ToDouble(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"])
							
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				return assetReturn;
			}
		}

		public IList<AssetHistory> GetYearlySnapShot(int portfolioId, int assetId, bool YTD)
		{
			IList<AssetHistory> assetReturn = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (YTD)
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid=" + portfolioId + " and assettype=" + assetId + " and year="+ DateTime.Now.Year +" order by month desc limit 1;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid=" + portfolioId + " and assettype=" + assetId + " and (month in (1,12) " +
							" or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) order by year asc, month asc;", _conn);
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						assetReturn.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDouble(reader["dividend"]),
							Investment = Convert.ToDouble(reader["invstmt"]),
							AssetValue = Convert.ToDouble(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
							portfolioId = Convert.ToInt32(reader["portfolioid"] == null ? 0 : reader["portfolioid"]),
							Assettype = (reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"])
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				return assetReturn;
			}
		}
		public void GetCurrentMonthSnapShot(int portfolioId, IList<AssetHistory> assetSnapshot)
		{
			//IList<AssetHistory> assetReturn = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				 
				command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid="+portfolioId +" " +
							"and (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))" +
							"order by year asc, month asc;", _conn);
				
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						assetSnapshot.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDouble(reader["dividend"]),
							Investment = Convert.ToDouble(reader["invstmt"]),
							AssetValue = Convert.ToDouble(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
							portfolioId = Convert.ToInt32(reader["portfolioid"] == null ? 0 : reader["portfolioid"]),
							Assettype = (reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"])
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				//return assetReturn;
			}
		}

		public IList<AssetHistory> GetAssetSnapshot()
		{
			IList<AssetHistory> snapshots = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				command = new MySqlCommand(@"SELECT month,year, sum(assetvalue)as assetvalue,sum(dividend)as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
					group by month,year order by year asc, month asc;", _conn);

				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						snapshots.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDouble(reader["dividend"]),
							Investment = Convert.ToDouble(reader["invstmt"]),
							AssetValue = Convert.ToDouble(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				return snapshots;
			}
		}
		public IList<AssetHistory> GetMonthlyAssetSnapshot(int folioId, int astType)
		{
			IList<AssetHistory> snapshots = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				command = new MySqlCommand(@"SELECT month,year, sum(assetvalue)as assetvalue,sum(dividend)as dividend ,sum(invstmt) as invstmt, assettype FROM myfin.assetsnapshot 
					where assettype="+astType+"" +
					" group by month,year,assettype order by year desc, month desc;", _conn);

				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						snapshots.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDouble(reader["dividend"]),
							Investment = Convert.ToDouble(reader["invstmt"]),
							AssetValue = Convert.ToDouble(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				return snapshots;
			}
		}
		public void GetSalaryAndRental(int pastmonths, IList<CashFlow> cashFlow)
		{
			 
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string cmdText = @"SELECT sum(amt) amt, typeofTransaction,acctid,dtoftransaction,description 
						FROM myfin.banktransactions 
						where description='Salary' OR description='Rent'
						group by dtoftransaction
						order by dtoftransaction desc limit "+pastmonths+" ;   ";

				MySqlCommand command = new MySqlCommand(cmdText, _conn);
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						cashFlow.Add(new CashFlow()
						{
							Cashflow = Convert.ToDouble(reader["Amt"]),
							Dividend =0,
							year = Convert.ToDateTime(reader["dtoftransaction"]).Year,
							month = Convert.ToDateTime(reader["dtoftransaction"]).Month
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				//return TransactionDetails;
			}
		}
		public IList<EquityTransaction> GetBankCashFlow(int folioId,int months,AssetType type)
		{
			IList<EquityTransaction> TransactionDetails = new List<EquityTransaction>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string cmdText = @"SELECT * FROM myfin.banktransactions where folioid=" + folioId +
					" order by dtoftransaction desc limit " + months + ";";
				if(folioId==0)
				{
					cmdText = @"SELECT sum(amt) amt, typeofTransaction,acctid,dtoftransaction,description " +
						"FROM myfin.banktransactions group by dtoftransaction " +
						"order by dtoftransaction desc limit 12;";
				}
				MySqlCommand command = new MySqlCommand(cmdText,_conn);
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						TransactionDetails.Add(new EquityTransaction()
						{
							assetType = (int)type,
							description = reader["typeoftransaction"].ToString(),
							price = Convert.ToDouble(reader["Amt"]),
							tranDate = Convert.ToDateTime(reader["dtoftransaction"])							  
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				return TransactionDetails;
			}
		}
		public void GetAssetSnapshot(IList<AssetHistory> astHistroy, int portfolioId, int astType)
		{
			//IList<AssetHistory> snapshots = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command=null;
				if (portfolioId == 0)
				{					 
					command = new MySqlCommand(@"SELECT sum(invstmt) invstmt,sum(assetvalue) assetvalue,sum(dividend) dividend,year,month 
							FROM myfin.assetsnapshot where assettype="+ astType +"	" +
							"group by year, month order by year asc, month asc;", _conn);					
				}
				else //if (portfolioId > 0)
				{
				 command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
					where portfolioid=" + portfolioId + " and assettype="+astType+" order by year asc, month asc;", _conn);
				}
				//else if(astType >=2)
				//{
				//	command= new MySqlCommand(@"SELECT * from (SELECT sum(invstmt) invstmt,sum(dividend) dividend,
				//						sum(assetvalue) assetvalue,month,year,assettype FROM myfin.assetsnapshot 
				//						group by month,year				 
				//						order by year desc, month desc LIMIT 120)
    //                                    varGetCashFlowStatment1 ORDER by year asc, month asc;", _conn);
				//}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						astHistroy.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDouble(reader["dividend"]),
							Investment = Convert.ToDouble(reader["invstmt"]),
							AssetValue = Convert.ToDouble(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
							portfolioId = portfolioId,
							Assettype = astType
						}); 
					}
				}
				catch(Exception ex)
				{
					string s = ex.StackTrace;
				}
				//return snapshots;
			}
		}

		public void GetPropertyHistoricalValue(IList<AssetHistory> hst)
		{

		}
	}
}
