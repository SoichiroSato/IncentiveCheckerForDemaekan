@echo off
 
rem ダウンロードフォルダーに移動
C:
cd C:\Users\%username%\Downloads
echo %CD%

rem  Windows OSのアーキテクチャ（32bit版/64bit版）と一致するmsiファイルのURLを指定してダウンロード
if %PROCESSOR_ARCHITECTURE% =="AMD64" (
    bitsadmin /TRANSFER "download" https://dl.google.com/chrome/install/GoogleChromeStandaloneEnterprise64.msi %CD%\GoogleChromeStandaloneEnterprise.msi
) else (
    bitsadmin /TRANSFER "download" https://dl.google.com/chrome/install/GoogleChromeStandaloneEnterprise.msi %CD%\GoogleChromeStandaloneEnterprise.msi
)

rem Google Chrome をインストール
msiexec /i %CD%\GoogleChromeStandaloneEnterprise.msi

echo %errorlevel%