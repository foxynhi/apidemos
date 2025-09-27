using APIDemos.Base;
using APIDemos.Common;
using APIDemos.Page.Level3;
using OpenQA.Selenium;

namespace APIDemos.Page.Level2
{
  public class PreferencePage : BasePage
  {
    public PreferencePage(IWebDriver driver) : base(driver) { }

    private By PreferenceDependencies = By.XPath("//android.widget.TextView[@content-desc=\"3. Preference dependencies\"]");

    public PreferenceDepPage GoToPreferenceDepPage()
    {
      driver.Tap(PreferenceDependencies);
      return new PreferenceDepPage(driver);
    }
  }
}
