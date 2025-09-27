using APIDemos.Base;
using APIDemos.Common;
using OpenQA.Selenium;

namespace APIDemos.Page.Level3
{
  public class ExpandableListsPage : BasePage
  {
    public ExpandableListsPage(IWebDriver driver) : base(driver) { }

    public bool LongPressMenu()
    {
      driver.Tap(By.XPath("//android.widget.TextView[@content-desc=\"1. Custom Adapter\"]"));
      By peopleNames = By.XPath("//android.widget.TextView[@text=\"People Names\"]");
      By dogNames = By.XPath("//android.widget.TextView[@text=\"Dog Names\"]");

      driver.LongPress(peopleNames);

      var validateText = driver.GetText(By.XPath("//android.widget.TextView[@resource-id=\"android:id/title\" and @text=\"Sample menu\"]"));
      return validateText.Contains("Sample menu".ToLower());
    }
  }
}
