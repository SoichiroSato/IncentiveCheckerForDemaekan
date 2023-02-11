using System.Text;
using System.Reflection;
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
        private static readonly string LocationPath = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";

        /// <summary>
        /// ChromeOptionの配列
        /// </summary>
        /// <value></value>
        private static readonly string[] ChromeOptions = 
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
            try
            {
                CheckBrowser();
                message = await CreateSendMessageAsync();
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
            else
            {
                var accessToken = new FileOperate(LocationPath).ReadFile("./File/LineToken.txt");
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
            try
            { 
                await new Line(accessToken).SendMessage(message); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                resCode = 1; 
            }
            return resCode;
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
        /// 出前館 市区町村別ブースト情報サイトから
        /// csvファイル記載地域の明日のインセンティブ情報を取得して
        /// Line通知メッセージを作成する
        /// </summary>
        /// <returns>Line通知メッセージ</returns>
        private static async Task<string> CreateSendMessageAsync()
        {
            var fileOperate = new FileOperate(LocationPath);
            var targetPlace = fileOperate.ConvertCsvToDataTable("./File/TargetPlace.csv");
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
            using var webDriver = new WebDriverOperation(ChromeOptions, 10);
            var map = new Dictionary<string, Dictionary<string, string>>();
            using var reader = targetPlace.CreateDataReader();
            while (reader.Read())
            {
                var area = (string)reader["エリア"];
                var prefecture = (string)reader["都道府県"];
                var city = (string)reader["市区町村"];
                map.Add(prefecture + city, webDriver.GetIncentiveInfo(area, prefecture, city, targetDate));
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
            using var webDriver = new WebDriverOperation(ChromeOptions, 10);
            map.Add(prefecture + city, webDriver.GetIncentiveInfo(area, prefecture, city, targetDate));
        }
    }
}

