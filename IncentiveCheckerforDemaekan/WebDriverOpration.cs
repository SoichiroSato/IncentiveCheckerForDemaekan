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
        public WebDriverOpration(string[]? options = null) :base(options){}

        /// <summary>
        /// 出前館 市区町村別ブースト情報サイトから明日のインセンティブ情報を取得する
        /// </summary>
        /// <param name="area">エリア</param>
        /// <param name="prefecture">都道府県</param>
        /// <param name="city">市区町村</param>
        /// <returns>インセンティブ情報</returns>
        public Dictionary<string,string> GetInsentiveInfo(string area,string prefecture,string city)
        {
            Driver.Navigate().GoToUrl("https://cdn.demae-can.com/contents/driver/boost/area/index.html");
            Driver.ExecuteScript("const newProto = navigator.__proto__;delete newProto.webdriver;navigator.__proto__ = newProto;");
            Driver.FindElement(By.Id("datepicker")).Clear();
            Driver.FindElement(By.Id("datepicker")).SendKeys(DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
            new SelectElement(Driver.FindElement(By.Id("area"))).SelectByText(area);
            new SelectElement(Driver.FindElement(By.Id("prefecture"))).SelectByText(prefecture);
            new SelectElement(Driver.FindElement(By.Id("city"))).SelectByText(city);
            var table = Driver.FindElement(By.Id("resultmap"));
            var thead = table.FindElement(By.Id("resulthead"));
            var columns = thead.FindElements(By.TagName("th"));
            List<string> key = new(); 
            //column[0]は市区町村
            for(int i = 1; i < columns.Count; i++)
            {
                key.Add(columns[i].Text);
            }
            var tbody = table.FindElement(By.Id("resultbody"));
            var rows = tbody.FindElements(By.TagName("tr"));
            List<string> value = new();
            foreach (var row in rows)
            {
                var td = row.FindElements(By.TagName("td"));
                //td[0]は市区町村名
                if (td[0].Text != city) { continue; }
                for(int i =1;i<td.Count;i++)
                {
                    value.Add(td[i].Text);
                }
                if(value.Count > 0) { break; }
            }
            Dictionary<string,string> dic = new();
            if(key.Count == value.Count)
            {
                for(int i = 0; i < value.Count; i++)
                {
                    if (double.TryParse(value[i], out double val) && val > 1.0)
                    {
                        dic.Add(key[i], value[i]);
                    }
                }
            }
            return dic;
        }
    }
}
