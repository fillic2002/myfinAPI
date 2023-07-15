using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Model.DTO;
using MySqlConnector;

namespace myfinAPI.Data
{
	public class Admin
	{
		string _connString;

		public Admin()
		{
			  _connString= "Server = localhost; Database = myfin; Uid = root; Pwd = Welcome@1; ";
		}
		//public bool AddBondDetails(Bond bondDetails)
		//{
		//	using (MySqlConnection _conn = new MySqlConnection(_connString))
		//	{
		//		_conn.Open();
		//		try
		//		{
		//			MySqlCommand command = null;
		//			string dt = bondDetails.dateOfMaturity.ToString("yyyy-MM-dd");
		//			command = new MySqlCommand(@"INSERT INTO myfin.bonddetails(Bondid,BondName,CouponRate,DOM,facevalue,minInvst,symbols) 
		//		Values( '"+ bondDetails.BondId+"','" +bondDetails.BondName + "'," + bondDetails.couponRate + ",'" + dt+ "'," + bondDetails.faceValue + 
		//				"," + bondDetails.minInvst+",'"+bondDetails.symbol +"'); ", _conn);
		//			int count = command.ExecuteNonQuery();
		//			if (count > 0)
		//				return true;
		//			else
		//				return false;
		//		}
		//		catch (Exception ex)
		//		{
		//			return false;
		//		}
		//	}
		//}
		public bool AddExpense(ExpenseDTO exp)
		{
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				try
				{
					MySqlCommand command = null;
					string dt = exp.dtOfTran.ToString("yyyy-MM-dd");
					command = new MySqlCommand(@"INSERT INTO myfin.expense(dtoftransaction,amt,description,folioid,exptypeId) 
				Values('" + dt + "'," + exp.amt + ",'" + exp.desc + "'," + exp.folioId + "," + (int)exp.expenseType + "); ", _conn);
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
	}
}
