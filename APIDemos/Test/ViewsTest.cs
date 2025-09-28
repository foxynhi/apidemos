using APIDemos.Common;
using APIDemos.Page;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace APIDemos.Test
{
  public class ViewsTest : BaseTest
  {
    public ViewsTest(DeviceConfig cfg) : base(cfg) { }

    [Test]
    public void DragnDropTest()
    {
      var dragnDropPage = new HomePage(driver).GoToViewsPage().GoToDragnDropPage();
      dragnDropPage.DnDAction();
      Assert.That(dragnDropPage.VerifyDnDAction(), Is.True, "Drag and drop failed");
    }

    [Test]
    public void LongPressMenuTest()
    {
      var expandableListsPage = new HomePage(driver).GoToViewsPage().GoToExpListsPage();
      Assert.That(expandableListsPage.LongPressMenu(), Is.True);
    }

    [Test]
    public void TabsTest()
    {
      var tabsPage = new HomePage(driver).GoToViewsPage().GoToTabsPage();
      Assert.That(tabsPage.SwitchTabs(), Is.True);
    }

    [Test]
    public void abc()
    {
      var W = driver.Manage().Window.Size.Width;
      var H = driver.Manage().Window.Size.Height;
      var viewsPage = new HomePage(driver).GoToViewsPage();
      new Actions(driver).MoveToLocation((int)(W * 0.5), (int)(H * 0.6))
             .ClickAndHold()
             .Pause(TimeSpan.FromMilliseconds(200))
             .MoveByOffset(0, -(int)(H * 0.6))
             .Pause(TimeSpan.FromMilliseconds(500))
             .Release()
             .Perform();
    }
  }
}
