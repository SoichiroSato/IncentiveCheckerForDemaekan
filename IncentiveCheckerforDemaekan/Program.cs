using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;

namespace IncentiveCheckerforDemaekan
{
    class Program
    {
        /// <summary>
        /// 非同期か同期にするかのフラグ
        /// </summary>
        private static readonly bool AsyncFlg = bool.Parse(AppConfig.GetAppSettingsValue("async").ToLower());

        /// <summary>
        /// カレントパス
        /// </summary>
        private static readonly string LocationPath = GetCurrentPath();

        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから
        /// csvファイル記載地域の明日のインセンティブ情報を取得して
        /// Line通知を行なう
        /// </summary>
        /// <param name="args">Lineアクセストークン</param>
        static async Task<int> Main(string[] args)
        {
            Console.WriteLine(DateTime.Now);
            string message;
            int resCode;
            try
            {
                if (AsyncFlg)
                {
                    await CreateFilesAsync(); 
                }
                else
                {
                    CreateFiles();
                }

                CheckBrowser();
                message = await CreateSendMessageAsync();
                resCode = 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message;);
                message = ex.Message;
                resCode = 1;
            }
            if (args.Length > 0)
            {
                resCode = await SendLine(args[0], message, resCode);
            }
            else if(File.Exists(Path.Combine(LocationPath, "LineToken.txt")))
            {
                var accessToken = new FileOparate(LocationPath).ReadFile("LineToken.txt");
                resCode = await SendLine(accessToken, message, resCode);

            }
            return resCode;
        }

        /// <summary>
        /// Lineに結果を通知してレスポンスコードを返す
        /// </summary>
        /// <param name="accessToken">Lineアクセストークン</param>
        /// <param name="message">通知メッセージ</param>
        /// <param name="resCode">レスポンスコード</param>
        /// <returns>レスポンスコード</returns>
        private static async Task<int> SendLine(string accessToken, string message, int resCode)
        {
            try{ await new Line(accessToken).SendMessage(message); }
            catch { resCode = 1; }
            return resCode;
        }

        /// <summary>
        /// 必要なファイルがあるかそれぞれ確認してなかったらファイルを作る
        /// </summary>
        /// <param name="locationPath">カレントディレクトリ</param>
        private static void CreateFiles()
        {
            var fileOparate = new FileOparate(LocationPath);
            CreatFile(fileOparate, "ChromeInstall" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".bat" : ".sh"));
            CreatFile(fileOparate, "TargetPlace.csv");
            CreatFile(fileOparate, "LineToken.txt");
        }

        /// <summary>
        /// 必要なファイルがあるかそれぞれ確認してなかったらファイルを作る
        /// </summary>
        /// <param name="locationPath">カレントディレクトリ</param>
        private static async Task CreateFilesAsync()
        {
            var fileOparate = new FileOparate(LocationPath);
            var tasks = new List<Task>
            {
                Task.Run(() =>{CreatFile(fileOparate, "ChromeInstall" + (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".bat" : ".sh"));}),
                Task.Run(() =>{CreatFile(fileOparate, "TargetPlace.csv");}),
                Task.Run(() =>{CreatFile(fileOparate, "LineToken.txt");})
            };
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// 必要なファイルがあるか確認してなかったらファイルを作る
        /// </summary>
        /// <param name="fileOparate">FileOparateオブジェクト</param>
        /// <param name="fileName">ファイル名</param>
        private static void CreatFile(FileOparate fileOparate,string fileName)
        {
            if (File.Exists(Path.Combine(fileOparate.LocationPath, fileName))) { return; }
            string fileContents = "";
            if(fileName == "ChromeInstall.bat")
            {
                fileContents = FileContents.ChromeInstallWindows();
            }
            else if (fileName == "ChromeInstall.sh")
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    fileContents = FileContents.ChromeInstallLinux();
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    fileContents = FileContents.ChromeInstallMac();
                }
            }
            else if (fileName == "TargetPlace.csv")
            {
                fileContents = FileContents.TargetPlace();
            }
            fileOparate.WriteFile(fileName, fileContents,false, new UTF8Encoding());
        }

        /// <summary>
        /// Chromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        /// <param name="locationPath"></param>
        private static void CheckBrowser()
        {
            using var browser = new Browser(LocationPath);
            browser.InstallChrome();
        }

        /// <summary>
        /// 実行ファイルのカレントディレクトリを取得する
        /// </summary>
        /// <returns>カレントディレクトリ</returns>
        private static string GetCurrentPath()
        {
            var locationPath = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (locationPath == null) 
            { 
                return ""; 
            }else
            {
                return locationPath;
            }
        }

        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから
        /// csvファイル記載地域の明日のインセンティブ情報を取得して
        /// Line通知メッセージを作成する
        /// </summary>
        /// <returns>Line通知メッセージ</returns>
        private static async Task<string> CreateSendMessageAsync()
        {
            var fileOparate = new FileOparate(LocationPath);
            var targetPlace = fileOparate.ConvertCsvToDatatble("TargetPlace.csv");
            var targetDate = DateTime.Now.AddDays(1);
            var map = AsyncFlg ? await CreateIncentiveMapAsync(targetPlace, targetDate) : CreateIncentiveMap(targetPlace, targetDate);
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(targetDate.ToString("MM/dd") + "のインセンティブ情報");
            stringBuilder.AppendLine();
            foreach (var (address, incentive) in map)
            {
                stringBuilder.AppendLine(address);
                foreach (var (time, magnification) in incentive)
                {
                    //1.1倍以上の時間帯だけ抽出する
                    if (double.TryParse(magnification, out double val) && val > 1.0)
                    {
                        stringBuilder.AppendLine($"{time}:{val}");
                    }
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();
        }
                
        /// <summary>
        /// 各エリアのインセンティブ情報を取得してMapに追加する
        /// </summary>
        /// <param name="map">格納Map</param>
        /// <param name="area">エリア</param>
        /// <param name="prefecture">都道府県</param>
        /// <param name="city">市区町村</param>
        private static Dictionary<string, Dictionary<string, string>> CreateIncentiveMap(DataTable targetPlace, DateTime targetDate)
        {
            using var webDriver = new WebDriverOpration(CreateChromeOptionsArray(), 10);
            var map = new Dictionary<string, Dictionary<string, string>>();
            using var reader = targetPlace.CreateDataReader();
            while (reader.Read())
            {
                var area = (string)reader["エリア"];
                var prefecture = (string)reader["都道府県"];
                var city = (string)reader["市区町村"];
                map.Add(prefecture + city, webDriver.GetInsentiveInfo(area, prefecture, city, targetDate));
            }
            return map;
        }

        /// <summary>
        ///  csvファイル記載地域の明日のインセンティブ情報を取得してMap化する
        /// </summary>
        /// <param name="targetPlace"> csvファイル記載地域</param>
        /// <returns>csvファイル記載地域すべてのインセンティブ情報</returns>
        private static async Task<Dictionary<string, Dictionary<string, string>>> CreateIncentiveMapAsync(DataTable targetPlace, DateTime targetDate)
        {
            var map = new Dictionary<string, Dictionary<string, string>>();
            using var reader = targetPlace.CreateDataReader();
            var tasks = new List<Task>();
            while (reader.Read())
            {
                var area = (string)reader["エリア"];
                var prefecture = (string)reader["都道府県"];
                var city = (string)reader["市区町村"];
                tasks.Add(Task.Run(() => AddMapOfIncentive(map, area, prefecture, city, targetDate)));
            }
            await Task.WhenAll(tasks);
            return map;
        }

        /// <summary>
        /// 各エリアのインセンティブ情報を取得してMapに追加する
        /// </summary>
        /// <param name="map">格納Map</param>
        /// <param name="area">エリア</param>
        /// <param name="prefecture">都道府県</param>
        /// <param name="city">市区町村</param>
        private static void AddMapOfIncentive(Dictionary<string, Dictionary<string, string>> map, string area, string prefecture, string city, DateTime targetDate)
        {
            using var webDriver = new WebDriverOpration(CreateChromeOptionsArray(), 10);
            map.Add(prefecture + city, webDriver.GetInsentiveInfo(area, prefecture, city, targetDate));
        }

        /// <summary>
        /// ChromeOptionの配列を作成する
        /// </summary>
        /// <returns>ChromeOptionの配列</returns>
        private static string[] CreateChromeOptionsArray()
        {
            var options = new List<string>()
            {
                "--headless",
                "--no-sandbox",
                "--incognito",
                "--start-maximized",
                "--blink-settings=imagesEnabled=false",
                "--lang=ja",
                "--proxy-server='direct://'",
                "--proxy-bypass-list=*"
            };
            return options.ToArray();
        }
    }
}

