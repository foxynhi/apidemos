
using APIDemos.Base;
using APIDemos.Common;
using APIDemos.Page.Level2;
using OpenQA.Selenium;

namespace APIDemos.Page
{
  public class HomePage : BasePage
  {
    public HomePage(IWebDriver driver) : base(driver) { }

    private By App = By.XPath("//android.widget.TextView[@content-desc=\"App\"]");
    private By Preference = By.XPath("//android.widget.TextView[@content-desc=\"Preference\"]");
    private By Views = By.XPath("//android.widget.TextView[@content-desc=\"Views\"]");

    public AppPage GoToAppPage() {
      driver.Tap(App);
      return new AppPage(driver);
    }
    public PreferencePage GoToPreferencePage()
    {
      driver.Tap(Preference);
      return new PreferencePage(driver);
    }
    public ViewsPage GoToViewsPage()
    {
      driver.Tap(Views);
      return new ViewsPage(driver);
    }

  }
}
