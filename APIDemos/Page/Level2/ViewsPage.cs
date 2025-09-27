using APIDemos.Base;
using APIDemos.Common;
using APIDemos.Page.Level3;
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V137.Input;

namespace APIDemos.Page.Level2
{
  public class ViewsPage : BasePage
  {
    public ViewsPage(IWebDriver driver) : base(driver) { }

    private By ExpandableLists = By.XPath("//android.widget.TextView[@content-desc=\"Expandable Lists\"]");
    public DragnDropPage GoToDragnDropPage()
    {
      driver.Tap(By.XPath("//android.widget.TextView[@content-desc=\"Drag and Drop\"]"));
      return new DragnDropPage(driver);
    }

    public ExpandableListsPage GoToExpListsPage()
    {
      driver.Tap(ExpandableLists);
      return new ExpandableListsPage(driver);
    }
  }
}
