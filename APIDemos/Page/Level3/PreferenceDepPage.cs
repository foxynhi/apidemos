using APIDemos.Base;
using APIDemos.Common;
using OpenQA.Selenium;

namespace APIDemos.Page.Level3
{
  public class PreferenceDepPage : BasePage
  {
    public PreferenceDepPage(IWebDriver driver) : base(driver) { }

    private By Wifi = By.Id("android:id/checkbox");
    private By WifiSettings = By.XPath("//android.widget.ListView[@resource-id=\"android:id/list\"]/android.widget.LinearLayout[2]/android.widget.RelativeLayout");
    private By Modal = By.Id("android:id/alertTitle");
    private By TextInp = By.XPath("//android.widget.EditText[@resource-id=\"android:id/edit\"]");
    private By OkBtn = By.XPath("//android.widget.Button[@resource-id=\"android:id/button1\"]");

    public void TurnOnWifi()
    {
      IWebElement el = driver.FindVisible(Wifi);
      if (el != null)
      {
        if (el.GetAttribute("checked") == "false")
        {
          driver.Tap(Wifi);
        }
      }
    }

    public bool VerifyWifiOn() => driver.WaitUntilClickable(Wifi).GetAttribute("checked") == "true";

    public void ChangeWifiSettings(string settings)
    {
      driver.Tap(WifiSettings);

      var Alert = driver.FindVisible(Modal);
      if (Alert != null)
      {
        driver.SetText(TextInp, settings);

        driver.Tap(OkBtn);
      }
    }

    public bool VerifySettings(string settings) 
    {
      driver.Tap(WifiSettings);
      return driver.GetText(TextInp).Equals(settings);
    }
  }
}
