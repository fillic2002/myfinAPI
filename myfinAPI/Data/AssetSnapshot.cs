using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Model;
using myfinAPI.Model.Domain;
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
						Dividend = Convert.ToDecimal(reader["dividend"]),
						Investment = Convert.ToDecimal(reader["invstmt"]),
						AssetValue = Convert.ToDecimal(reader["assetvalue"]),
						year = Convert.ToInt32(reader["year"]),
						portfolioId = Convert.ToInt32(reader["portfolioid"] == null ? 0 : reader["portfolioid"]),
						Assettype = (AssetType)((reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"]))
						}); 
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
							where portfolioid=" + portfolioId + " AND assettype=" + (int)assetType + " " +
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
							Dividend = Convert.ToDecimal(reader["dividend"]),
							Investment = Convert.ToDecimal(reader["invstmt"]),
							AssetValue = Convert.ToDecimal(reader["assetvalue"]),
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
		public void GetAssetSnapshot(IList<AssetHistory> astHistroy, int portfolioId, int astType)
		{
			//IList<AssetHistory> snapshots = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (portfolioId == 0)
				{
					command = new MySqlCommand(@"SELECT sum(invstmt) invstmt,sum(assetvalue) assetvalue,sum(dividend) dividend,year,month 
							FROM myfin.assetsnapshot where assettype=" + astType + "	" +
							"group by year, month order by year asc, month asc;", _conn);
				}
				else //if (portfolioId > 0)
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
					where portfolioid=" + portfolioId + " and assettype=" + astType + " order by year asc, month asc;", _conn);
				}

				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						astHistroy.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDecimal(reader["dividend"]),
							Investment = Convert.ToDecimal(reader["invstmt"]),
							AssetValue = Convert.ToDecimal(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
							portfolioId = portfolioId,
							Assettype = (AssetType)astType
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				//return snapshots;
			}
		}
		public void GetMonthlyAssetSnapshot(int folioId, AssetType astType, IList<AssetHistory> snapshots)
		{
			//IList<AssetHistory> snapshots = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (folioId == 0)
				{
					command = new MySqlCommand(@"SELECT month,year, sum(assetvalue)as assetvalue,sum(dividend)as dividend ,sum(invstmt) as invstmt, assettype FROM myfin.assetsnapshot 
					where assettype=" + (int)astType + "" +
						" group by month,year,assettype order by year desc, month desc;", _conn);
				}
				else {
					command = new MySqlCommand(@"SELECT month,year, sum(assetvalue)as assetvalue,sum(dividend)as dividend ,sum(invstmt) as invstmt, assettype FROM myfin.assetsnapshot 
					where assettype=" + (int)astType + " and portfolioid= "+folioId+" group by month,year,assettype order by year desc, month desc;", _conn);
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						snapshots.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDecimal(reader["dividend"]),
							Investment = Convert.ToDecimal(reader["invstmt"]),
							AssetValue = Convert.ToDecimal(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
							Assettype = (AssetType)Convert.ToInt32(reader["assettype"]),
							portfolioId = folioId
						}); 
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				//return snapshots;
			}
		}
		//public void GetCurrentMonthSnapShot(int portfolioId, IList<AssetHistory> assetSnapshot, int month, int year)
		//{
		//	//IList<AssetHistory> assetReturn = new List<AssetHistory>();
		//	using (MySqlConnection _conn = new MySqlConnection(_connString))
		//	{
		//		_conn.Open();
		//		MySqlCommand command = null;

		//		command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
		//					where portfolioid=" + portfolioId + " " +
		//					"and (month=" + month + " AND year=" + year + " )" +
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
		public IList<AssetHistory> GetYearlySnapShot(int portfolioId, AssetType assetId, bool YTD)
		{
			IList<AssetHistory> assetReturn = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (YTD)
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid=" + portfolioId + " and assettype=" + (int)assetId + " and year=" + DateTime.Now.Year + " order by month desc limit 1;", _conn);
				}
				else
				{
					command = new MySqlCommand(@"SELECT * FROM myfin.assetsnapshot 
							where portfolioid=" + portfolioId + " and assettype=" + (int)assetId + " and (month in (12) " +
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
							Dividend = Convert.ToDecimal(reader["dividend"]),
							Investment = Convert.ToDecimal(reader["invstmt"]),
							AssetValue = Convert.ToDecimal(reader["assetvalue"]),
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
				return assetReturn;
			}
		}
		public IList<AssetHistory> GetYearlySnapShot(AssetType assetId, int folioId)
		{
			IList<AssetHistory> assetReturn = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (assetId == (AssetType)0)
				{
					if (folioId > 0)
					{
						command = new MySqlCommand(@"SELECT year,month, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
								where portfolioid=" + folioId + " AND (month in (1,12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) " +
									"group by month,year order by year asc;", _conn);
					}
					else
					{
						command = new MySqlCommand(@"SELECT year,month, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
								where month in (1,12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE())) " +
									"group by month,year order by year asc;", _conn);
					}					
				}
				else
				{
					if (folioId == 0)
					{
						command = new MySqlCommand(@"SELECT year,month,assettype, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
								where (month in (12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) and assettype=" + (int)assetId + " " +
									"group by month,year, assettype  order by year asc; ", _conn);
					}
					else
					{
						command = new MySqlCommand(@"SELECT year,month,assettype, sum(assetvalue) as assetvalue,sum(dividend) as dividend ,sum(invstmt) as invstmt FROM myfin.assetsnapshot 
								where portfolioid="+ folioId +" AND (month in (12) or (month=MONTH(CURRENT_DATE()) AND year=year(CURRENT_DATE()))) and assettype=" + (int)assetId + " " +
									"group by month,year, assettype  order by year asc; ", _conn);
					}
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						assetReturn.Add(new AssetHistory()
						{
							month = Convert.ToInt32(reader["month"]),
							Dividend = Convert.ToDecimal(reader["dividend"]),
							Investment = Convert.ToDecimal(reader["invstmt"]),
							AssetValue = Convert.ToDecimal(reader["assetvalue"]),
							year = Convert.ToInt32(reader["year"]),
							Assettype = (AssetType)((reader["assettype"] == null) ? 0 : Convert.ToInt32(reader["assettype"])),
							portfolioId=folioId
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				return assetReturn;
			}
		}
		public IList<AssetHistory> GetAssetSnapshot()
		{
			IList<AssetHistory> snapshots = new List<AssetHistory>();
			using (MySqlConnection _conn = new MySqlConnection(_connString))
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
							Dividend = Convert.ToDecimal(reader["dividend"]),
							Investment = Convert.ToDecimal(reader["invstmt"]),
							AssetValue = Convert.ToDecimal(reader["assetvalue"]),
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

		public void GetSectorWiseSnapshot(IList<SectorAssetDistribution> snapshots, int folioId)
		{
			//IList<SectorAssetDistribution> snapshots = new List<SectorAssetDistribution>();
			using (MySqlConnection _conn = new MySqlConnection(_connString))
			{
				_conn.Open();
				MySqlCommand command = null;
				if (folioId == 0)
				{ 
				command = new MySqlCommand(@"SELECT SUM(et.qty * et.price) AS tran, action, ed.sector, et.isin,
						SUM( CASE WHEN action=1 THEN  et.qty * ed.liveprice
								  WHEN action=2 THEN -et.qty*ed.liveprice	
                                  WHEN action = 5 THEN et.qty*ed.liveprice ELSE et.qty*ed.liveprice END) AS current,
							SUM(  CASE WHEN action = 1 THEN et.qty * et.price WHEN action = 2 THEN -et.qty * et.price
			            WHEN action = 5 THEN et.qty * et.price  ELSE et.qty * et.price  END) AS NetInvest
						FROM myfin.equitytransactions et JOIN myfin.equitydetails ed ON ed.isin = et.isin    
						where ed.assettypeid=1	GROUP BY ed.sector;  ", _conn);
				}
				else
				{
					command = new MySqlCommand(@"SELECT SUM(et.qty * et.price) AS tran, action, ed.sector, et.isin,
						SUM( CASE WHEN action=1 THEN  et.qty * ed.liveprice
								  WHEN action=2 THEN -et.qty*ed.liveprice	
                                  WHEN action = 5 THEN et.qty*ed.liveprice ELSE et.qty*ed.liveprice END) AS current,
							SUM(  CASE WHEN action = 1 THEN et.qty * et.price WHEN action = 2 THEN -et.qty * et.price
			            WHEN action = 5 THEN et.qty * et.price  ELSE et.qty * et.price  END) AS NetInvest
						FROM myfin.equitytransactions et JOIN myfin.equitydetails ed ON ed.isin = et.isin    
						where ed.assettypeid=1 AND et.portfolioId="+ folioId +"	GROUP BY ed.sector;  ", _conn);
				}
				using var reader = command.ExecuteReader();
				try
				{
					while (reader.Read())
					{
						snapshots.Add(new SectorAssetDistribution()
						{
							SectorName = reader["sector"].ToString(),
							CurrentValue = Convert.ToDecimal(reader["current"]),
							Invested = Convert.ToDecimal(reader["NetInvest"]),							
						});
					}
				}
				catch (Exception ex)
				{
					string s = ex.StackTrace;
				}
				//return snapshots;
			}

		}

	}
}
