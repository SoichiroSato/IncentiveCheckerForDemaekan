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
        public static string ChromeInstallWindows()
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
        /// ChromeInstall.shの中身を作成する
        /// </summary>
        /// <returns>ChromeInstall.shの中身</returns>
        public static string ChromeInstallLinux()
        {
            var sb = new StringBuilder();
            sb.AppendLine("@echo off");
            sb.Append("#!/bin/bash" + "\n");
            sb.Append("res=`cat /etc/os-release`" + "\n");
            sb.Append("echo $res" + "\n");
            sb.Append("if [[ \"$res\" == *CentOS* ]] || [[ \"$res\" == *centos* ]]; then" + "\n");
            sb.Append("  sudo yum install https://dl.google.com/linux/direct/google-chrome-stable_current_x86_64.rpm" + "\n");
            sb.Append("  sudo yum install google-chrome-stable" + "\n");
            sb.Append("elif [[ \"$res\" == *ubuntu* ]]; then" + "\n");
            sb.Append("  sudo apt update" + "\n");
            sb.Append("  sudo apt upgrade" + "\n");
            sb.Append("  sudo wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb" + "\n");
            sb.Append("  sudo apt install ./google-chrome-stable_current_amd64.deb" + "\n");
            sb.Append("  sudo apt -f install" + "\n");
            sb.Append("fi" + "\n");

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
