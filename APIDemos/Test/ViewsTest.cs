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
      expandableListsPage.LongPressMenu();
      //Assert.That(expandableListsPage.VerifyDnDAction(), Is.True, "Drag and drop failed");
    }

    //[Testd
    public void abc()
    {
      var TestPage = new HomePage(driver).GoToViewsPage();
      var ele = driver.FindElement(By.XPath("//android.widget.TextView[@content-desc=\"Grid\"]"));
      Thread.Sleep(2000);
      new Actions(driver).ScrollToElement(ele);
    }
  }
}
