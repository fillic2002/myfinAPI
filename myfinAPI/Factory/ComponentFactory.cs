﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Business;
using myfinAPI.Data;

namespace myfinAPI.Factory
{
	public static class ComponentFactory
	{
		private static mysqlContext _mysqlComponent;
		private static WebScrapper _webScraperComponent;
		private static PortfoliMgmt _portfolioComponent;
		private static Dashboard _dashboard;
		private static Banking _bankAc;
		private static Transaction _transaction;
		private static AssetSnapshot _snapshot;
		private static Xirr _xirr;
		private static Admin _admin;

		public static Admin GetAdminObj()
		{
			if (_admin == null)
				_admin = new Admin();
			return _admin;
		}
		public static AssetSnapshot GetSnapshotObj()
		{
			if (_snapshot == null)
				_snapshot = new AssetSnapshot();
			return _snapshot;
		}
		public static Xirr GetXirrObject()
		{
			if (_xirr == null)
				_xirr = new Xirr();
			return _xirr;
		}
		public static Transaction GetTranObject()
		{
			if (_transaction == null)
				_transaction = new Transaction();
			return _transaction;
		}
		public static Banking GetBankObject()
		{
			if (_bankAc == null)
				_bankAc = new Banking();
			return _bankAc;
		}
		public static Dashboard GetDashboardObject()
		{
			if (_dashboard == null)
				_dashboard = new Dashboard();
			return _dashboard;
		}
		public static mysqlContext GetMySqlObject()
		{
			if (_mysqlComponent == null)
				_mysqlComponent = new mysqlContext();
			return _mysqlComponent;
		}

		public static WebScrapper GetWebScrapperObject()
		{
			if (_webScraperComponent == null)
				_webScraperComponent = new WebScrapper();
			return _webScraperComponent;
		}
		public static PortfoliMgmt GetPortfolioObject()
		{
			if (_portfolioComponent == null)
				_portfolioComponent = new PortfoliMgmt();
			return _portfolioComponent;
		}


	}
}
