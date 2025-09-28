using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Interfaces;
using OpenQA.Selenium.BiDi.BrowsingContext;
using OpenQA.Selenium.BiDi.Input;
using OpenQA.Selenium.Interactions;

namespace APIDemos.Common
{
  public static class DriverExtensions
  {
    private static int screenW (this IWebDriver driver) => driver.Manage().Window.Size.Width;
    private static int screenH (this IWebDriver driver) => driver.Manage().Window.Size.Height;
    public static Actions Act (this IWebDriver driver) => new Actions(driver);
    public static IWebElement FindVisible(this IWebDriver driver, By by, int seconds = 10) => driver.WaitUntilVisible(by, seconds);
    public static IWebElement FindClickable(this IWebDriver driver, By by) => driver.WaitUntilClickable(by);
    public static void SwipeUp(this IWebDriver driver)
    {
      int W = screenW(driver);
      int H = screenH(driver);
      Act(driver).MoveToLocation((int)(W * 0.5), (int)(H * 0.9))
             .ClickAndHold()
             .Pause(TimeSpan.FromMilliseconds(200))
             .MoveByOffset(0, -(int)(H * 0.7))
             .Pause(TimeSpan.FromMilliseconds(500))
             .Release()
             .Perform();
    }

    public static void SwipeDown(this IWebDriver driver)
    {
      int W = screenW(driver);
      int H = screenH(driver);
      Act(driver).MoveToLocation((int)(W * 0.5), (int)(H * 0.1))
             .ClickAndHold()
             .Pause(TimeSpan.FromMilliseconds(200))
             .MoveByOffset(0, (int)(H * 0.7))
             .Pause(TimeSpan.FromMilliseconds(500))
             .Release()
             .Perform();
    }
    public static void SwipeToElement(this IWebDriver driver, By by, int maxSwipes)
    {
      int swipeCount = 0;
      while (swipeCount < maxSwipes)
      {
        try
        {
          var element = driver.FindElement(by);
          if (element.Displayed)
            return;
        }
        catch (NoSuchElementException)
        {
        }
        driver.SwipeUp();

        swipeCount++;
      }
      throw new NoSuchElementException($"Element {by} not found after {maxSwipes} swipes.");    
    }
    public static void Swipe(this IWebDriver driver, int startX, int startY, int endX, int endY)
    {
      Act(driver).MoveToLocation(startX, startY)
             .ClickAndHold()
             .Pause(TimeSpan.FromMilliseconds(200))
             .MoveByOffset(startX-endX, startY - endY)
             .Pause(TimeSpan.FromMilliseconds(500))
             .Release()
             .Perform();
    }
    public static string GetText(this IWebDriver driver, By by) => driver.FindVisible(by).Text;

    public static void SetText(this IWebDriver driver, By by, string text) => driver.FindVisible(by).SendKeys(text);

    public static void Tap(this IWebDriver driver, By by) => driver.FindClickable(by).Click();

    public static void DragnDrop(this IWebDriver driver, By source, By target)
    {
      IWebElement from = driver.WaitUntilVisible(source, 3);
      IWebElement to = driver.WaitUntilVisible(target, 3);
      Act(driver).ClickAndHold(from)
          .Pause(TimeSpan.FromMilliseconds(500))
          .MoveToElement(to)
          .Release()
          .Perform();
    }

    public static void LongPress(this IWebDriver driver, By by)
    {
      var ele = driver.FindVisible(by);
      Act(driver).ClickAndHold(ele)
                  .Pause(TimeSpan.FromMilliseconds(800))
                  .Release()
                  .Perform();
    }
  }
}


//public static void ScrollTo(this IWebDriver driver, By by, int maxSwipes)
//{
//  //Act(driver).ScrollToElement(driver.FindVisible(by)).Perform();
//  for (int i = 0; i < maxSwipes; i++)
//  {
//    if (driver.WaitUntilVisible(by, 2) != null)
//    {
//      Console.WriteLine("Exit");
//      return;
//    }

//    driver.SwipeUp();
//  }

//  throw new Exception("Element not found after swiping.");
//}
//public static void ScrollToJS(this IWebDriver driver, By by)
//{
//  ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView(true);", driver.FindClickable(by));
//}