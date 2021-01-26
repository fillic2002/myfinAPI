using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace myfinAPI.Data
{
	public class WebScrapper: IDisposable
	{
		IWebDriver _driver;
		string _webScrapperUrl = "https://www.nseindia.com/get-quotes/equity?symbol=";
		public void Dispose()
		{
			_driver.Quit();
		}

		public async Task<double> GetLivePriceAsync(string symbol, string isin)
		{
			
			if (symbol == null)
				return 0;
			double liveprice = ComponentFactory.GetMySqlObject().GetLivePrice(isin);
			try
			{
				
				if (liveprice == 0)
				{
					_driver = new ChromeDriver();
					 _driver.Navigate().GoToUrl(_webScrapperUrl + symbol);
					Thread.Sleep(1000);
					liveprice = Convert.ToDouble(_driver.FindElements(By.Id("quoteLtp"))[0].Text);

					 ComponentFactory.GetMySqlObject().UpdateLivePrice(isin, liveprice);

					Dispose();
					
				}
			}
			catch(Exception ex)
			{
				string s = ex.Message;
				Dispose();
				
			}
			return liveprice;
		}
		 
	}
}
