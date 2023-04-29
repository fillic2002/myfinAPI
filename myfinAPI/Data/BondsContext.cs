using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Model.DTO;
using MySqlConnector;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Data
{
	public class BondsContext
	{
		string _connString;
		public BondsContext()
		{
			_connString = "Server = localhost; Database = myfin; Uid = root; Pwd = Welcome@1; ";
		}
		public void GetBondTransaction(int portfolioId, IList<BondTransaction> transactionList)
		{
			try
			{
				using (MySqlConnection _conn = new MySqlConnection(_connString))
				{
					_conn.Open();
					MySqlCommand command;
					if (portfolioId == 0)
					{

						command = new MySqlCommand(@"SELECT * FROM myfin.bondtransaction as bt
								join myfin.bonddetails bd
								on bd.bondid = bt.bondid;", _conn);
					}
					else
					{
						command = new MySqlCommand(@"SELECT * FROM myfin.bondtransaction as bt
								join myfin.bonddetails bd
								on bd.bondid = bt.bondid where folioid=" + portfolioId + ";", _conn);
					}


					using (var reader = command.ExecuteReader())
					{

						while (reader.Read())
						{
							var LivPric = reader["currentprice"].ToString();
							transactionList.Add(new BondTransaction()
							{
								BondDetail = new Bond()
								{
									BondId = reader["bondid"].ToString(),
									BondName = reader["bondName"].ToString(),
									faceValue = (double)reader["facevalue"],
									couponRate = (double)reader["Couponrate"],
									dateOfMaturity = Convert.ToDateTime(reader["dom"]),
									LivePrice = Convert.ToDouble(LivPric==string.Empty?0: reader["currentprice"]),
									intrestCycle = reader["Intrestcycle"].ToString()
								},
								folioId = Convert.ToInt32(reader["folioId"]),
								purchaseDate = Convert.ToDateTime(reader["dateofpurchase"]),
								Qty = Convert.ToDouble(reader["qty"]),
								InvstPrice = Convert.ToDouble(reader["price"]),
								TranType = (TranType)reader["tranType"],
								//AccuredIntrest = Convert.ToDouble(reader["AccurdIntrest"])
							});
						}
					}
				}

			}
			catch (Exception ex)
			{
				string s = ex.Message;
			}
		}
		public void GetBondDetails( IList<Bond> bondDetails)
		{
			try
			{
				using (MySqlConnection _conn = new MySqlConnection(_connString))
				{
					_conn.Open();
					MySqlCommand command;
					command = new MySqlCommand(@"SELECT * FROM myfin.bonddetails order by updatedon desc limit 100;", _conn);
					 
					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							try
							{
								bondDetails.Add(new Bond()
								{
									BondId = reader["bondId"].ToString(),
									couponRate = Convert.ToDouble(reader["couponRate"] == null ? 0 : reader["couponRate"]),
									dateOfMaturity = Convert.ToDateTime(reader["DOM"]),
									YTM = Convert.ToInt32(reader["ytm"] == null ? 0 : reader["ytm"]),
									LivePrice = Convert.ToDouble(reader["currentPrice"] == null ? 0 : reader["currentPrice"]),
									BondName = (reader["bondName"]==null?"": reader["bondName"]).ToString(),
									faceValue = Convert.ToDouble(reader["facevalue"] == null ? 0 : reader["facevalue"]),
									intrestCycle = (reader["intrestCycle"] == null ? "Y" : reader["intrestCycle"]).ToString(),
									updateDate = Convert.ToDateTime(reader["Updatedon"]==null? new DateTime(1001,1,1): reader["Updatedon"])

								});
							}
							catch(Exception e)
							{
								string s = e.Message;
								continue;
							}
						}
					}
				}

			}
			catch (Exception ex)
			{
				string s = ex.Message;
				
			}
		}
		public IList<Bond> SearchBond(Bond bondDetail)
		{
			try
			{
				IList<Bond> bList = new List<Bond>();
				using (MySqlConnection _conn = new MySqlConnection(_connString))
				{
					_conn.Open();
					MySqlCommand command;
					command = new MySqlCommand(@"SELECT * FROM myfin.bonddetails where "+ FormatWhereClause(bondDetail) + " limit 20;", _conn);

					using (var reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							try
							{
								bondDetail = new Bond();
								bondDetail.BondId = reader["bondId"].ToString();
								bondDetail.couponRate = Convert.ToDouble(reader["couponRate"] == null ? 0 : reader["couponRate"]);
								bondDetail.dateOfMaturity = Convert.ToDateTime(reader["DOM"]);
								//bondDetail.YTM = Convert.ToInt32(reader["ytm"] == null ? 0 : reader["ytm"]);
								bondDetail.LivePrice = Convert.ToDouble(reader["currentPrice"] == null ? 0 : reader["currentPrice"]);
								bondDetail.BondName = (reader["bondName"] == null ? "" : reader["bondName"]).ToString();
								bondDetail.faceValue = Convert.ToDouble(reader["facevalue"] == null ? 0 : reader["facevalue"]);
								bondDetail.intrestCycle = (reader["intrestCycle"] == null ? "Y" : reader["intrestCycle"]).ToString();
								bondDetail.updateDate = Convert.ToDateTime(reader["Updatedon"] == null ? new DateTime(1001, 1, 1) : reader["Updatedon"]);
								bList.Add(bondDetail);
							}
							catch (Exception e)
							{
								string s = e.Message;
								continue;
							}
						}
					}
				}
				return bList;
			}
			catch (Exception ex)
			{
				string s = ex.Message;
				return null;
			}
		}

		public bool SaveliveBondDetails(Bond b)
		{
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				try
					{

						string dt = b.dateOfMaturity.ToString("yyyy-MM-dd");
						string dtUpdate = DateTime.UtcNow.ToString("yyyy-MM-dd");
						using var command = new MySqlCommand(@"REPLACE INTO myfin.bonddetails ( bondID,bondname,couponrate,dom, ytm,facevalue,currentprice,updatedon,symbols, intrestcycle,rating,issuer) 
												VALUES ( '" + b.BondId + "','" + b.BondName + "'," + b.couponRate + ",'" + dt + "'," + b.YTM + "," +
												b.faceValue + "," + b.LivePrice + ",'"+ dtUpdate + "','"+b.symbol+"','"+b.intrestCycle+"','"+b.rating+"','"+b.issuer+"');", _conn);
						int result = command.ExecuteNonQuery();
					}
				catch(Exception e)					
				{
					Console.WriteLine(e.Message);
					return false;
					
				}
			}
			
			return true;
		}
		public bool UpdateBondLivePrice(Bond bonds)
		{
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				//foreach (Bond b in bonds)
				//{
					try
					{
						//string dt = b.dateOfMaturity.ToString("yyyy-MM-dd");
						string dtUpdate = DateTime.UtcNow.ToString("yyyy-MM-dd");
						using var command = new MySqlCommand(@"UPDATE myfin.bonddetails 
												SET currentprice= " + bonds.LivePrice + ", updatedon='" + dtUpdate + "', symbols='"+ bonds.symbol+"'" +
												" WHERE bondid= '" + bonds.BondId + "';", _conn);
						int result = command.ExecuteNonQuery();
					}
					catch (Exception e)
					{
						//continue;
					}
				//}
			}
			return true;
		}
		public bool PostBondTransaction(BondTransaction tran)
		{
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				string dt = tran.purchaseDate.ToString("yyyy-MM-dd");
				using var command = new MySqlCommand(@"INSERT INTO myfin.bondtransaction ( bondId, price,qty,dateofpurchase, trantype,folioid) 
												VALUES ( '" + tran.BondDetail.BondId + "'," + tran.InvstPrice + "," + tran.Qty + ",'" + dt + "'," + (int)tran.TranType + "," + tran.folioId + ");", _conn);
				int result = command.ExecuteNonQuery();
			}
			return true;
		}
		public bool AddBondDetails(Bond bondDetails)
		{
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				try
				{
					MySqlCommand command = null;
					string dt = bondDetails.dateOfMaturity.ToString("yyyy-MM-dd");
					command = new MySqlCommand(@"INSERT INTO myfin.bonddetails(Bondid,BondName,CouponRate,DOM,facevalue,minInvst,symbols) 
				Values( '" + bondDetails.BondId + "','" + bondDetails.BondName + "'," + bondDetails.couponRate + ",'" + dt + "'," + bondDetails.faceValue +
						"," + bondDetails.minInvst + ",'" + bondDetails.symbol + "'); ", _conn);
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
		public bool DeleteBondTransaction(BondTransaction bondTran)
		{
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				try
				{
					MySqlCommand command = null;
					string dt = bondTran.purchaseDate.ToString("yyyy-MM-dd");
					command = new MySqlCommand(@"Delete from myfin.bondtransaction where bondid= '" + bondTran.BondDetail.BondId + "' and folioId=" 
						+ bondTran.folioId+ " and dateofpurchase='" + dt + "'; ", _conn);
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

		private string FormatWhereClause(Bond bondDetail)
		{
			string whereClause=string.Empty;
			if (bondDetail.BondId != null)
			{
				if (bondDetail.BondId.StartsWith("IN"))
					whereClause = " bondId like '" + bondDetail.BondId + "%'";
				else if (bondDetail.issuer != null)
				{
					whereClause = " issuer = '" + bondDetail.issuer + "' AND symbols ='" + bondDetail.symbol + "'";
				}
			}else if (bondDetail.BondName!= null)
			{

				if (bondDetail.BondName.StartsWith("IN"))
					whereClause = " bondid like '" + bondDetail.BondName + "%'";
				else
					whereClause = " bondname like '" + bondDetail.BondName+ "%'";				
			}
			return whereClause;
		}
	}
}
