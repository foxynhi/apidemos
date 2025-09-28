using APIDemos.Base;
using APIDemos.Common;
using OpenQA.Selenium;

namespace APIDemos.Page.Level2
{
  public class DragnDropPage : BasePage
  {
    public DragnDropPage(IWebDriver driver) : base(driver) { }

    public void DnDAction()
    {
      By dot1 = By.XPath("//android.view.View[@resource-id=\"io.appium.android.apis:id/drag_dot_1\"]");
      By dot2 = By.XPath("//android.view.View[@resource-id=\"io.appium.android.apis:id/drag_dot_2\"]");
      driver.DragnDrop(dot1, dot2);
    }

    public bool VerifyDnDAction()
    {
      string text = driver.FindVisible(By.XPath("//android.widget.TextView[@resource-id=\"io.appium.android.apis:id/drag_result_text\"]")).Text;
      return (text.Contains("Dropped") | text.Contains("Dropping"));
    }
  }
}
