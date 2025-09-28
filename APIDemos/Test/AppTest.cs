
using APIDemos.Page;

namespace APIDemos.Test
{
  public class AppTest : BaseTest
  {
    public AppTest(DeviceConfig cfg) : base(cfg) {}

    [Test]
    public void SystemNotificationTest()
    {
      var notificationPage = new HomePage(driver).GoToAppPage().GoToNotificationPage();
      notificationPage.GetSystemNotification();
    }
  }
}
