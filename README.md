# IncentiveCheckerforDemaekan
# 概要

[出前館インセンティブサイト](https://cdn.demae-can.com/contents/driver/boost/area/index.html)からcsvで指定したエリアのインセンティブ情報をLine通知するツールです。<br/>
タスクスケジューラー等に設定して使用します。

## 作成の背景

出前館のインセンティブ情報は出前館インセンティブサイトで公開されており、各自アクセスして確認する必要があります。<br/>
出前館インセンティブサイトは全エリアのインセンティブ情報を公開しているため、表示するためにはエリアと都道府県を選択する必要があります。<br/>
しかし参照するエリアはわずか数エリアであり、参照したいエリアが都道府県をまたぐ場合は都度画面操作が必要です。<br/>
そこで自分の必要なエリアのインセンティブ情報のみを日常生活で必要不可欠となっているLine通知することで情報取得を簡易化し、その日の配達戦略考案に貢献するツールを作りました。<br/>

<img src="https://user-images.githubusercontent.com/36285803/196443688-81bab726-e818-4e93-9d55-631289947da6.png" width="300px">

## 使用技術

■ 言語・FW<br>

-   .Net 6.0
-   C#
-   Bat
-   Shell

■ ライブラリ<br>

-   Selenium

■ その他<br>

-   GitHub
-   SourceTree
-   LineNotify
-   GoogleChrome
-   PlantUml

## クラス図
![image](https://user-images.githubusercontent.com/36285803/210171652-99d7323b-1ba3-4dc0-b475-14aef70edeb1.png)

## 利用準備 (LINE）
1. [LINE Notify](https://notify-bot.line.me/ja/)の公式ページへいき、ログインします。 ログインIDとパスワードは個人で利用しているLINEの資格情報と同じです。
2. 右上にある自分の名前 > マイページ をクリックします。
3. アクセストークンの発行(開発者向け)」 > 「トークンを発行する」をクリックします。
4. 「トークンを発行する」クリックすると、どのグループへ通知するかの選択画面が表示されるので任意のグループを選択してアクセストークンを発行します。 ※発行されたアクセストークンはメモ帳等に保存してください。
5. Line Notifyを選択したグループに招待します。

## 利用準備 (本プロジェクトのビルド、ファイルの設定）

1. 本プロジェクトをクローンするか、zipとしてダウンロードし解凍します。
2. 本プロジェクトをビルドします。
3. ビルドしてできた「TargetPlace.csv」の2行目以降に通知したい地域を「エリア,都道府県,市区町村」の順に入力します。

<img src="https://user-images.githubusercontent.com/36285803/196426396-c082219a-cb8c-4936-a4c9-c11bb39a526c.png" width="800px">

4. コマンドライン引数を利用しない場合はビルドしてできた「LineToken.txt」に取得したLineアクセストークンを入力します。

## 利用準備 (処理設定）
本ツールは一部非同期通信処理を実装しております。<br/>
非同期通信で処理を行う場合は実行速度向上が見込めます。<br/>
ただし実行PCやサーバのスペックによっては処理負荷が向上するためエラーが発生する場合もございます。<br/>
その場合は同期通信での実行をお願いします。<br/>
設定方法はApp.config（IncentiveCheckerforDemaekan.dll.config)の以下の設定を行ってください。<br/>
デフォルトは同期通信です。<br/>
```
<?xml version="1.0" encoding="utf-8" ?>
<configuration>
	<appSettings>
		<!--falseかtrueを入力してください。
		false:同期通信、true非同期通信-->
		<add key ="async" value="false"/>
	</appSettings>
</configuration>
```


## 利用準備 (GoogleChromeの準備）※Windows以外
本ツールにはGoogleChromeを使うためインストールをしといてください。<br>
※Windowsの場合はGoogleChromeがインストールされてない場合は本ツールを管理者権限で実行した場合自動でインストールします。

## 実行方法(Windows)
1. LineToken.txtに取得したLineアクセストークンを入力していない場合
```
    IncentiveCheckerforDemaekan.exe [取得したLineアクセストークン]
```
2. LineToken.txtに取得したLineアクセストークンを入力している場合
```
    IncentiveCheckerforDemaekan.exe 
```

## 実行方法(Linux)
※事前に.net6 .net SDK .net runtimeなどをインストールしておいてください。

1. LineToken.txtに取得したLineアクセストークンを入力していない場合
```
    dotnet IncentiveCheckerforDemaekan.dll [取得したLineアクセストークン]
```    
2. LineToken.txtに取得したLineアクセストークンを入力している場合
```
    dotnet IncentiveCheckerforDemaekan.dll
``` 
## タスクスケジューラーへの登録例(Windows)

<img src="https://user-images.githubusercontent.com/36285803/196444072-e66561c1-3a5c-4283-b716-6a585de4214e.png" width="300px">
<img src="https://user-images.githubusercontent.com/36285803/196453824-31f7c7eb-da2b-42f6-a0ee-c43592991a90.png" width="300px">
<img src="https://user-images.githubusercontent.com/36285803/196449854-e6be6ccf-18be-4c35-933d-b4cac32a99c4.png" width="300px">
<img src="https://user-images.githubusercontent.com/36285803/196449993-050cc5c9-9e0c-4a35-abb5-db230a93dc17.png" width="300px">

## タスクスケジューラーへの登録例(Linux、Mac)
```
    crontab -e
```

```
    CRON_TZ="Japan"
    [mm] [hh] * * * cd [homeディレクトリからの格納ディレクトリ]; dotnet IncentiveCheckerforDemaekan.dll 
```
