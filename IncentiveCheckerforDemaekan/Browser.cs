using IncentiveCheckerforDemaekan.Base;
using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

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
        /// Choromeをインストールする
        /// 各ファンクションでOSチェックを行う
        /// </summary>
        public void InstallChrome()
        {
            InstallChromeWindows();
            InstallChromeLinux();
            InstallChromeMac();
        }

        /// <summary>
        /// LinuxでChoromeがインストールされているか確認しなかったらインストールする
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
            ExcuteFile(processStartInfo);
        }

        /// <summary>
        /// MacでChoromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        private void InstallChromeMac()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) { return; }
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/Applications/Utilities/Terminal.app/Contents/MacOS/Terminal",
                Arguments = Path.Combine(LocationPath, "File/Mac/ChromeInstall.sh"),
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            ExcuteFile(processStartInfo);
        }

        /// <summary>
        /// windowsでChoromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        private void InstallChromeWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return; }           
            RegistryKey? browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            browserKeys ??= Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            if (browserKeys == null) { return; }
            var subKeyNames = browserKeys.GetSubKeyNames();
            var browsers = new List<string>();
            foreach (var browser in subKeyNames)
            {
                // ブラウザーの名前
                RegistryKey? browserKey = browserKeys.OpenSubKey(browser);
                if (browserKey == null) { return; }
                var browserName = browserKey.GetValue(null);
                if (browserName == null) { return; }
                browsers.Add((string)browserName);
            }
            if(browsers.Contains("Google Chrome")) { return; }
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
            ExcuteFile(processStartInfo);
        }       
    }
}
