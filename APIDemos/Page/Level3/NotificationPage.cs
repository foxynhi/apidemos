using APIDemos.Base;
using APIDemos.Common;
using OpenQA.Selenium;

namespace APIDemos.Page.Level3
{
  public class NotificationPage : BasePage
  {
    public NotificationPage(IWebDriver driver) : base(driver) { }

    public bool GetSystemNotification()
    {
      driver.Tap(By.XPath("//android.widget.TextView[@content-desc=\"IncomingMessage\"]"));
      Thread.Sleep(1000);
      var alert = driver.SwitchTo().Alert();
      bool isSuccess =  driver.GetText(By.XPath("//android.widget.TextView[@resource-id=\"com.android.permissioncontroller:id/permission_message\"]")).Contains("notifications".ToLower());
      if (isSuccess) {
        alert.Accept();
        return isSuccess;
      }
      return false;
    }
  }
}
