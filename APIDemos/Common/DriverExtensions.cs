using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.BiDi.Input;
using OpenQA.Selenium.Interactions;

namespace APIDemos.Common
{
  public static class DriverExtensions
  {
    private static int screenW (this IWebDriver driver) => driver.Manage().Window.Size.Width;
    private static int screenH (this IWebDriver driver) => driver.Manage().Window.Size.Height;
    public static Actions Act (this IWebDriver driver) => new Actions(driver);
    public static IWebElement FindVisible(this IWebDriver driver, By by) => driver.WaitUntilVisible(by);
    public static IWebElement FindClickable(this IWebDriver driver, By by) => driver.WaitUntilClickable(by);

    public static void ScrollTo(this IWebDriver driver, By by) => Act(driver).ScrollToElement(driver.FindVisible(by));
    public static void SwipeToElementJS(this IWebDriver driver, By by)
    {
      ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", driver.FindElement(by));
    }

    public static string GetText(this IWebDriver driver, By by) => driver.FindVisible(by).Text;

    public static void SetText(this IWebDriver driver, By by, string text) => driver.FindVisible(by).SendKeys(text);

    public static void Tap(this IWebDriver driver, By by) => driver.FindClickable(by).Click();

    public static void DragnDrop(this IWebDriver driver, By source, By target)
    {
      IWebElement from = driver.WaitUntilVisible(source, 3);
      IWebElement to = driver.WaitUntilVisible(target, 3);
      new Actions(driver).DragAndDrop(from, to).Perform();
    }

    public static void LongPress(this IWebDriver driver, By by)
    {
      var ele = driver.FindVisible(by);
      Act(driver).MoveToElement(ele)
                  .Pause(TimeSpan.FromMilliseconds(300))
                  .ClickAndHold()
                  .Pause(TimeSpan.FromMilliseconds(800))
                  .Perform();
    }
  }
}
