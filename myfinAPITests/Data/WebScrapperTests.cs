using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit.Sdk;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace myfinAPI.Data.Tests
{
	[TestClass()]
	public class WebScrapperTests
	{
		[TestMethod()]
		public void GetTransactionTest()
		{
			WebScrapper obj = new WebScrapper();
			obj.GetTransaction();
			Assert.Fail();
		}
	}
}