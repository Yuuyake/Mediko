using OpenQA.Selenium;

namespace Arcsight_Health_Checker {
    public static class Driver {
        public static IWebDriver ffoxDriver;
        public static int currTab;
        public static object ExecuteJS(this IWebDriver ffoxDriver, string script) {
            return ((IJavaScriptExecutor)ffoxDriver).ExecuteScript(script);
        }
    }
}