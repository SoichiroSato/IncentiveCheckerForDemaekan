using OpenQA.Selenium.Chrome;
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