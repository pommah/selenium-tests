using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class AdminTests
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
        public void AddAndDeleteUniversity()
        {
            Utils.Autorise(_webDriver, "admin", "password");

            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/university/add");
            
            var regionSelect = new SelectElement(_webDriver.FindElement(By.Id("region")));
            regionSelect.SelectByText("Рязанская область");

            var fullName = _webDriver.FindElement(By.Id("fullName"));
            fullName.SendKeys("Временный тестовый унивеситет");

            var shortName = _webDriver.FindElement(By.Id("shortName"));
            shortName.SendKeys("ВТУ");
            
            var statusSelect = new SelectElement(_webDriver.FindElement(By.Id("status")));
            statusSelect.SelectByText("Частный");

            var submit = _webDriver.FindElements(By.TagName("Button")).First(b => b.Text == "Добавить");
            submit.Click();
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/university/index/region/62");

            var newUniverRow = _webDriver.FindElements(By.TagName("tr")).
                FirstOrDefault(tr => tr.FindElements(By.TagName("td")).Count( el => el.Text == "Временный тестовый унивеситет") > 0);
            
            Assert.NotNull(newUniverRow);

            var deleteButton = newUniverRow.FindElement(By.TagName("img"));
            deleteButton.Click();

            _webDriver.SwitchTo().Alert().Accept();

            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/university/index/region/62");
            
            var remainingCells = _webDriver.FindElements(By.TagName("td"));
            Assert.IsEmpty(remainingCells.Where(td => td.Text == "Временный тестовый унивеситет"));
        }
        
    }
}