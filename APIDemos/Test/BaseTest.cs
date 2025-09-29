
using APIDemos.Page;
using APIDemos.Report;
using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;

//[assembly: Parallelizable(ParallelScope.Fixtures)]
//[assembly: LevelOfParallelism(3)]

namespace APIDemos.Test;

public sealed record DeviceConfig(
    string Name,
    string DeviceName,
    string PlatformName,
    int SystemPort,
    string? App = null,
    string? AppPackage = null,
    string? AppActivity = null
);
public static class DeviceMatrix
{
  public static IEnumerable<DeviceConfig> All => new[]
  {
    new DeviceConfig(
      Name: "Pixel 9",
      DeviceName: "emulator-5554",
      PlatformName: "Android",
      SystemPort: 8888,
      App: null,
      AppPackage: "io.appium.android.apis",
      AppActivity: ".ApiDemos"
    ),
    //new DeviceConfig(
    //  Name: "Medium Phone",
    //  DeviceName: "emulator-5556",
    //  PlatformName: "Android",
    //  SystemPort: 8889,
    //  App: null,
    //  AppPackage: "io.appium.android.apis",
    //  AppActivity: ".ApiDemos"
    //),
    //new DeviceConfig(
    //  Name: "Pixel 7",
    //  DeviceName: "emulator-5558",
    //  PlatformName: "Android",
    //  SystemPort: 8890,
    //  App: null,
    //  AppPackage: "io.appium.android.apis",
    //  AppActivity: ".ApiDemos"
    //)
  };
}
[TestFixtureSource(typeof(DeviceMatrix), nameof(DeviceMatrix.All))]
[Parallelizable(ParallelScope.All)]
public class BaseTest
{
  protected AndroidDriver driver = default!;
  private DeviceConfig _cfg;

  private static readonly ExtentReports _extent = ExtentService.Extent;
  private static readonly ThreadLocal<ExtentTest> _test = new();

  public BaseTest(DeviceConfig cfg) => _cfg = cfg;

  [SetUp]
  public void SetUp()
  {
    string testName = TestContext.CurrentContext.Test.Name;
    _test.Value = _extent
                  .CreateTest(testName)
                  .AssignCategory(_cfg.Name)
                  .AssignCategory(_cfg.PlatformName)
                  .AssignDevice(_cfg.Name);

    var options = new AppiumOptions();

    options.DeviceName = _cfg.Name;
    options.AutomationName = "UiAutomator2";
    options.PlatformName = _cfg.PlatformName;
    options.AddAdditionalAppiumOption("udid", _cfg.DeviceName);
    options.AddAdditionalAppiumOption("systemPort", _cfg.SystemPort);

    if (!string.IsNullOrWhiteSpace(_cfg.App))
    {
      options.AddAdditionalAppiumOption("app", _cfg.App);
    }
    else
    {
      options.AddAdditionalAppiumOption("appPackage", _cfg.AppPackage);
      options.AddAdditionalAppiumOption("appActivity", _cfg.AppActivity);
    }

    _test.Value.Info("Starting driver");
    try
    {
      var url = Environment.GetEnvironmentVariable("APPIUM_SERVER_URL")
          ?? "http://127.0.0.1:4723";
      driver = new AndroidDriver(new Uri(url), options, TimeSpan.FromMinutes(3));
    }
    catch (UnknownErrorException ex) 
    { 
      Console.WriteLine(ex.Message);
    }
    _test.Value.Info($"Session: {driver.SessionId}");
  }

  [TearDown]
  public void TearDown()
  {
    var context = TestContext.CurrentContext;
    try
    {
      if (context.Result.Outcome.Status == TestStatus.Failed)
      {
        string screenshotPath = Path.Combine(ExtentService.ReportPath, $"{context.Test.Name}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        
        ((ITakesScreenshot)driver).GetScreenshot().SaveAsFile(screenshotPath);

        _test.Value?.AddScreenCaptureFromPath(Path.GetFileName(screenshotPath));
      }
      else if (context.Result.Outcome.Status == TestStatus.Passed)
      {
        _test.Value?.Pass("Test passed");
      }
      else
      {
        _test.Value?.Skip("Test skipped");
      }
    } catch (Exception ex)
    {
      _test.Value?.Warning($"Failed to capture screenshot: {ex.Message}");
    }
    finally
    {
      try { driver?.Quit(); } catch { }
      driver?.Dispose();
    }
  }
  [OneTimeTearDown]
  public void OneTimeTearDown()
  {
    _extent.Flush();
    TestContext.WriteLine($"Report: {ExtentService.ReportPath}");
  }
}

