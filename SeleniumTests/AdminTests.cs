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

        [Test]
        public void EditUniversity()
        {
            Utils.Autorise(_webDriver, "user", "password");
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/university/edit");

            var shortName = _webDriver.FindElement(By.Id("short"));
            var shortVal = shortName.GetAttribute("value");
            shortName.Clear();
            shortName.SendKeys("Test");

            var full = _webDriver.FindElement(By.Id("full"));
            var fullVal = full.GetAttribute("value");
            full.Clear();
            full.SendKeys("Full Test");

            var region = new SelectElement(_webDriver.FindElement(By.Id("region")));
            var regionVal = region.SelectedOption.Text;
            region.SelectByText("Кировская область");
            
            var status = new SelectElement(_webDriver.FindElement(By.Id("status")));
            var statusVal = status.SelectedOption.Text;
            status.SelectByText("Частный");

            _webDriver.FindElements(By.TagName("button")).First(e => e.Text == "Сохранить").Click();
            Thread.Sleep(2000);
            
            string univerName = _webDriver.FindElement(By.XPath("//html/body/div[@class='head']/div[@class='headMenus']/div[2]"))
                .Text;
            Assert.AreEqual("Test", univerName);
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/university/edit");
            
            shortName = _webDriver.FindElement(By.Id("short"));
            shortName.Clear();
            shortName.SendKeys(shortVal);
            
            full = _webDriver.FindElement(By.Id("full"));
            full.Clear();
            full.SendKeys(fullVal);
            
            region = new SelectElement(_webDriver.FindElement(By.Id("region")));
            region.SelectByText(regionVal);
            
            status = new SelectElement(_webDriver.FindElement(By.Id("status")));
            status.SelectByText(statusVal);
            
            _webDriver.FindElements(By.TagName("button")).First(e => e.Text == "Сохранить").Click();
            Thread.Sleep(2000);
            
            univerName = _webDriver.FindElement(By.XPath("//html/body/div[@class='head']/div[@class='headMenus']/div[2]"))
                .Text;
            Assert.AreEqual(shortVal, univerName);
        }

        [Test]
        public void AddEditDeleteUser()
        {
            _webDriver.Manage().Window.Maximize();
            
            Utils.Autorise(_webDriver, "admin", "password");
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/user/add");

            _webDriver.FindElement(By.Id("name")).SendKeys("ВременныйПользователь");
            _webDriver.FindElement(By.Id("login")).SendKeys("tempTestUser");
            _webDriver.FindElement(By.Id("password")).SendKeys("password");
            _webDriver.FindElement(By.Id("confirmPassword")).SendKeys("password");
            _webDriver.FindElement(By.Id("email")).SendKeys("test@mail.ru");
            new SelectElement(_webDriver.FindElement(By.Id("permission"))).SelectByValue("2");
            new SelectElement(_webDriver.FindElement(By.Id("university"))).SelectByValue("2");
            
            _webDriver.FindElements(By.TagName("Button")).First(b => b.Text == "Добавить").Click();

            var userRow = _webDriver.FindElements(By.TagName("tr"))
                .FirstOrDefault(tr => tr.FindElements(By.TagName("td")).Count(td => td.Text == "tempTestUser") > 0);
            
            Assert.NotNull(userRow);

            var edit = userRow.FindElement(By.XPath(".//td[6]/img[2]"));
            edit.Click();

            var nameInput = _webDriver.FindElement(By.Id("name"));
            nameInput.Clear();
            nameInput.SendKeys("Отред временный пользователь");
            var emailInput = _webDriver.FindElement(By.Id("email"));
            emailInput.Clear();
            emailInput.SendKeys("testedited@mail.ru");
            _webDriver.FindElement(By.Id("lastPassword")).SendKeys("password");
            _webDriver.FindElement(By.Id("newPassword")).SendKeys("newpassword");

            _webDriver.FindElements(By.TagName("Button")).First(b => b.Text == "Сохранить").Click();

            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/user");
            
            userRow = _webDriver.FindElements(By.TagName("tr"))
                .FirstOrDefault(tr => tr.FindElements(By.TagName("td")).Count(td => td.Text == "tempTestUser") > 0);
            
            var delete = userRow.FindElement(By.XPath(".//td[6]/img[1]"));
            delete.Click();
            _webDriver.SwitchTo().Alert().Accept();
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/user");
            
            var remainingCells = _webDriver.FindElements(By.TagName("td"));
            Assert.IsEmpty(remainingCells.Where(td => td.Text == "tempTestUser"));
        }
    }
}