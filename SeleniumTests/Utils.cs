using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace SeleniumTests
{
    public static class Utils
    {
        public const string SiteUrl = "http://127.0.0.1";


        public static IWebDriver Driver => new FirefoxDriver();

        public static void Autorise(IWebDriver webDriver, string login, string password)
        {
            webDriver.Navigate().GoToUrl(SiteUrl);
            webDriver.FindElement(By.Id("login")).SendKeys(login);
            webDriver.FindElement(By.Id("password")).SendKeys(password);
            webDriver.FindElement(By.ClassName("authButton")).Click();
        }
    }
}