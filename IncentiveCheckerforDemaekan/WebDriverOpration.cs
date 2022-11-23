using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// 出前館 市区町村別ブースト情報サイト操作クラス
    /// </summary>
    public class WebDriverOpration : WebDriver
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="options">choromeオプションに設定する文字列配列</param>
        public WebDriverOpration(string[]? options = null, double wait = 0) :base(options, wait) {}

        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから明日のインセンティブ情報を取得する
        /// </summary>
        /// <param name="area">エリア</param>
        /// <param name="prefecture">都道府県</param>
        /// <param name="city">市区町村</param>
        /// <returns>インセンティブ情報</returns>
        public Dictionary<string,string> GetInsentiveInfo(string area,string prefecture,string city,DateTime targetDate)
        {
            Driver.Navigate().GoToUrl("https://cdn.demae-can.com/contents/driver/boost/area/index.html");
            Driver.ExecuteScript("const newProto = navigator.__proto__;delete newProto.webdriver;navigator.__proto__ = newProto;");
            Driver.FindElement(By.Id("datepicker")).Clear();
            Driver.FindElement(By.Id("datepicker")).SendKeys(targetDate.ToString("yyyy-MM-dd"));
            new SelectElement(Driver.FindElement(By.Id("area"))).SelectByText(area);
            new SelectElement(Driver.FindElement(By.Id("prefecture"))).SelectByText(prefecture);
            var table = Driver.FindElement(By.Id("resultmap"));
            var thead = table.FindElement(By.Id("resulthead"));
            var columns = thead.FindElements(By.TagName("th"));
            var tbody = table.FindElement(By.Id("resultbody"));
            var rows = tbody.FindElements(By.TagName("tr"));
            var dic = new Dictionary<string, string>();
            foreach (var row in rows)
            {
                var td = row.FindElements(By.TagName("td"));
                //column[0]は市区町村
                if (td[0].Text != city) { continue; }
                //td[0]とcolumn[0]は読み取らない
                for (int i =1;i<td.Count;i++)
                {
                    dic.Add(columns[i].Text, td[i].Text);
                }
                if(dic.Count > 0) { break; }
            }
            return dic;
        }
    }
}
