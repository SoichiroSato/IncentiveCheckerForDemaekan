using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
namespace IncentiveCheckerforDemaekan
{
    public class WebDriver : IDisposable
    {

        public ChromeDriverService DriverService { get; }
        public ChromeDriver Driver { get; }
        public WebDriver(string[]? options = null)
        {
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);

            // WebDriverManagerが保存した場所
            var driverVersion = new ChromeConfig().GetMatchingBrowserVersion();
            var driverPath = $"./Chrome/{driverVersion}/X64/";
            DriverService = ChromeDriverService.CreateDefaultService(driverPath);
            if(options == null)
            {
                Driver = new ChromeDriver(DriverService);
            }
            else
            {
                ChromeOptions chromeOptions = new();
                chromeOptions.AddArguments(options);
                Driver = new ChromeDriver(DriverService, chromeOptions);
            }
            //ドライバの起動場所を設定
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Driver.Quit();
                DriverService.Dispose();
            }
        }
    }
}