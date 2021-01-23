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

			using var command = new MySqlCommand(@"SELECT ad.assetname, st.qty, st.action,st.price,ad.ID
									FROM myfin.sharetransaction as st 
									inner join myfin.assetdetail as ad
									on ad.id= st.assetid
									where st.portfolioId=" + portfolioID, connection);
			using var reader = command.ExecuteReader();
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			while (reader.Read())
			{
				tranList.Add(new EquityTransaction()
				{
					equityName= reader["assetname"].ToString(),
					qty= Convert.ToDouble(reader["qty"]),
					tranType= reader["action"].ToString(),
					price= Convert.ToDouble(reader["price"]),
					equityId = reader["ID"].ToString()
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

		public IList<EquityTransaction> getTransaction()
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT *
												FROM myfin.sharetransaction as st
												join myfin.assetdetail ad
												on st.assetid=ad.id;", connection);
			using var reader = command.ExecuteReader();
			IList<EquityTransaction> transactionList = new List<EquityTransaction>();
			while (reader.Read())
			{
				transactionList.Add(new EquityTransaction()
				{
					equityId = reader["assetID"].ToString(),
					portfolioId = Convert.ToInt32(reader["portfolioId"]),
					tranDate = Convert.ToDateTime(reader["transactiondate"]),
					qty = Convert.ToInt32(reader["qty"]),
					price = Convert.ToDouble(reader["price"]),
					equityName = reader["assetName"].ToString(),
					tranType = reader["action"].ToString()
				});
			}
			return transactionList;

		}

		public bool postTransaction(EquityTransaction tran)
		{

			if (connection.State != ConnectionState.Open)
				connection.Open();
			string dt = tran.tranDate.ToString("yyyy-MM-dd");
			using var command = new MySqlCommand(@"INSERT INTO myfin.sharetransaction ( price, action,assetid,qty,portfolioid,transactiondate) 
												VALUES ( "+ tran.price +",'"+tran.tranType+"',"+tran.equityId+","+tran.qty+","+tran.portfolioId+",'"+dt+"');", connection);
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

		public bool UpdateLivePrice(string assetId,double liveprice)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"UPDATE myfin.assetdetail SET liveprice = "+ liveprice +"," +
									" dtupdated ='" + DateTime.UtcNow.ToString("yyyy-MM-dd") + "' WHERE (Id = "+ assetId+");",connection);
			
			int result = command.ExecuteNonQuery();

			return true;
		}
		public IList<DashboardDetail> GetShareAndMFDetails(IList<DashboardDetail> assetTypeList)
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"select SUM(st.qty*st.price) as total,att.Name
													from myfin.sharetransaction as st
													join myfin.assetdetail ad
													on ad.id=st.assetID
													Join myfin.assettype att
													on att.idAssetType=ad.assetTypeID
													group by ad.assettypeid;
													 ", connection);
			using var reader = command.ExecuteReader();
		 
			while (reader.Read())
			{
				assetTypeList.Add(new DashboardDetail()
				{
					total = Convert.ToDouble(reader["total"]),
					assetName = reader["Name"].ToString()

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
											FROM myfin.assetdetail att
											where att.id="+assetId+";", connection);

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
