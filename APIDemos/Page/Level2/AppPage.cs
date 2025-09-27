using APIDemos.Base;
using APIDemos.Common;
using OpenQA.Selenium;

namespace APIDemos.Page.Level2
{
  public class AppPage : BasePage
  {
    public AppPage(IWebDriver driver) : base(driver) { }

    public string VerifyPage()
    {
      return driver.GetText(By.XPath("//android.widget.TextView[@content-desc=\"Action Bar\"]"));
    }
  }
}
