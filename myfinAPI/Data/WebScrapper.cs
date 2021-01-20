using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using myfinAPI.Model;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace myfinAPI.Data
{
	public class WebScrapper
	{
		public void GetLivePrice(IList<ShareInfo> listofShare)
		{
			IList<ShareInfo> response = new List<ShareInfo>();
			IWebDriver driver = new ChromeDriver();
			foreach(ShareInfo item in listofShare)
			{
				driver.Navigate().GoToUrl("https://www.bseindia.com/stock-share-price/itc-ltd/itc/500875/");
				item.livePrice = driver.FindElements(By.TagName("strong"))[36].Text;
			}
			driver.Close();
		}
	}
}
