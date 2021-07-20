using System;
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
		private static Portfolio _portfolioComponent;

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
		public static Portfolio GetPortfolioObject()
		{
			if (_portfolioComponent == null)
				_portfolioComponent = new Portfolio();
			return _portfolioComponent;
		}


	}
}
