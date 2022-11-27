using System.Net.Http.Headers;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// Line操作クラス
    /// </summary>
    public class Line
    {
        /// <summary>
        /// アクセストークン
        /// </summary>
        public string AccessToken { get; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="accessToken">Lineアクセストークン</param>
        public Line(string accessToken)
        {
            AccessToken = accessToken;
        }
        /// <summary>
        /// Line通知メッセージを送信する
        /// </summary>
        /// <param name="message">送信内容</param>
        /// <returns>Line通知メッセージを送信するタスク</returns>
        public async Task SendMessage(string message)
        {
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", message } });
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            await client.PostAsync(AppConfig.GetAppSettingsValue("lineUrl"), content);
        }


    }
}
