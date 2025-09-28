using APIDemos.Base;
using APIDemos.Common;
using APIDemos.Page.Level3;
using OpenQA.Selenium;

namespace APIDemos.Page.Level2
{
  public class AppPage : BasePage
  {
    public AppPage(IWebDriver driver) : base(driver) { }
    private By notificationPage = By.XPath("//android.widget.TextView[@content-desc=\"Notification\"]");

    public NotificationPage GoToNotificationPage()
    {
      driver.Tap(notificationPage);
      return new NotificationPage(driver);
    }
  }
}
