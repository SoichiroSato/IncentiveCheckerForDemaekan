using IncentiveCheckerforDemaekan.Base;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// ブラウザ系処理クラス
    /// </summary>
    public class Browser : Cmd
    {
        /// <summary>
        /// カレントパス
        /// </summary>
        public string LocationPath { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="locationPath">カレントパス</param>
        public Browser(string locationPath)
        {
            LocationPath = locationPath;
        }

        /// <summary>
        /// Chromeをインストールする
        /// 各ファンクションでOSチェックを行う
        /// </summary>
        [SupportedOSPlatform("windows")]
        public void InstallChrome()
        {
            InstallChromeWindows();
            InstallChromeLinux();
            InstallChromeMac();
        }

        /// <summary>
        /// LinuxでChromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        private void InstallChromeLinux()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { return; }
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = @Path.Combine(LocationPath, "File/Linux/ChromeInstall.sh"),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            ExecuteFile(processStartInfo);
        }

        /// <summary>
        /// MacでChromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        private void InstallChromeMac()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash",
                Arguments = Path.Combine(LocationPath, "File/Mac/ChromeInstall.sh"),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            ExecuteFile(processStartInfo);
        }

        /// <summary>
        /// windowsでChromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        [SupportedOSPlatform("windows")]
        private void InstallChromeWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return; }           
            RegistryKey browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            browserKeys ??= Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            var subKeyNames = browserKeys.GetSubKeyNames();
            var browsers = (from browser in subKeyNames// ブラウザーの名前
                            let browserKey = browserKeys.OpenSubKey(browser)
                            let browserName = browserKey.GetValue(null)
                            select (string)browserName).ToList();
            if (browsers.Contains("Google Chrome")) { return; }
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("ComSpec"),
                Arguments = "/c " + Path.Combine(LocationPath, "File/Windows/ChromeInstall.bat"),
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            };
            ExecuteFile(processStartInfo);
        }       
    }
}
