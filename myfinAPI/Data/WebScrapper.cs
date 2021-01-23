using System;
using System.Collections.Generic;
using System.Linq;
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
		public void Dispose()
		{
			_driver.Quit();
		}

		public double GetLivePrice(string assestId)
		{
			if (assestId == null)
				return 0;

			double liveprice = ComponentFactory.GetMySqlObject().GetLivePrice(assestId);
			if (liveprice == 0)
			{
				_driver = new ChromeDriver();
				_driver.Navigate().GoToUrl("https://www.bseindia.com/stock-share-price/itc-ltd/itc/" + assestId);

				liveprice = Convert.ToDouble(_driver.FindElements(By.TagName("strong"))[36].Text);

				ComponentFactory.GetMySqlObject().UpdateLivePrice(assestId, liveprice);

				Dispose();
			}
			return liveprice;
		}
		 
	}
}
