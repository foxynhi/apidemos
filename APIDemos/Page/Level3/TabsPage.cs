using APIDemos.Base;
using APIDemos.Common;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIDemos.Page.Level3
{
  public class TabsPage : BasePage
  {
    public TabsPage(IWebDriver driver) : base(driver){}

    public bool SwitchTabs()
    {
      driver.Tap(By.XPath("//android.widget.TextView[@content-desc=\"1. Content By Id\"]"));

      By Tab1Content = By.XPath("//android.widget.TextView[@content-desc=\"tab1\"]");
      By Tab2Content = By.XPath("//android.widget.TextView[@content-desc=\"tab2\"]");
      By Tab3 = By.XPath("//android.widget.TabWidget[@resource-id=\"android:id/tabs\"]/android.widget.LinearLayout[3]");
      By Tab3Content = By.XPath("//android.widget.TextView[@content-desc=\"tab3\"]");

      driver.Tap(Tab3);
      return driver.GetText(Tab3Content).Contains("tab3");
    }
  }
}
