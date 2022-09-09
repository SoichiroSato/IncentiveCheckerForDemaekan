using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// WebDriver基底クラス
    /// </summary>
    public class WebDriver : IDisposable
    {
        /// <summary>
        /// ChromeDriverService
        /// </summary>
        public ChromeDriverService DriverService { get; }
        /// <summary>
        /// ChromeDriver
        /// </summary>
        public ChromeDriver Driver { get; }
        /// <summary>
        /// コンストラクタ
        /// WebDriverManagerを使ってWebDriverを設定する
        /// </summary>
        /// <param name="options">choromeオプションに設定する文字列配列</param>
        public WebDriver(string[]? options = null,double wait = 0)
        {
            var chromeConfig = new ChromeConfig();
            new DriverManager().SetUpDriver(chromeConfig, VersionResolveStrategy.MatchingBrowser);
            string driverVersion = chromeConfig.GetMatchingBrowserVersion();
            string driverPath = $"./Chrome/{driverVersion}/X64/";
            DriverService = ChromeDriverService.CreateDefaultService(driverPath);
            if(options == null)
            {
                Driver = new ChromeDriver(DriverService);
            }
            else
            {
                var chromeOptions = new ChromeOptions();
                chromeOptions.AddArguments(options);
                Driver = new ChromeDriver(DriverService, chromeOptions);
            }
            if(wait != 0)
            {
                Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(wait);
            } 
           
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose基底処理
        /// </summary>
        /// <param name="disposing">disposing</param>
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