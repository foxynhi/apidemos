using APIDemos.Page;
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
  }
}
