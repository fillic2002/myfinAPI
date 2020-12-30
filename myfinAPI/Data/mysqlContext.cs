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

			using var command = new MySqlCommand(@"SELECT ad.assetname, st.qty, st.action
									FROM myfin.sharetransaction as st 
									inner join myfin.assetdetail as ad
									on ad.id= st.assetid
									where st.portfolioId="+ portfolioID, connection);
			using var reader = command.ExecuteReader();
			IList<EquityTransaction> tranList = new List<EquityTransaction>();
			while (reader.Read())
			{
				tranList.Add(new EquityTransaction()
				{
					equityName= reader["assetname"].ToString(),
					qty= Convert.ToDouble(reader["qty"]),
					tranType= reader["action"].ToString()
				});
			}
			return tranList;

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
					equityId = Convert.ToInt32(reader["assetID"]),
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
			string dt = tran.tranDate.ToString("yyyy-mm-dd");
			using var command = new MySqlCommand(@"INSERT INTO myfin.sharetransaction ( price, action,assetid,qty,portfolioid,transactiondate) 
												VALUES ( 4.5, 'B',"+tran.equityId+","+tran.qty+","+tran.portfolioId+",'"+dt+"');", connection);
			int result = command.ExecuteNonQuery();
			
			return true;

		}

		public IList<DashboardDetail> GetDashboardDetail()
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
			IList<DashboardDetail> assetTypeList = new List<DashboardDetail>();
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
	}
}
