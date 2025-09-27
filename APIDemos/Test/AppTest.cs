
using APIDemos.Page;

namespace APIDemos.Test
{
  public class AppTest : BaseTest
  {
    public AppTest(DeviceConfig cfg) : base(cfg) {}

    [Test]
    public void VerifyTest()
    {
      var homePage = new HomePage(driver);
      var appPage = homePage.GoToAppPage();
      Assert.That(appPage.VerifyPage().Equals("Action Bar"));
    }
  }
}
