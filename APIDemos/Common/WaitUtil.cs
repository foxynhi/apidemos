

using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace APIDemos.Common
{
  public static class WaitUtil
  {
    private static WebDriverWait Wait(this IWebDriver driver, int seconds = 10) => new WebDriverWait(driver, TimeSpan.FromSeconds(seconds));

    public static IWebElement WaitUntilClickable (this IWebDriver driver, By by, int seconds = 10)
    {
      return Wait(driver, seconds).Until(ExpectedConditions.ElementToBeClickable(by));
    }
    public static IWebElement WaitUntilVisible (this IWebDriver driver, By by, int seconds = 10)
    {
      return Wait(driver, seconds).Until(ExpectedConditions.ElementIsVisible(by));
    }
    public static IWebElement WaitUntilExists(this IWebDriver driver, By by, int seconds = 10)
    {
      return new WebDriverWait(driver, TimeSpan.FromSeconds(seconds))
        .Until(d => {
          var els = d.FindElements(by);
          return els.Count > 0 ? els[0] : null;
        })!;
    }
  }
}
