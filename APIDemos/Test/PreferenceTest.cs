
using APIDemos.Page;

namespace APIDemos.Test
{
  public class PreferenceTest : BaseTest
  {
    public PreferenceTest(DeviceConfig cfg) : base(cfg) {}

    [Test]
    public void WifiTest()
    {
      var preferencePage = new HomePage(driver).GoToPreferencePage();
      var preferenceDepPage = preferencePage.GoToPreferenceDepPage();

      preferenceDepPage.TurnOnWifi();
      Assert.That(preferenceDepPage.VerifyWifiOn(), Is.True, "Wifi is not on");

      string settings = "Change Wifi settings";
      preferenceDepPage.ChangeWifiSettings(settings);
      Assert.That(preferenceDepPage.VerifySettings(settings), Is.True, "Wifi settings is wrong");
    }
  }
}
