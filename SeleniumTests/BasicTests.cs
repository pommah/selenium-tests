using System;
using NUnit.Framework;
using OpenQA.Selenium;

namespace SeleniumTests
{
    [TestFixture]
    public class BasicTests
    {       
        private IWebDriver _webDriver;

        [SetUp]
        public void SetupTest()
        {
            _webDriver = Utils.Driver;
            _webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl);
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                _webDriver.Close();
            }
            finally
            {
            }
        }

        [Test]
        public void TestSiteOpen()
        {
            Assert.NotNull(_webDriver.FindElement(By.Id("login")));
        }

        [Test]
        [TestCase("medRzn", "password", "medRzn")]
        [TestCase("admin", "password", "Администратор")]
        [TestCase("minobr", "password", "Министерство Образования")]
        public void TestAutorisation(string login, string password, string expectedName)
        {
            Utils.Autorise(_webDriver, login, password);
            
            string userName = _webDriver.FindElement(By.XPath("//html/body/div[@class='head']/div[@class='headMenus']/div[1]"))
                .Text;
            Assert.AreEqual(expectedName, userName);
        }



        [Test]
        public void TestSessionDestroying()
        {
            Utils.Autorise(_webDriver, "medRzn", "password");
            
            var exitLink =
                _webDriver.FindElement(By.XPath("//html/body/div[@class='head']/div[@class='headMenus']/div[3]"));
            
            exitLink.Click();
            exitLink.Click(); //¯\_(ツ)_/¯
            
            Assert.NotNull(_webDriver.FindElement(By.Id("login")));
        }
        
        
    }
}