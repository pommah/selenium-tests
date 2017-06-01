using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace SeleniumTests
{
    [TestFixture]
    public class UserTests
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
        public void AddRemoveDirections()
        {
            Utils.Autorise(_webDriver, "user", "password");
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/direction/edit_all");

            var checkBoxes = _webDriver.FindElements(By.TagName("input")).Where(
                i => i.GetAttribute("type") == "checkbox"
                     && !i.Selected).Take(5).ToList();

            var ids = checkBoxes.Select(c => c.GetAttribute("id")).ToList();

            foreach (var checkBox in checkBoxes)
            {
                checkBox.Click();
            }

            _webDriver.FindElements(By.TagName("button")).First(b => b.Text == "Сохранить").Click();
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/direction/edit_all");
            foreach (var id in ids)
            {
                var checkbox = _webDriver.FindElement(By.Id(id));
                Assert.True(checkbox.Selected);
                checkbox.Click();
            }
            
            _webDriver.FindElements(By.TagName("button")).First(b => b.Text == "Сохранить").Click();
        }

        [Test]
        public void AddAndDeleteStudentWithExistingProgram()
        {
            Utils.Autorise(_webDriver, "user", "password");
            
            _webDriver.Navigate().GoToUrl(Utils.SiteUrl + "/student/add");

            _webDriver.FindElement(By.Id("fio")).SendKeys("Иванов Иван Иванович");
            new SelectElement(_webDriver.FindElement(By.Id("noz_group"))).SelectByText("Нарушение слуха");
            _webDriver.FindElement(By.Id("begin")).SendKeys(DateTime.Now.Subtract(TimeSpan.FromDays(365 * 4 + 2)).ToString("yyyy-MM-dd"));
            _webDriver.FindElement(By.Id("end")).SendKeys(DateTime.Now.Subtract(TimeSpan.FromDays(1)).ToString("yyyy-MM-dd"));

            foreach (var checkbox in _webDriver.FindElements(By.TagName("input")).Where(i => i.Displayed && i.GetAttribute("type") == "checkbox"))
            {
                checkbox.Click();
            }
            
            var filePath = "/tmp/test.pdf";
            File.WriteAllText(filePath, "content");
            
            _webDriver.FindElement(By.Id("fileNameReability")).SendKeys(filePath);
            _webDriver.FindElement(By.Id("fileNamePsycho")).SendKeys(filePath);
            _webDriver.FindElement(By.Id("fileNameCareer")).SendKeys(filePath);
            _webDriver.FindElement(By.Id("fileNameEmployment")).SendKeys(filePath);
            _webDriver.FindElement(By.Id("fileNameDistance")).SendKeys(filePath);
            _webDriver.FindElement(By.Id("fileNamePortfolio")).SendKeys(filePath);
            
            var programSelector = new SelectElement(_webDriver.FindElement(By.Id("program")));
            foreach (var option in programSelector.Options)
            {
                if (option.Text.Contains("Нарушение слуха"))
                {
                    programSelector.SelectByText(option.Text);
                    break;
                }
            }
            
            _webDriver.FindElements(By.TagName("button")).First(b => b.Text == "Добавить").Click();
        }
    }
}