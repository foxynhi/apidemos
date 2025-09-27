using OpenQA.Selenium;
using APIDemos.Common;

namespace APIDemos.Base
{
  public class BasePage
  {
    protected IWebDriver driver;

    public BasePage(IWebDriver driver) => this.driver = driver;
  }
}
