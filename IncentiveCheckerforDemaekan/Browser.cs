using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// ブラウザ系処理クラス
    /// </summary>
    public class Browser
    {
        /// <summary>
        /// chromeがインストールされているか確認する
        /// </summary>
        /// <returns>インストールされているかどうか</returns>
        public static bool IsInstallChrome()
        {        
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return IsInstallChromeWindows();
            }
            else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return IsInstallChromeLinux();
            }
            return false;
        }

        /// <summary>
        /// windowsでchromeがインストールされているか確認する
        /// </summary>
        /// <returns>インストールされているかどうか</returns>
        public static bool IsInstallChromeWindows()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return false; }
            var browsers = new List<string>();
            RegistryKey? browserKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Clients\StartMenuInternet");
            browserKeys ??= Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Clients\StartMenuInternet");
            if ((browserKeys == null)) { return false; }
            var subKeyNames = browserKeys.GetSubKeyNames();
            foreach (var browser in subKeyNames)
            {
                // ブラウザーの名前
                RegistryKey? browserKey = browserKeys.OpenSubKey(browser);
                if (browserKey == null) { return false; }
                var browserName = browserKey.GetValue(null);
                if (browserName == null) { return false; }
                browsers.Add((string)browserName);
            }
            return browsers.Contains("Google Chrome");
        }

        /// <summary>
        /// Linuxでchromeがインストールされているか確認する
        /// </summary>
        /// <returns>インストールされているかどうか</returns>
        public static bool IsInstallChromeLinux()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { return false; }
            using var cmd = new Cmd();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = "google-chrome --version",
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var result = cmd.ExcuteFile(processStartInfo);
            return !(result.Contains("コマンドが見つかりません") || result.Contains("command not found")) ;
        }

        /// <summary>
        /// LinuxでChoromeをインストールする
        /// </summary>
        /// <param name="filePath">実行ファイルのフルパス</param>
        public static void InstallChromeLinux(string filePath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) { return; }
            using var cmd = new Cmd();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/sh",
                Arguments = @filePath,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            cmd.ExcuteFile(processStartInfo);
        }

        /// <summary>
        /// WindowsOSでChoromeをインストールする
        /// </summary>
        /// <param name="filePath">実行ファイルのフルパス</param>
        public static void InstallChromeWindows(string filePath)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { return; }
            using var cmd = new Cmd();
            var processStartInfo = new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("ComSpec"),
                Arguments = "/c " + filePath,
                RedirectStandardInput = false,
                RedirectStandardOutput = false,
                UseShellExecute = true,
                CreateNoWindow = true,
                Verb = "runas"
            };
            cmd.ExcuteFile(processStartInfo);
        }
    }
}
