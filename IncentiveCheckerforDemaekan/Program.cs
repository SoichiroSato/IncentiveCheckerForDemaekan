using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Data;
using System.Configuration;

namespace IncentiveCheckerforDemaekan
{
    class Program
    {
        public static bool AsyncFlg = ReadAppSettings();

        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから
        /// csvファイル記載地域の明日のインセンティブ情報を取得して
        /// Line通知を行なう
        /// </summary>
        /// <param name="args">Lineアクセストークン</param>
        static async Task<int> Main(string[] args)
        {
            string message;
            int resCode;
            string locationPath = GetCurrentPath();
            try
            {
                if (AsyncFlg)
                {
                    ExitsFile(locationPath);
                }
                else
                {
                    await ExitsFileAsync(locationPath);
                }
                CheckBrowser(locationPath);
                message = await MakeSendMessageAsync(locationPath);
                resCode = 0;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                resCode = 1;
            }
            if (args.Length > 0)
            {
                resCode = await SendLine(args[0], message, resCode);
            }
            else if (File.Exists(Path.Combine(locationPath, "LineToken.txt")))
            {
                var accessToken = new FileOparate(locationPath).ReadTxt("LineToken.txt");
                resCode = await SendLine(accessToken, message, resCode);

            }
            return resCode;
        }

        public static bool ReadAppSettings()
        {
            string? AsyncSetting = ConfigurationManager.AppSettings["async"]?.ToLower();
            return AsyncSetting != null && bool.Parse(AsyncSetting);
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
            try
            {
                await new Line(accessToken).SendMessage(message);
            }
            catch { resCode = 1; }

            return resCode;
        }

        /// <summary>
        /// 必要なファイルがあるか確認してなかったらファイルを作る
        /// </summary>
        /// <param name="locationPath">カレントディレクトリ</param>
        private static void ExitsFile(string locationPath)
        {
            var fileOprate = new FileOparate(locationPath);
            if (!File.Exists(Path.Combine(locationPath, "ChromeInstall.bat")))
            {
                fileOprate.WriteFile("ChromeInstall.bat", FileContents.ChromeInstall());
            }
            if (!File.Exists(Path.Combine(locationPath, "TargetPlace.csv")))
            {
                fileOprate.WriteFile("TargetPlace.csv", FileContents.TargetPlace());
            }
            if (!File.Exists(Path.Combine(locationPath, "LineToken.txt")))
            {
                fileOprate.WriteFile("LineToken.txt", "");
            }
        }

        /// <summary>
        /// 必要なファイルがあるか確認してなかったらファイルを作る
        /// </summary>
        /// <param name="locationPath">カレントディレクトリ</param>
        private static async Task ExitsFileAsync(string locationPath)
        {
            var fileOprate = new FileOparate(locationPath);
            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    if (!File.Exists(Path.Combine(locationPath, "ChromeInstall.bat")))
                    {
                        fileOprate.WriteFile("ChromeInstall.bat", FileContents.ChromeInstall());
                    }
                }),
                Task.Run(() =>
                {
                    if (!File.Exists(Path.Combine(locationPath, "TargetPlace.csv")))
                    {
                        fileOprate.WriteFile("TargetPlace.csv", FileContents.TargetPlace());
                    }
                }),
                Task.Run(() =>
                {
                    if (!File.Exists(Path.Combine(locationPath, "LineToken.txt")))
                    {
                        fileOprate.WriteFile("LineToken.txt", "");
                    }
                })
            };
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Chromeがインストールされているか確認しなかったらインストールする
        /// </summary>
        /// <param name="locationPath"></param>
        private static void CheckBrowser(string locationPath)
        {
            var browsers = Browser.GetInstallBrowser();
            if (!browsers.Contains("Google Chrome"))
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    Browser.InstallChromeWindows(Path.Combine(locationPath, "ChromeInstall.bat"));
                }
            }
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
        private static async Task<string> MakeSendMessageAsync(string locationPath)
        {
            var fileOparate = new FileOparate(locationPath);
            var targetPlace = fileOparate.ReadTargetPlace("TargetPlace.csv");
            var targetDate = DateTime.Now.AddDays(1);
            var map = AsyncFlg ? MakeIncentiveMap(targetPlace, targetDate) : await MakeIncentiveMapAsync(targetPlace, targetDate);
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
        ///  csvファイル記載地域の明日のインセンティブ情報を取得してMap化する
        /// </summary>
        /// <param name="targetPlace"> csvファイル記載地域</param>
        /// <returns>csvファイル記載地域すべてのインセンティブ情報</returns>
        private static Dictionary<string, Dictionary<string, string>> MakeIncentiveMap(DataTable targetPlace, DateTime targetDate)
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
            using var webDriver = new WebDriverOpration(options.ToArray(),10);
            var map = new Dictionary<string, Dictionary<string, string>>();
            using var reader = targetPlace.CreateDataReader();
            while (reader.Read())
            {
                map.Add((string)reader["都道府県"] + (string)reader["市区町村"], webDriver.GetInsentiveInfo((string)reader["エリア"], (string)reader["都道府県"], (string)reader["市区町村"],targetDate));
            }
            return map;
        }

        /// <summary>
        ///  csvファイル記載地域の明日のインセンティブ情報を取得してMap化する
        /// </summary>
        /// <param name="targetPlace"> csvファイル記載地域</param>
        /// <returns>csvファイル記載地域すべてのインセンティブ情報</returns>
        private static async Task<Dictionary<string, Dictionary<string, string>>> MakeIncentiveMapAsync(DataTable targetPlace, DateTime targetDate)
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
            var options = new List<string>()
            {
                "--headless",
                "--incognito",
                "--start-maximized",
                "--blink-settings=imagesEnabled=false",
                "--lang=ja",
                "--proxy-server='direct://'",
                "--proxy-bypass-list=*"
            };
            using var webDriver = new WebDriverOpration(options.ToArray(), 10);
            map.Add(prefecture + city, webDriver.GetInsentiveInfo(area, prefecture, city, targetDate));
        }
    }
}

