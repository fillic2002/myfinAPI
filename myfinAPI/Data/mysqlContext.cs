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
		MySqlConnection connection = new MySqlConnection("Server = localhost; Database = myfin; Uid = root; Pwd = Welcome@1; ");
		public IList<ShareInfo> getShare()
		{
			if(connection.State!= ConnectionState.Open) 
				connection.Open();

			using var command = new MySqlCommand("SELECT * FROM myfin.shareinfo;", connection);
			using var reader = command.ExecuteReader();
			IList<ShareInfo> sharesList = new List<ShareInfo>();
			while (reader.Read())
			{
				sharesList.Add(new ShareInfo()
				{
					id = Convert.ToInt32(reader.GetValue(0)),
					fullName = reader.GetValue(1).ToString(),
					shortName = reader.GetValue(2).ToString()
				});						
			}
			return sharesList;
			
		}

		public IList<EquityTransaction> getPortfolio(int portfolioID)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT ed.name, st.qty, st.action,st.price,ed.symbol,ed.ISIN
						FROM myfin.equitytransactions as st 
						inner join myfin.equitydetails as ed
						on ed.ISIN= st.assetid
						where st.portfolioId=" + portfolioID, connection);
			using var reader = command.ExecuteReader();
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			while (reader.Read())
			{
				tranList.Add(new EquityTransaction()
				{
					equityName= reader["name"].ToString(),
					qty= Convert.ToDouble(reader["qty"]),
					tranType= reader["action"].ToString(),
					price= Convert.ToDouble(reader["price"]),
					equityId = reader["ISIN"].ToString(),
					symbol = reader["symbol"].ToString()
				});
			}
			return tranList;

		}

		public IList<portfolio> getUserfolio()
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT folioname, portfolioid FROM myfin.portfolio;" , connection);
			using var reader = command.ExecuteReader();
			IList<portfolio> folioList = new List<portfolio>();
			while (reader.Read())
			{
				folioList.Add(new portfolio()
				{
					folioName = reader["folioname"].ToString(),				 
					folioID = (int)reader["portfolioid"]
				});
			}
			return folioList;

		}

		public IList<EquityTransaction> getTransaction(int portfolioId)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT *
								FROM myfin.equitytransactions as et
								join myfin.equitydetails ed
								on et.assetid=ed.isin Where et.portfolioid=" + portfolioId+";", connection);
			using var reader = command.ExecuteReader();
			IList<EquityTransaction> transactionList = new List<EquityTransaction>();
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
			return transactionList;

		}

		public bool postEquityTransaction(EquityTransaction tran)
		{

			if (connection.State != ConnectionState.Open)
				connection.Open();
			string dt = tran.tranDate.ToString("yyyy-MM-dd");
			using var command = new MySqlCommand(@"INSERT INTO myfin.equitytransactions ( price, action,assetid,qty,portfolioid,transactiondate) 
												VALUES ( "+ tran.price +",'"+tran.tranType+"','"+tran.equityId+"',"+tran.qty+","+tran.portfolioId+",'"+dt+"');", connection);
			int result = command.ExecuteNonQuery();
			
			return true;
		}

		public bool postGoldTransaction(EquityTransaction tran)
		{

			if (connection.State != ConnectionState.Open)
				connection.Open();
			string dt = tran.tranDate.ToString("yyyy-MM-dd");
			using var command = new MySqlCommand(@"INSERT INTO myfin.propertytransaction ( assettype, dtupdated,assetvalue,qty,portfolioid) 
												VALUES ( " + tran.typeAsset + ",'" + dt + "','" + tran.price+ "'," + tran.qty + "," + tran.portfolioId + ");", connection);
			int result = command.ExecuteNonQuery();

			return true;
		}

		public bool postBankTransaction(BankDetail tran)
		{

			if (connection.State != ConnectionState.Open)
				connection.Open();
			string dt = tran.transactionDate.ToString("yyyy-MM-dd");
			using var command = new MySqlCommand(@"INSERT INTO myfin.bankdetail ( amt, useracctid,roi,datetotransaction,userid) 
												VALUES ( " + tran.amt+ ",'" + tran.acctId+ "'," + tran.roi + ",'" + dt + "',"+tran.userid+");", connection);
			int result = command.ExecuteNonQuery();

			return true;

		}

		public bool UpdateLivePrice(string isin,double liveprice)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"UPDATE myfin.equitydetails SET liveprice = "+ liveprice +"," +
									" dtupdated ='" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "' WHERE (ISIN = '"+ isin+"');",connection);
			
			int result = command.ExecuteNonQuery();

			return true;
		}
		public IList<DashboardDetail> GetShareAndMFDetails(IList<DashboardDetail> assetTypeList)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"select SUM(et.qty*et.price) as total,ed.Name
										from myfin.equitytransactions as et
										join myfin.equitydetails ed
										on ed.isin=et.assetID;", connection);
			using var reader = command.ExecuteReader();
		 
			while (reader.Read())
			{
				assetTypeList.Add(new DashboardDetail()
				{
					total = Convert.ToDouble(reader["total"]),
					assetName = "Shares"

				});
			}
			return assetTypeList;
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

		public TotalBankAsset GetBankAssetTotal()
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT SUM(amt) FROM myfin.bankdetail WHERE isActive=1;", connection);
			 

			TotalBankAsset assetTypeList = new TotalBankAsset();

			var result= command.ExecuteScalar();
			assetTypeList.amt = Convert.ToDouble(result);

			return assetTypeList;
		}

		public IList<BankDetail> GetBankAssetDetail()
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"select * from myfin.bankdetail bd
													join myfin.bankaccountinfo ba
													ON bd.useracctid=ba.accttypeid
													where isactive=1;", connection);


			IList<BankDetail> assetTypeList = new List<BankDetail>();

			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				assetTypeList.Add(new BankDetail()
				{
					acctName = reader["Bank Name"].ToString(),
					acctId = (int)reader["useracctid"],
					acctType= reader["assettype"].ToString(),
					amt= Convert.ToDouble(reader["amt"]),
					roi= Convert.ToDouble(reader["roi"]),
					transactionDate= Convert.ToDateTime(reader["datetotransaction"])
				});
			}
			return assetTypeList;
		}

		public double GetLivePrice (string assetId)
		{
			double currentPrice = 0;
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT dtUpdated,liveprice
											FROM myfin.equitydetails ed
											where ed.isin='" + assetId+"';", connection);

			using var reader = command.ExecuteReader();

			while (reader.Read())
			{
				var dt = reader["dtUpdated"];
				if (dt != DBNull.Value)
				{
					if (Convert.ToDateTime(dt).Date == DateTime.Today)
					{
						var res = reader["liveprice"];
						//if (res != DBNull.Value)
						currentPrice = Convert.ToDouble(res);
					}
				}
			}
			return currentPrice;

		}
	}
}
