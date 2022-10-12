using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Model;
using MySqlConnector;
using static myfinAPI.Model.AssetClass;

namespace myfinAPI.Data
{
	public class AssetSnapshot
	{
		string _connString;
		public AssetSnapshot()
		{
			_connString = "Server = localhost; Database = myfin; Uid = root; Pwd = Welcome@1; ";
		}
		/// <summary>
		/// Portfolio wise Month snapshot
		/// </summary>
		/// <param name="portfolioId"></param>
		/// <param name="assetSnapshot"></param>
		/// <param name="month"></param>
		/// <param name="year"></param>
		public void GetMonthSnapShot(int portfolioId, IList<AssetHistory> assetSnapshot, int month, int year)
		{
		
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;

				command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid=" + portfolioId + " " +
							"and (month=" + month + " AND year=" + year + " )" +
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
							Assettype = (AssetType)((reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"]))
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
			 
			}
		}
		public void GetAssetWiseMonthSnapShot(int portfolioId, IList<AssetHistory> assetSnapshot, int month, int year, AssetType assetType)
		{

			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (portfolioId > 0)
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid=" + portfolioId + " AND and assettype=" + (int)assetType + " " +
								"and (month=" + month + " AND year=" + year + " )" +
								"order by year asc, month asc;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where assettype=" + (int)assetType + " " +
								"and (month=" + month + " AND year=" + year + " )" +
								"order by year asc, month asc;", _conn);
				}

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
							Assettype = (AssetType)((reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"]))
						}); ;
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}

			}
		}
	}
}
