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
using ExpenseType = myfinAPI.Model.DTO.ExpenseType;
using ExpType = myfinAPI.Model.DTO.ExpType;

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
					query = @"SELECT ed.sector,ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN,ed.assettypeid,ed.liveprice,st.transactiondate,ed.pb,ed.marketcap
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.isin
						where st.portfolioId=" + portfolioID + " order by transactiondate asc;";
				}
				else
				{
					query = @"SELECT ed.sector,ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN,ed.assettypeid,ed.liveprice,st.transactiondate,ed.pb,ed.marketcap
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.isin order by transactiondate asc;";
				}
				using var command = new MySqlCommand(query, _conn);


				using (var reader = command.ExecuteReader())
				{
					while (reader.Read())
					{
						tranList.Add(new EquityTransaction()
						{							
							qty = Convert.ToDecimal(reader["qty"]),
							tranType = (TranType)Convert.ToInt16(reader["action"]),
							price = Convert.ToDecimal(reader["price"]),
							equity = new EquityBase()
							{
								equityName = reader["name"].ToString(),
								assetId = reader["ISIN"].ToString(),
								livePrice = Convert.ToDecimal(reader["liveprice"]),
								symbol = reader["symbol"].ToString(),
								sector = reader["sector"].ToString(),
								PB = reader["pb"] == null ? 0 : Convert.ToDecimal(reader["pb"]),
								MarketCap = reader["marketcap"] == null ? 0 : Convert.ToDecimal(reader["marketcap"]),
								assetType=(AssetType)reader["assettypeid"],
							},							
							//assetTypeId = (AssetType)reader["assettypeid"],							
							tranDate = Convert.ToDateTime(reader["transactiondate"]),							
							
						});
					}
				}
			}
		}

		public IList<Investment> GetUserfolio()
		{
			IList<Investment> folioList = new List<Investment>();
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"SELECT folioname, portfolioid FROM myfin.portfolio;", _conn);
				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					folioList.Add(new Investment()
					{
						folioName = reader["folioname"].ToString(),
						folioID = (int)reader["portfolioid"]
					});
				}
			}
			return folioList;

		}
		public IList<EquityTransaction> GetTransaction(int portfolioId,int month, int astType,int year )
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
						command = new MySqlCommand(@"select et.portfolioId,et.transactiondate,et.qty,et.price,et.action,et.pb,ed.name,et.marketcap,ed.isin   from myfin.equitytransactions et
							join myfin.equitydetails ed on ed.isin=et.isin
							where ed.assettypeid=" + astType+ " and MONTH(et.transactiondate)="+month+" AND Year(et.transactiondate)="+year+";", _conn);
					}
					else
					{
						command = new MySqlCommand(@"select et.portfolioId,et.transactiondate,et.qty,et.price,et.action,et.pb,ed.name,et.marketcap,ed.isin   from myfin.equitytransactions et
							join myfin.equitydetails ed on ed.isin=et.isin
							where ed.assettypeid=" + astType + " and MONTH(et.transactiondate)=" + month + "" +
							" AND Year(et.transactiondate)=" + year + " and portfolioid="+portfolioId+";", _conn);
					}

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							transactionList.Add(new EquityTransaction()
							{								
								portfolioId = Convert.ToInt32(reader["portfolioId"]),
								tranDate = Convert.ToDateTime(reader["transactiondate"]),
								qty = Convert.ToInt32(reader["qty"]),
								price = Convert.ToDecimal(reader["price"]),
								tranType = (TranType)Convert.ToInt16(reader["action"]),
								PB_Tran = Convert.ToDecimal(reader["pb"]),
								MarketCap_Tran = Convert.ToDecimal(reader["marketcap"]),
								equity = new EquityBase()
								{
									//PB = Convert.ToDouble(reader["pb"]),
									equityName = reader["name"].ToString(),
									//MarketCap = Convert.ToDouble(reader["marketcap"]),
									assetId = reader["isin"].ToString()
								}
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
		public void GetAllTransaction(int portfolioId, IList<EquityTransaction> transactionList)
		{
			try
			{				 
				using (MySqlConnection _conn = new MySqlConnection(connString))
				{
					_conn.Open();
					MySqlCommand command;
					if (portfolioId == 0)
					{
						 
						command = new MySqlCommand(@"SELECT  ed.sector,et.isin,et.portfolioId,et.transactiondate,et.qty,et.price,et.action,ed.name,ed.assettypeid,et.pb as pb_tran,et.marketcap as mc_Tran,ed.symbol,et.tranid,et.transactionid
								FROM myfin.equitytransactions as et
								join myfin.equitydetails ed
								on et.isin = ed.isin Order by et.transactiondate desc;", _conn);
					}
					else
					{
						command = new MySqlCommand(@"SELECT  ed.sector,et.isin,et.portfolioId,et.transactiondate,et.qty,et.price,et.action,ed.name,ed.assettypeid,et.pb as pb_tran,et.marketcap as mc_Tran,ed.symbol,et.tranid,et.transactionid
								FROM myfin.equitytransactions as et
								join myfin.equitydetails ed
								on et.isin=ed.isin Where et.portfolioid=" + portfolioId + " Order by et.transactiondate desc;", _conn);
					}

					 
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							Guid id;
							if (String.IsNullOrEmpty(reader["tranid"].ToString()) == true)
							{
								id = Guid.NewGuid();
								updateTranGUID(id, Convert.ToInt32(reader["transactionid"].ToString()));
							}
							else
							{
								string ids = reader["tranid"].ToString();
								id = Guid.Parse(ids);
							}
							transactionList.Add(new EquityTransaction()
							{
								
								portfolioId = Convert.ToInt32(reader["portfolioId"]),
								tranDate = Convert.ToDateTime(reader["transactiondate"]),
								qty = Convert.ToInt32(reader["qty"]),
								price = Convert.ToDecimal(reader["price"]),
								PB_Tran = Convert.ToDecimal(reader["pb_tran"]),
								tranType = (TranType) Convert.ToInt16(reader["action"]),
								MarketCap_Tran = Convert.ToDecimal(reader["mc_Tran"]),								
								id= Convert.ToInt32(reader["transactionid"].ToString()),
								tranId= id,
								equity = new EquityBase()
								{									
									equityName = reader["name"].ToString(),
									 symbol = reader["symbol"].ToString(),
									assetId = reader["isin"].ToString(),
									assetType = (AssetType)Convert.ToInt32(reader["assettypeid"]),
									sector = reader["sector"].ToString(),
								}

							});
						}
					}
				}				 
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				//return null;
			}

		}

		private int updateTranGUID(Guid id, int tranId)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command;
				command = new MySqlCommand(@"update myfin.equitytransactions set tranId='"+id+"' where transactionid="+tranId+";", _conn);
				return command.ExecuteNonQuery();
			}
		}

		public IList<EquityTransaction> GetYearlyInvstPerEqt(int portfolioId, string equityId, IList<EquityTransaction> transactionList)
		{
			try
			{
				using (MySqlConnection _conn = new MySqlConnection(connString))
				{
					_conn.Open();
					MySqlCommand command;
					if (portfolioId == 0)
					{
						command = new MySqlCommand(@"select ISIN, sum(qty)qty, year(transactiondate) tran,action,pb,marketcap from myfin.equitytransactions etr
								where ISIN='"+ equityId + "' group by year(transactiondate),action " +
								"Order by transactiondate asc;", _conn);
					}
					else
					{
						command = new MySqlCommand(@"select ISIN, sum(qty)qty, year(transactiondate) tran,action,pb,marketcap from myfin.equitytransactions etr
								where ISIN='" + equityId + "' AND portfolioid="+ portfolioId+" group by year(transactiondate),action " +
								"Order by transactiondate asc;", _conn);
					}

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							transactionList.Add(new EquityTransaction()
							{
								 
								portfolioId = portfolioId,
								tranDate = new DateTime( Convert.ToInt32(reader["tran"]),1,1),
								qty = Convert.ToInt32(reader["qty"]),
								MarketCap_Tran= Convert.ToDecimal(reader["marketcap"]),
								//price = Convert.ToDouble(reader["price"]),
								//MarketCap = Convert.ToDouble(reader["marketcap"]),
								tranType = (TranType) Convert.ToInt32(reader["action"]),
								PB_Tran = Convert.ToDecimal(reader["pb"]),
								equity = new EquityBase()
								{
									//PB = Convert.ToDouble(reader["pb"]),
									//equityName = reader["name"].ToString(),
									//MarketCap = Convert.ToDouble(reader["marketcap"]),
									assetId = reader["isin"].ToString()
								}
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
		public void GetTransaction(int portfolioId, string equityId, IList<EquityTransaction> transactionList)
		{
			try
			{
				//IList<EquityTransaction> transactionList = new List<EquityTransaction>();
				using (MySqlConnection _conn = new MySqlConnection(connString))
				{
					_conn.Open();
					MySqlCommand command;
					if (portfolioId ==0)
					{
						command = new MySqlCommand(@"select isin,portfolioId,transactiondate,qty,price,marketcap,action,pb,openshare,verified,tranId
								from myfin.equitytransactions etr
								where ISIN='" + equityId + "' Order by transactiondate asc;",_conn);
					}
					else
					{
						 command = new MySqlCommand(@"select isin,portfolioId,transactiondate,qty,price,marketcap,action,pb,openshare,verified,tranId
												from myfin.equitytransactions etr
												where ISIN='" + equityId + "' and portfolioid=" + portfolioId + " Order by transactiondate asc;", _conn);
					}
 
					using (var reader = command.ExecuteReader())
					{
						
						decimal qty = 0;
						decimal ownership=0;
						while (reader.Read())
						{
							object openShares = reader["openshare"] is DBNull ? 0m : (long)reader["openshare"];
							decimal totalShr;
							bool? verf = Convert.IsDBNull(reader["verified"])? false: Convert.ToBoolean(reader["verified"]);
							//totalShr =totalShr.ToString().Replace("{}", "0");
							totalShr = Convert.ToDecimal(openShares);
							 
								
							qty += Convert.ToInt32(reader["qty"]);

							if (totalShr > 0m)
							{
								ownership = (decimal)qty/totalShr;
								decimal round=Math.Round(ownership, 8);
							}

							transactionList.Add(new EquityTransaction()
							{
								//equityId = reader["isin"].ToString(),
								portfolioId = Convert.ToInt32(reader["portfolioId"]),
								tranDate = Convert.ToDateTime(reader["transactiondate"]),
								qty = Convert.ToInt32(reader["qty"]),
								price = Convert.ToDecimal(reader["price"]),
								MarketCap_Tran = Convert.ToDecimal(reader["marketcap"]),								
								tranType = (TranType)Convert.ToInt32(reader["action"]),
								PB_Tran = Convert.ToDecimal(reader["pb"]),								
								tranId = Guid.Parse(reader["tranId"].ToString()),
								freefloat_tran = totalShr,
								 Ownership=ownership,
								verified = verf,
								equity = new EquityBase()
								{								
									assetId = reader["isin"].ToString(),
									freefloat = totalShr
								}
							});
						}
					}
				}
				//return transactionList;
			}
			catch (Exception ex)
			{
				string msg = ex.Message;
				//return null;
			}
		}

		//public void GetBondTransaction(int portfolioId, IList<BondTransaction> transactionList)
		//{
		//	try {
		//		using (MySqlConnection _conn = new MySqlConnection(connString))
		//		{
		//			_conn.Open();
		//			MySqlCommand command;
		//			if (portfolioId == 0)
		//			{

		//				command = new MySqlCommand(@"SELECT * FROM myfin.bondtransaction as bt
		//						join myfin.bonddetails bd
		//						on bd.id = bt.bondid;", _conn);
		//			}
		//			else
		//			{
		//				command = new MySqlCommand(@"SELECT * FROM myfin.bondtransaction as bt
		//						join myfin.bonddetails bd
		//						on bd.id = bt.bondid where folioid=" + portfolioId + ";", _conn);
		//			}


		//			using (var reader = command.ExecuteReader())
		//			{
		//				while (reader.Read())
		//				{
		//					transactionList.Add(new BondTransaction()
		//					{
		//						BondId  = reader["id"].ToString(),
		//						folioId= Convert.ToInt32(reader["folioId"]),
		//						purchaseDate = Convert.ToDateTime(reader["dateofpurchase"]),
		//						Qty = Convert.ToInt32(reader["qty"]),
		//						InvstPrice= Convert.ToDouble(reader["price"]),
		//						BondName = reader["bondName"].ToString(),
		//						TranType = (TranType) reader["tranType"],						

		//					});
		//				}
		//			}
		//		}

		//	}catch( Exception ex)
		//	{
		//		string s = ex.Message;
		//	}
		//}
		
		public bool PostPF_PPFTransaction(PFAccount tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.DateOfTransaction.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.pf (dtofchange,emp,employer, pension,typeofcredit,folioid,acttype) 
												VALUES ( '" + dt+ "'," + tran.InvestmentEmp + "," + tran.InvestmentEmplr+ "," + tran.Pension + "," + 
												(int)tran.TypeOfTransaction + ","+ tran.Folioid +","+(int)tran.AccountType +");", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool PostBankTransaction(BankTransaction tran)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.banktransactions (Amt,typeoftransaction,folioid,Acctid,dtoftransaction,description) 
												VALUES ( " + tran.Amt + ",'" + tran.tranType+ "','" + tran.folioId+ "'," + tran.AcctId+ ",'" + dt + "','"+tran.Description+"');", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool PostEquityTransaction(EquityTransaction tran)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"REPLACE INTO myfin.equitytransactions ( price, action,isin,qty,portfolioid,transactiondate,PB,marketcap,tranid,openshare) 
												VALUES ( " + tran.price + ",'" + (int)tran.tranType + "','" + tran.equity.assetId + "'," + tran.qty + "," + tran.portfolioId + ",'" 
													+ dt + "',"+tran.equity.PB + ","+tran.equity.MarketCap + ",'"+ Guid.NewGuid() +"',"+tran.freefloat_tran+");", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool TransactionVerified(EquityTransaction tran)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				 
				using var command = new MySqlCommand(@"update myfin.equitytransactions set verified = 1 where tranid='" + tran.tranId+"';", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		//public bool PostBondTransaction(BondTransaction tran)
		//{
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		string dt = tran.purchaseDate.ToString("yyyy-MM-dd");
		//		using var command = new MySqlCommand(@"INSERT INTO myfin.bondtransaction ( bondId, price,qty,dateofpurchase, trantype,folioid) 
		//										VALUES ( " + tran.BondId + "," + tran.InvstPrice+ "," + tran.Qty+ ",'" + dt+ "'," + tran.TranType + "," + tran.folioId + ");", _conn);
		//		int result = command.ExecuteNonQuery();
		//	}
		//	return true;
		//}
		public bool RemoveTransaction(EquityTransaction tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"DELETE FROM myfin.equitytransactions WHERE ISIN='" + tran.equity.assetId + "' AND transactiondate='" + dt + "';", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool postPropertyTransaction(EquityTransaction tran)
		{

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				string dt = tran.tranDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.propertytransaction ( assettype, dtofpurchase,purchaseprc,qty,portfolioid,tranmode) 
												VALUES ( " + tran.equity.assetType + ",'" + dt + "','" + tran.price + "'," + tran.qty + "," + tran.portfolioId + ",'"+tran.tranType +"');", _conn);
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
		public void GetMonthlyPFContribution(int folioId, int acctType, int yr, IList<PFAccount> acct)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				//NOTE:: YEAR and MONTH column is going to be obsolete in PF DB TABLE
				_conn.Open();
				MySqlCommand command;
				if (folioId == 0)
				{
					command = new MySqlCommand(@"SELECT *
									FROM myfin.pf where Acttype=" + acctType + " AND YEAR(dtofchange)="+yr+";", _conn);
				}
				else
				{
					command = new MySqlCommand(@"SELECT *
									FROM myfin.pf where Acttype=" + acctType + " and folioid="+folioId+ " AND YEAR(dtofchange)=" + yr + ";", _conn);
				}

				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					acct.Add(new PFAccount()
					{
						Year = Convert.ToInt32(Convert.ToDateTime(reader["dtofchange"]).Year),
						InvestmentEmp = Convert.ToDecimal(reader["emp"]),
						TypeOfTransaction = Enum.Parse<TranType>( reader["typeofcredit"].ToString()),
						InvestmentEmplr = Convert.ToDecimal(reader["employer"]),
						Pension = Convert.ToDecimal(reader["pension"]),
						DateOfTransaction = Convert.ToDateTime(reader["dtofchange"]),
						Folioid = Convert.ToInt32(reader["folioid"]),
					});
				}
			}
		}
		public void GetPFYearlyDetails(IList<PFAccount> hstry, int folioid,AssetType type)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command;
				if(folioid ==0)
				{
					command = new MySqlCommand(@"SELECT sum(emp) emp,sum(employer) employer,sum(pension) pension,typeofcredit,year,dtofchange 
									FROM myfin.pf where Acttype=" + (int)type+" group by year, typeofcredit  order by year asc;", _conn);
				}
				else
				{
					 command = new MySqlCommand(@"SELECT sum(emp) emp,sum(employer) employer,sum(pension) pension,typeofcredit,year,dtofchange FROM myfin.pf 
					 where folioid=" + folioid + " AND Acttype=" + (int)type + " group by year, typeofcredit  order by year asc;", _conn);
				}
				 
				using var reader = command.ExecuteReader();

				while (reader.Read())
				{
					hstry.Add(new PFAccount()
					{
					 	Year = Convert.ToInt32(reader["year"]),						 
						InvestmentEmp  = Convert.ToDecimal(reader["emp"]),
						TypeOfTransaction = Enum.Parse<TranType>(reader["typeofcredit"].ToString()),
						InvestmentEmplr = Convert.ToDecimal(reader["employer"]),
						Pension= Convert.ToDecimal(reader["pension"]),
						DateOfTransaction = Convert.ToDateTime(reader["dtofchange"])
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
		public bool UpdateEquityDetails(ShareInfo e)
		{
			if (e is null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				using var command = new MySqlCommand(@"REPLACE myfin.equitydetails SET description = '" + e.desc + "'," +
									" divlink ='" + e.divlink + "'," +
									"Symbol='"+e.shortName+"'," +
									"sector='"+e.sector+"',name='"+e.fullName+"', ISIN= '" + e.id + "',assettypeid=1;", _conn);

				int result = command.ExecuteNonQuery(); 
			}
			return true;
		}
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
		public void GetBankAssetDetail(IList<BankDetail> assetTypeList)
		{
			//IList<BankDetail> assetTypeList = new List<BankDetail>();
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
						userid= (int)reader["folioid"],
						isActive = Convert.ToBoolean(reader["isActive"])
					});
				}
			}
			//return assetTypeList;
		}

		public EquityBase GetAssetDetail(string equityID)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				EquityBase _eqs = new EquityBase();
				using var command = new MySqlCommand(@"select * from myfin.equitydetails where ISIN like '" + equityID + "%';", _conn);
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					_eqs.equityName = reader["Name"].ToString();
					_eqs.assetId = reader["isin"].ToString();
					_eqs.sourceurl = reader["description"].ToString();
					_eqs.divUrl = reader["divlink"].ToString();
					_eqs.sector = reader["sector"].ToString();
					_eqs.symbol = reader["Symbol"].ToString();
					var frFloat =  reader["totalshare"] is DBNull ? "0" : reader["totalshare"].ToString();
					_eqs.freefloat = Convert.ToDecimal( frFloat);
				}
				return _eqs;
			}
		}
		public IList<EquityBase> SearchShare(string name)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				IList<EquityBase> _eqs = new List<EquityBase>();
				using var command = new MySqlCommand(@"select * from myfin.equitydetails where name like '" + name + "%';", _conn);
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					_eqs.Add(new EquityBase()
					{
						equityName  = reader["Name"].ToString(),
						assetId= reader["isin"].ToString(),
						sourceurl = reader["description"].ToString(),
						divUrl = reader["divlink"].ToString(),
						sector= reader["sector"].ToString(),
						symbol= reader["Symbol"].ToString()
					});
				}
				return _eqs;
			}
		}

		public void GetCompanyWiseYearyDividend(IList<dividend> _eqs,int year)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				//IList<dividend> _eqs = new List<dividend>();
				using var command = new MySqlCommand(@"select sum(Dividend) dividend,d.ISIN,Year(d.dtupdated)yr,eqd.name from myfin.dividend d 
							join myfin.equitydetails eqd on eqd.isin=d.isin
							where Year(d.dtupdated)= '" + year+"' group by Year(d.dtupdated),ISIN order by year(d.dtupdated) desc;", _conn);
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					_eqs.Add(new dividend()
					{
						dt = new DateTime(Convert.ToInt32(reader["yr"]), 1, 1),
						//eqt = new EquityBase() { assetId = reader["isin"].ToString(),
												//equityName= reader["name"].ToString()
					}						
					//divValue = Convert.ToDouble(reader["dividend"])
						
					);
				}			 
			}			 
		}
		public void GetMonthlyDividend(int folioId, IList<dividend> divDetails)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				MySqlCommand command;
				if (folioId > 0)
				{
					command = new MySqlCommand(@"select sum(dividend) dividend,month, year, portfolioid from myfin.assetsnapshot 
									where portfolioid=" + folioId + " and dividend>0 and assettype=1 group by month, year;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"select sum(dividend) dividend,month, year from myfin.assetsnapshot 
									where dividend>0 and assettype=1 group by month, year,portfolioid;", _conn);
				}
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					divDetails.Add(new dividend()
					{
						dt = new DateTime(Convert.ToInt32(reader["year"]), Convert.ToInt32(reader["month"]), 1),
						//companyid = reader["isin"].ToString(),
						divValue = Convert.ToDecimal(reader["dividend"])
					});
				}
				
			}
		}
		public void GetYrlyDividend(string companyId, IList<dividend> _eqs)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				//IList<dividend> _eqs = new List<dividend>();
				using var command = new MySqlCommand(@"select sum(Dividend) dividend,ISIN,Year(dtupdated)yr from myfin.dividend where 
							ISIN= '" + companyId + "' and typeofcredit in('IntDividend','FDividend')  group by Year(dtupdated);", _conn);
				using var reader = command.ExecuteReader();
				while (reader.Read())
				{
					_eqs.Add(new dividend()
					{
						dt = new DateTime(Convert.ToInt32(reader["yr"]),1,1),						
						//eqt = new EquityBase { assetId = reader["isin"].ToString() },
						divValue = Convert.ToDecimal(reader["dividend"])
					});
				}
				//return _eqs;
			}
		}
		public void GetYearlyDividend(AssetType assetId, IList<dividend> d, int folioid)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if(folioid==0)
				{
					command = new MySqlCommand(@"SELECT year,assettype, sum(dividend) as dividend FROM myfin.assetsnapshot 
						where assettype=" + (Int16)assetId + " group by year, assettype  order by year asc;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"SELECT year,assettype, sum(dividend) as dividend FROM myfin.assetsnapshot 
						where assettype=" + (Int16)assetId + " and portfolioid=" + folioid + " group by year, assettype  order by year asc;", _conn);
				}
				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					d.Add(new dividend()
					{
						divValue = Convert.ToDecimal(reader["dividend"]),						
						dt = new DateTime(Convert.ToInt32(reader["year"]),1,1)
					});
				}
			}
		}
		public IList<dividend> GetCompanyDividend(string assetId)
		{
			IList<dividend> d = new List<dividend>();
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
						divValue = Convert.ToDecimal(reader["dividend"]),
						creditType = (TranType)Convert.ToInt16(reader["typeOfcredit"]),
						dt = Convert.ToDateTime(reader["dtupdated"])
					});
				}
			}
			return d;
		}
		//public IList<dividend> GetCompanyDividend(string assetId)
		//{
		//	IList<dividend> d =new List<dividend>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();

		//		using var command = new MySqlCommand(@"select * from myfin.dividend
		//					where isin='" + assetId + "';", _conn);
		//		var reader = command.ExecuteReader();

		//		while (reader.Read())
		//		{
		//			d.Add(new dividend()
		//			{
		//				divValue = Convert.ToDouble(reader["dividend"]),
		//				//eqt = new EquityBase { assetId = reader["isin"].ToString() },
		//				dt =Convert.ToDateTime(reader["dtupdated"])
		//			}) ;
		//		}				
		//	}
		//	return d;
		//}
		public void GetNetDividend(IList<CashFlow> d, IList<dividend> div, AssetType astType,int folioId)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (folioId>0)
				{
					command = new MySqlCommand(@"select sum(dividend) dividend,month, year from myfin.assetsnapshot 
					where assettype=" + (int)astType + " and portfolioid=" + folioId + "  group by year, month	order by year desc,month desc;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"select sum(dividend) dividend,month, year from myfin.assetsnapshot 
					where assettype=" + (int)astType + " group by year, month	order by year desc,month desc;", _conn);
				}

				
				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					if (Convert.ToDouble(reader["dividend"]) > 0)
					{
						div.Add(new dividend()
						{
							dt = new DateTime(Convert.ToInt32(reader["year"]), Convert.ToInt32(reader["month"]), 1),
							divValue = Convert.ToDecimal(reader["dividend"])
						});
					}										
				}
			}
		}
		public void GetAssetWiseNetDividend(IList<dividend> div, AssetType asttype, int folioId)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (folioId == 0)
				{
					command = new MySqlCommand(@"select sum(dividend) dividend,month, year from myfin.assetsnapshot 
					where assettype= " + (Int16)asttype + " group by year " +
						"order by year desc,month desc;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"select sum(dividend) dividend,month, year from myfin.assetsnapshot 
					where assettype= " + (Int16)asttype + " and portfolioid="+folioId+" group by year " +
					"order by year desc,month desc;", _conn);
				}
				var reader = command.ExecuteReader();

				while (reader.Read())
				{
					div.Add(new dividend()
					{
						dt = new DateTime(Convert.ToInt32(reader["year"]), Convert.ToInt32(reader["month"]), 1),
						divValue = Convert.ToDecimal(reader["dividend"])
					});

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
							_eq.livePrice = Convert.ToDecimal(reader["liveprice"]);							
						}
						//else
						//	_eq.tranType = (TranType)reader["description"];
					}
				}
			}
			return _eq;

		}
		//public IList<AssetHistory> GetYearlySnapshot(AssetType assetId)
		//{
		//	IList<AssetHistory> assetReturn = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command = null;
		//		if (assetId == (AssetType)0)
		//		{
		//			command = new MySqlCommand(@"SELECT year,month, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
		//						where month in (1,12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))
		//						group by month,year	order by year asc;", _conn);
		//		}
		//		else {
		//			command = new MySqlCommand(@"SELECT year,month,assettype, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
		//						where (month in (12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) and assettype="+(int)assetId+" " +
		//						"group by month,year, assettype  order by year asc; ", _conn);
		//			}
		//		using var reader = command.ExecuteReader();
		//		try
		//		{
		//			while (reader.Read())
		//			{
		//				assetReturn.Add(new AssetHistory()
		//				{
		//					month = Convert.ToInt32(reader["month"]),
		//					//Dividend = Convert.ToDouble(reader["dividend"]),
		//					Investment = Convert.ToDouble(reader["invstmt"]),
		//					AssetValue = Convert.ToDouble(reader["assetvalue"]),
		//					year = Convert.ToInt32(reader["year"])
							
		//				});
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			string s = ex.StackTrace;
		//		}
		//		return assetReturn;
		//	}
		//}
		//public IList<AssetHistory> GetYearlySnapShot(int portfolioId, AssetType assetId, bool YTD)
		//{
		//	IList<AssetHistory> assetReturn = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command = null;
		//		if (YTD)
		//		{
		//			command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
		//					where portfolioid=" + portfolioId + " and assettype=" + (int)assetId + " and year="+ DateTime.Now.Year +" order by month desc limit 1;", _conn);
		//		}
		//		else
		//		{
		//			command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
		//					where portfolioid=" + portfolioId + " and assettype=" + (int)assetId + " and (month in (12) " +
		//					" or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) order by year asc, month asc;", _conn);
		//		}
		//		using var reader = command.ExecuteReader();
		//		try
		//		{
		//			while (reader.Read())
		//			{
		//				assetReturn.Add(new AssetHistory()
		//				{
		//					month = Convert.ToInt32(reader["month"]),
		//					Dividend = Convert.ToDouble(reader["dividend"]),
		//					Investment = Convert.ToDouble(reader["invstmt"]),
		//					AssetValue = Convert.ToDouble(reader["assetvalue"]),
		//					year = Convert.ToInt32(reader["year"]),
		//					portfolioId = Convert.ToInt32(reader["portfolioid"] == null ? 0 : reader["portfolioid"]),
		//					Assettype = (AssetType)((reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"]))
		//				}); ;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			string s = ex.StackTrace;
		//		}
		//		return assetReturn;
		//	}
		//}
		//public void GetCurrentMonthSnapShot(int portfolioId, IList<AssetHistory> assetSnapshot, int month, int year)
		//{
		//	//IList<AssetHistory> assetReturn = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command = null;
				 
		//		command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
		//					where portfolioid="+portfolioId +" " +
		//					"and (month="+month+" AND year="+year+" )" +
		//					"order by year asc, month asc;", _conn);
				
		//		using var reader = command.ExecuteReader();
		//		try
		//		{
		//			while (reader.Read())
		//			{
		//				assetSnapshot.Add(new AssetHistory()
		//				{
		//					month = Convert.ToInt32(reader["month"]),
		//					Dividend = Convert.ToDouble(reader["dividend"]),
		//					Investment = Convert.ToDouble(reader["invstmt"]),
		//					AssetValue = Convert.ToDouble(reader["assetvalue"]),
		//					year = Convert.ToInt32(reader["year"]),
		//					portfolioId = Convert.ToInt32(reader["portfolioid"] == null ? 0 : reader["portfolioid"]),
		//					Assettype = (AssetType)((reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"]))
		//				}); ;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			string s = ex.StackTrace;
		//		}
		//		//return assetReturn;
		//	}
		//}
		public bool ReplaceFolioComment(int folioId, string Comment)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();				
				
				MySqlCommand command = null;
				command = new MySqlCommand(@"UPDATE myfin.portfolio set comment='"+Comment+"' where portfolioid="+folioId+";", _conn);
				int count= command.ExecuteNonQuery();
				if (count > 0)
					return true;
				else
					return false;
			}
		}
		public void GetExpenseType( IList<ExpType> t)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				MySqlCommand command = null;				
				command = new MySqlCommand(@"select * FROM myfin.expensetype;", _conn);				
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						t.Add(new ExpType()
						{							
							expTypeId = Convert.ToInt32(reader["exptypeid"]),
							expTypeDesc = reader["expdesc"].ToString()							
						});
					}
				}
				catch (Exception ex)
				{

					string s = ex.StackTrace;
				}
			}
		}
		public void GetMontlyFolioExpenseHistory(int folioId, IList<MonthlyExpenseDTO> exp, int pastMonth)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				MySqlCommand command = null;
				if (folioId > 0)
				{
					command = new MySqlCommand(@"select sum(amt) totalamt,folioid,DATE_FORMAT(dtoftransaction, '%m-%Y')my from myfin.expense 
							WHERE folioid=" + folioId +	" group by Month(dtoftransaction),year(dtoftransaction),folioid;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"select sum(amt) totalamt,DATE_FORMAT(dtoftransaction, '%m-%Y')my,0 folioid from myfin.expense 
							group by Month(dtoftransaction), year(dtoftransaction);", _conn);
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						exp.Add(new MonthlyExpenseDTO()
						{
							monthYear = reader["my"].ToString(),
							totalExpAmt = Convert.ToDouble(reader["totalamt"]),
							folioId = Convert.ToInt32(reader["folioId"])
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
			}
		}
		public void GetMontlyFolioExpense(int folioId, IList<ExpenseDTO> exp,string my)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				MySqlCommand command = null;
				if (folioId > 0)
				{
					command = new MySqlCommand(@"select e.description,e.exptypeid,e.expid,amt,folioid,DATE_FORMAT(dtoftransaction, '%m-%Y')my,et.* from myfin.expense e 
							join myfin.expensetype et ON et.exptypeid=e.exptypeid
							where DATE_FORMAT(dtoftransaction, '%m-%Y')='" + my+"' AND folioid="+ folioId+";", _conn);
				}
				else
				{
					command = new MySqlCommand(@"select e.description,e.exptypeid,e.expid, amt ,folioid,DATE_FORMAT(dtoftransaction, '%m-%Y')my,et.* from myfin.expense e
								join myfin.expensetype et on et.exptypeid=e.exptypeid
								where DATE_FORMAT(dtoftransaction, '%m-%Y')='" + my+ "';", _conn);
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						exp.Add(new ExpenseDTO()
						{	
							amt= Convert.ToDouble(reader["amt"]),
							expId = Convert.ToInt16(reader["expId"]),
							expenseType = (ExpenseType)Enum.Parse(typeof(ExpenseType),reader["exptypeid"].ToString()),							 
							folioId = Convert.ToInt32(reader["folioId"]),
							desc = reader["description"].ToString()
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
			}
		}
		public void GetFolioExpense(int folioId, IList<ExpenseDTO> exp)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				MySqlCommand command = null;
				if (folioId > 0)
				{
					command = new MySqlCommand(@"select * from myfin.expense e join myfin.expensetype et ON et.exptypeid=e.exptypeid
							where folioid=" + folioId + ";", _conn);
				}
				else
				{
					command = new MySqlCommand(@"select * from myfin.expense e 
									join myfin.expensetype et ON et.exptypeid=e.exptypeid;", _conn);
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						exp.Add(new ExpenseDTO()
						{
							dtOfTran = Convert.ToDateTime(reader["dtOfTransaction"]),
							amt = Convert.ToDouble(reader["Amt"]),
							 folioId= Convert.ToInt32(reader["folioId"]),
							 desc = reader["Description"].ToString()
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
			}
		}
		public bool AddExpenseType(ExpType e)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();

				MySqlCommand command = null;
				command = new MySqlCommand(@"INSERT INTO myfin.expensetype(expDesc) Value('" + e.expTypeDesc + "');", _conn);
				int count = command.ExecuteNonQuery();
				if (count > 0)
					return true;
				else
					return false;
			}
		}
		public bool DeleteExpense(int expId)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
				try
				{
					MySqlCommand command = null;
					
					command = new MySqlCommand(@"DELETE FROM myfin.expense WHERE expid="+expId+"; ", _conn);
					int count = command.ExecuteNonQuery();
					if (count > 0)
						return true;
					else
						return false;
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
		//public bool AddExpense(ExpenseDTO exp)
		//{
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		try
		//		{
		//			MySqlCommand command = null;
		//			string dt = exp.dtOfTran.ToString("yyyy-MM-dd");
		//			command = new MySqlCommand(@"INSERT INTO myfin.expense(dtoftransaction,amt,description,folioid,exptypeId) 
		//		Values('" + dt + "'," + exp.amt + ",'" + exp.desc + "'," + exp.folioId + "," + exp.expenseType.expTypeId + "); ", _conn);
		//			int count = command.ExecuteNonQuery();
		//			if (count > 0)
		//				return true;
		//			else
		//				return false;
		//		}
		//		catch(Exception ex)
		//		{
		//			return false;
		//		}
		//	}
		//}

		public string GetFolioComment(int folioId)
		{
			using (MySqlConnection _conn = new MySqlConnection(connString))
			{
				_conn.Open();
			 
				MySqlCommand command = null;
				if (folioId > 0)
				{
					command = new MySqlCommand(@"SELECT comment FROM myfin.portfolio 
					where portfolioid=" + folioId + ";", _conn);
				}
				else
					return "";
				
				
				return command.ExecuteScalar().ToString();
			}
		}
		//public IList<AssetHistory> GetAssetSnapshot()
		//{
		//	IList<AssetHistory> snapshots = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command = null;
		//		command = new MySqlCommand(@"SELECT month,year, sum(assetvalue)as assetvalue,sum(dividend)as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
		//			group by month,year order by year asc, month asc;", _conn);

		//		using var reader = command.ExecuteReader();
		//		try
		//		{
		//			while (reader.Read())
		//			{
		//				snapshots.Add(new AssetHistory()
		//				{
		//					month = Convert.ToInt32(reader["month"]),
		//					Dividend = Convert.ToDouble(reader["dividend"]),
		//					Investment = Convert.ToDouble(reader["invstmt"]),
		//					AssetValue = Convert.ToDouble(reader["assetvalue"]),
		//					year = Convert.ToInt32(reader["year"]),
		//				}); ;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			string s = ex.StackTrace;
		//		}
		//		return snapshots;
		//	}
		//}
		//public void GetMonthlyAssetSnapshot(int folioId, int astType, IList<AssetHistory> snapshots)
		//{
		//	//IList<AssetHistory> snapshots = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command = null;
		//		command = new MySqlCommand(@"SELECT month,year, sum(assetvalue)as assetvalue,sum(dividend)as dividend ,sum(invstmt) as invstmt, assettype FROM myfin.assetsnapshot 
		//			where assettype="+astType+"" +
		//			" group by month,year,assettype order by year desc, month desc;", _conn);

		//		using var reader = command.ExecuteReader();
		//		try
		//		{
		//			while (reader.Read())
		//			{
		//				snapshots.Add(new AssetHistory()
		//				{
		//					month = Convert.ToInt32(reader["month"]),
		//					Dividend = Convert.ToDouble(reader["dividend"]),
		//					Investment = Convert.ToDouble(reader["invstmt"]),
		//					AssetValue = Convert.ToDouble(reader["assetvalue"]),
		//					year = Convert.ToInt32(reader["year"]),
		//					Assettype = (AssetType)Convert.ToInt32(reader["assettype"])
		//				}); ;
		//			}
		//		}
		//		catch (Exception ex)
		//		{
		//			string s = ex.StackTrace;
		//		}
		//		//return snapshots;
		//	}
		//}
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
							Cashflow = Convert.ToDecimal(reader["Amt"]),
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
							//assetTypeId = type,
							equity= new EquityBase() { assetType=type},
							//tranType = (TranType)reader["description"],
													
							price = Convert.ToDecimal(reader["Amt"]),
							tranDate = Convert.ToDateTime(reader["dtoftransaction"]),
							tranType = (TranType)reader["typeoftransaction"]
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

		//public void GetAssetSnapshot(IList<AssetHistory> astHistroy, int portfolioId, int astType)
		//{
		//	//IList<AssetHistory> snapshots = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command=null;
		//		if (portfolioId == 0)
		//		{					 
		//			command = new MySqlCommand(@"SELECT sum(invstmt) invstmt,sum(assetvalue) assetvalue,sum(dividend) dividend,year,month 
		//					FROM myfin.assetsnapshot where assettype="+ astType +"	" +
		//					"group by year, month order by year asc, month asc;", _conn);					
		//		}
		//		else //if (portfolioId > 0)
		//		{
		//		 command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
		//			where portfolioid=" + portfolioId + " and assettype="+astType+" order by year asc, month asc;", _conn);
		//		}
			 
		//		using var reader = command.ExecuteReader();
		//		try
		//		{
		//			while (reader.Read())
		//			{
		//				astHistroy.Add(new AssetHistory()
		//				{
		//					month = Convert.ToInt32(reader["month"]),
		//					Dividend = Convert.ToDouble(reader["dividend"]),
		//					Investment = Convert.ToDouble(reader["invstmt"]),
		//					AssetValue = Convert.ToDouble(reader["assetvalue"]),
		//					year = Convert.ToInt32(reader["year"]),
		//					portfolioId = portfolioId,
		//					Assettype = (AssetType)astType
		//				}); 
		//			}
		//		}
		//		catch(Exception ex)
		//		{
		//			string s = ex.StackTrace;
		//		}
		//		//return snapshots;
		//	}
		//}

		public void GetPropertyHistoricalValue(IList<AssetHistory> hst)
		{

		}

		
	}
}
