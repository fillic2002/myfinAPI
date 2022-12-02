using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using myfinAPI.Factory;
using myfinAPI.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
/// <summary>
/// Purpose of this Class should be moved to Sandbox as any live webscrapping should be a backend process
/// </summary>
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

		public double GetLivePriceAsync(portfolio folio)
		{
			
			if (folio.eq.symbol == null)
				return 0;
			EquityBase _eq = new EquityBase();
			_eq=ComponentFactory.GetMySqlObject().GetLivePrice(folio.eq.assetId);
			try
			{

				if (_eq.livePrice == 0)
				{
					//_driver = new ChromeDriver();
					//if (folio.equityType == 1)			
					//{
						
					//	_driver.Navigate().GoToUrl(_webScrapperUrl + folio.symobl);
					//	Thread.Sleep(1500);
					//	_eq.livePrice = Convert.ToDouble(_driver.FindElements(By.Id("quoteLtp"))[0].Text);						
					//}
				 // else
					//{	
					//	_driver.Navigate().GoToUrl(_eq.desctiption);
					//	Thread.Sleep(1000);
					//	//_eq.livePrice = Convert.ToDouble(
					//	_eq.livePrice=Convert.ToDouble(_driver.FindElements(By.ClassName("amt"))[0].Text.Substring(1));
						 
					//}
					//ComponentFactory.GetMySqlObject().UpdateLivePrice(folio.EquityId, _eq.livePrice);
					//Dispose();
				}
				
			}
			catch(Exception ex)

			{
				string s = ex.Message;
				Dispose();
				
			}
			return _eq.livePrice;
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

		public void GetEqtDetail(EquityTransaction tran)
		{

		}



	}
}
