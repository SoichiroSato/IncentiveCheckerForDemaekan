# IncentiveCheckerforDemaekan
## 概要

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

■ ライブラリ/API<br>

-   Selenium
-   LineNotify

■ その他<br>

-   GitHub
-   SourceTree
-   Docker | docker-compose
-   PlantUml

## クラス図
<img src="https://www.plantuml.com/plantuml/proxy?fmt=svg&src=https://raw.githubusercontent.com/SoichiroSato/IncentiveCheckerforDemaekan/master/ClassDiagram.pu" />

## 前提

1. Lineアカウントを持っている。
2. 本プロジェクトをクローンまたはzipダウンロード後解凍済み

## 利用準備

### 1.アクセストークンの取得（必須）
1. [LINE Notify](https://notify-bot.line.me/ja/)の公式ページへいき、ログインします。 ログインIDとパスワードは個人で利用しているLINEの資格情報と同じです。
2. 右上にある自分の名前 > マイページ をクリックします。
3. アクセストークンの発行(開発者向け)」 > 「トークンを発行する」をクリックします。
4. 「トークンを発行する」クリックすると、どのグループへ通知するかの選択画面が表示されるので任意のグループを選択してアクセストークンを発行します。<br> ※発行されたアクセストークンはメモ帳等に保存してください。
5. Line Notifyを選択したグループに招待します。

### 2.対象地域の設定（必須）
「IncentiveCheckerforDemaekan/File/TargetPlace.csv」の2行目以降に通知したい地域を「エリア,都道府県,市区町村」の順に入力します。

<img src="https://user-images.githubusercontent.com/36285803/196426396-c082219a-cb8c-4936-a4c9-c11bb39a526c.png" width="800px">

### 3.LineTokenの設定（Dockerを使う場合のみ必須）

コマンドライン引数を利用しない場合は<br>
「IncentiveCheckerforDemaekan/File/LineToken.txt」に取得したLineトークンを入力して保存します。

### 4.処理設定（任意）
本ツールは一部非同期通信処理を実装しております。<br/>
非同期通信で処理を行う場合は実行速度向上が見込めます。<br/>
ただし実行PCやサーバのスペックによっては処理負荷が向上するためエラーが発生する場合もございます。<br/>
その場合は同期通信での実行をお願いします。<br/>
設定方法はApp.Release.configの以下の設定を行ってください。<br/>
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


### 5.GoogleChromeの準備（任意・エラーが出た場合・Docker以外）
本ツールでは実行時にChormeのインストールチェック処理が組み込まれていますが、<br>
予めインストールしたい場合は別ブラウザでインストーラーをダウンロード実行するか以下のファイルを実行してください

#### 別ブラウザでインストーラーをダウンロード実行する場合
https://www.google.com/intl/ja_jp/chrome/

#### ファイルでインストールする場合

1. windows<br>
IncentiveCheckerforDemaekan/File/Windows/ChromeInstall.bat<br>
※管理者権限で実行してください
2. Linux<br>
IncentiveCheckerforDemaekan/File/Linux/ChromeInstall.sh
3. Mac<br>
IncentiveCheckerforDemaekan/File/Mac/ChromeInstall.sh<br>
※管理者権限で実行してください

## ビルド・実行方法

Dockerを使わない場合は<br>
事前に.net6のSDK、runtimeなどをインストールしておいてください。

https://dotnet.microsoft.com/ja-jp/download/dotnet/6.0

### Windows

1. ビルド

```
    dotnet build -c Release
```   

2. 実行

LineToken.txtに取得したアクセストークンを入力していない場合
```
    IncentiveCheckerforDemaekan.exe [取得したLineアクセストークン]
```
LineToken.txtに取得したアクセストークンを入力している場合
```
    IncentiveCheckerforDemaekan.exe 
```

### Mac・Linux
1. ビルド
```
    dotnet build -c Release
```    
2. 実行

LineToken.txtに取得したアクセストークンを入力していない場合
```
    IncentiveCheckerforDemaekan.dll [取得したLineアクセストークン]
```
LineToken.txtに取得したアクセストークンを入力している場合
```
    IncentiveCheckerforDemaekan.dll 
```

### Docker
※事前にdockerをインストールしておいてください。

1. ビルド
```
    docker-compose build
```    
2. コンテナ起動（実行）
```
    docker-compose up 
``` 
3. コンテナ停止
```
    docker-compose down
``` 

## タスクスケジューラーへの登録例

### Windows

<img src="https://user-images.githubusercontent.com/36285803/196444072-e66561c1-3a5c-4283-b716-6a585de4214e.png" width="300px">
<img src="https://user-images.githubusercontent.com/36285803/196453824-31f7c7eb-da2b-42f6-a0ee-c43592991a90.png" width="300px">
<img src="https://user-images.githubusercontent.com/36285803/196449854-e6be6ccf-18be-4c35-933d-b4cac32a99c4.png" width="300px">
<img src="https://user-images.githubusercontent.com/36285803/196449993-050cc5c9-9e0c-4a35-abb5-db230a93dc17.png" width="300px">

### Mac・Linux
```
    crontab -e
```

```
    CRON_TZ="Japan"
    [mm] [hh] * * * cd [homeディレクトリからの格納ディレクトリ]; dotnet IncentiveCheckerforDemaekan.dll 
```

### Docker
```
    crontab -e
```

```
    CRON_TZ="Japan"
    [mm] [hh] * * * cd [homeディレクトリからの格納ディレクトリ]; docker-compose up
```
