﻿using System;
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

		public async Task<double> GetLivePriceAsync(portfolio folio)
		{
			
			if (folio.symobl == null)
				return 0;
			double liveprice = ComponentFactory.GetMySqlObject().GetLivePrice(folio.EquityId);
			try
			{
				
				if (liveprice == 0 && folio.equityType ==1)
				{
					_driver = new ChromeDriver();
					 _driver.Navigate().GoToUrl(_webScrapperUrl + folio.symobl);
					Thread.Sleep(1000);
					liveprice = Convert.ToDouble(_driver.FindElements(By.Id("quoteLtp"))[0].Text);

					 ComponentFactory.GetMySqlObject().UpdateLivePrice(folio.EquityId, liveprice);

					Dispose();					
				}
				else
				{

				}
			}
			catch(Exception ex)

			{
				string s = ex.Message;
				Dispose();
				
			}
			return liveprice;
		}
		public void GetTransaction()
		{
			_webScrapperUrl = "https://www.moneycontrol.com/portfolio-management/portfolio-investment-dashboard/stock";
			_driver = new ChromeDriver();
			_driver.Navigate().GoToUrl(_webScrapperUrl);			
			var element = _driver.FindElement(By.Id("USER_LOGIN"));
			element.Click();
			_driver.SwitchTo().Frame(_driver.FindElement(By.Id("pwd")));
			//ComponentFactory.GetMySqlObject().UpdateLivePrice(isin, liveprice);

			Dispose();

		}

		 
	}
}
