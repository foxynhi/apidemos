using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
namespace APIDemos.Report
{
  public static class ExtentService
  {
    public static readonly ExtentReports Extent = new ExtentReports();
    public static readonly string ReportPath = "";

    static ExtentService()
    {
      ReportPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Reports", $"API_Demos_{DateTime.Now:yyyyMMdd_HHmmss}"));
      Directory.CreateDirectory(ReportPath);

      Console.WriteLine($"Report Path: {ReportPath}");

      var spark = new ExtentSparkReporter(Path.Combine(ReportPath, "index.html"));
      spark.Config.DocumentTitle = "Mobile Test Report";
      spark.Config.ReportName = "API Demos Parallel Run";

      Extent.AttachReporter(spark);
    }
  }
}
