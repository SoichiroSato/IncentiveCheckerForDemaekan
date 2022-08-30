using System.Text;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// ファイルの中身作成クラス
    /// </summary>
    public class FileContents
    {
        /// <summary>
        /// ChromeInstall.batの中身を作成する
        /// </summary>
        /// <returns>ChromeInstall.batの中身</returns>
        public static string ChromeInstall()
        {
            var sb = new StringBuilder();
            sb.AppendLine("@echo off");
            sb.AppendLine(" ");
            sb.AppendLine("rem ダウンロードフォルダーに移動");
            sb.AppendLine("C:");
            sb.AppendLine("cd C:\\Users\\%username%\\Downloads");
            sb.AppendLine("echo %CD%");
            sb.AppendLine("");
            sb.AppendLine("rem  Windows OSのアーキテクチャ（32bit版/64bit版）と一致するmsiファイルのURLを指定してダウンロード");
            sb.AppendLine("if %PROCESSOR_ARCHITECTURE% ==\"AMD64\" (");
            sb.AppendLine("    bitsadmin /TRANSFER \"download\" https://dl.google.com/chrome/install/GoogleChromeStandaloneEnterprise64.msi %CD%\\GoogleChromeStandaloneEnterprise.msi");
            sb.AppendLine(") else (");
            sb.AppendLine("    bitsadmin /TRANSFER \"download\" https://dl.google.com/chrome/install/GoogleChromeStandaloneEnterprise.msi %CD%\\GoogleChromeStandaloneEnterprise.msi");
            sb.AppendLine(")");
            sb.AppendLine("");
            sb.AppendLine("rem Google Chrome をインストール");
            sb.AppendLine("msiexec /i %CD%\\GoogleChromeStandaloneEnterprise.msi");
            sb.AppendLine("");
            sb.AppendLine("echo %errorlevel%");
            return sb.ToString();
        }

        /// <summary>
        /// TargetPlace.csvの中身を作成する
        /// </summary>
        /// <returns>TargetPlace.csvの中身</returns>
        public static string TargetPlace()
        {
            return "エリア,都道府県,市区町村";
        }
    }
}
