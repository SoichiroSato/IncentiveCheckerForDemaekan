using System.Text;
using System.Reflection;

namespace IncentiveCheckerforDemaekan
{
    class Program
    {
        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから
        /// csvファイル記載地域の明日のインセンティブ情報を取得して
        /// Line通知を行なう
        /// </summary>
        /// <param name="args">Lineアクセストークン</param>
        public static void Main(string[] args)
        {
            string message ;
            try
            {
                message = MakeSendMessage();
            }
            catch(Exception ex)
            {
                message = ex.Message;
            }

            Task task = new Line(args[0]).SendMessage(message);
            task.Wait();
        }

        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから
        /// csvファイル記載地域の明日のインセンティブ情報を取得して
        /// Line通知メッセージを作成する
        /// </summary>
        /// <returns>Line通知メッセージ</returns>
        public static string MakeSendMessage()
        {
            var locationPath = @Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (locationPath != null)
            {
                List<string> options = new()
                {
                    "--headless",
                    "--incognito",
                    "--start-maximized",
                    "--blink-settings=imagesEnabled=false",
                    "--lang=ja",
                    "--proxy-server='direct://'",
                    "--proxy-bypass-list=*",
                    "--proxy-bypass-list=*"
                };
                Dictionary<string, Dictionary<string, string>> map = new();
                using (WebDriverOpration webDriver = new(options.ToArray()))
                {

                    string filePath = Path.Combine(locationPath, "TargetPlace.csv");
                    List<string[]> targetPlace = File.ReadTargetPlace(filePath);
                    
                    foreach (string[] address in targetPlace)
                    {
                        map.Add(address[1]+address[2],webDriver.GetInsentiveInfo(address[0], address[1], address[2]));
                    }
                }
                StringBuilder stringBuilder = new();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine(DateTime.Now.AddDays(1).ToString("MM/dd") + "のインセンティブ情報");
                stringBuilder.AppendLine();
                foreach (var(address, incentive) in map)
                {
                    stringBuilder.AppendLine(address);
                    foreach(var(time, magnification) in incentive)
                    {
                        stringBuilder.AppendLine($"{time}:{magnification}");
                    }
                    stringBuilder.AppendLine();
                }
                return stringBuilder.ToString();    
            }
            return "";
        }
    }
}

