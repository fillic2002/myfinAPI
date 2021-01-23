using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Data;

namespace myfinAPI.Factory
{
	public static class ComponentFactory
	{
		private static mysqlContext _mysqlComponent;
		private static WebScrapper _webScraperComponent;

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


	}
}
