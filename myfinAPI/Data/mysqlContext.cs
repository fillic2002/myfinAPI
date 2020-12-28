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

		public IList<portfolio> getPortfolio()
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT si.shortname, SUM(st.qty) 
												FROM myfin.sharetransaction as st 
												inner join myfin.shareinfo as si
												on si.idshareinfo = st.shareid
												group by st.shareid; ", connection);
			using var reader = command.ExecuteReader();
			IList<portfolio> equityList = new List<portfolio>();
			while (reader.Read())
			{
				equityList.Add(new portfolio()
				{
					equityname= reader.GetValue(0).ToString(),
					 avgprice = Convert.ToDouble(reader.GetValue(1).ToString())
					
				});
			}
			return equityList;

		}
		public IList<EquityTransaction> getTransaction()
		{
			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"SELECT *
												FROM myfin.sharetransaction as st
												join myfin.shareinfo si
												on st.shareId=si.idShareInfo;", connection);
			using var reader = command.ExecuteReader();
			IList<EquityTransaction> transactionList = new List<EquityTransaction>();
			while (reader.Read())
			{
				transactionList.Add(new EquityTransaction()
				{
					equityId = Convert.ToInt32(reader["shareID"]),
					portfolioId = Convert.ToInt32(reader["portfolioId"]),
					tranDate= Convert.ToDateTime(reader["transactiondate"]),
					qty = Convert.ToInt32(reader["qty"]),
					price= Convert.ToDouble(reader["price"]),
					equityName = reader["shortname"].ToString(),

				});
			}
			return transactionList;

		}

		public bool postTransaction(EquityTransaction tran)
		{

			if (connection.State != ConnectionState.Open)
				connection.Open();

			using var command = new MySqlCommand(@"INSERT INTO myfin.sharetransaction (transactionid, price, action,shareid,qty,portfolioid) 
												VALUES (123, 4.5, 'B',"+tran.equityId+","+tran.qty+","+tran.portfolioId+");", connection);
			int result = command.ExecuteNonQuery();
			
			return true;

		}
	}
}
