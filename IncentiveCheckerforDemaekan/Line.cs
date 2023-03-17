using System.Net.Http.Headers;
using IncentiveCheckerforDemaekan.Base;

namespace IncentiveCheckerforDemaekan;

/// <summary>
/// Line操作クラス
/// </summary>
public class Line : Http
{
    /// <summary>
    /// アクセストークン
    /// </summary>
    public string AccessToken { get; }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="accessToken">Lineアクセストークン</param>
    public Line(string accessToken) : base()
    {
        AccessToken = accessToken;         
    }
    /// <summary>
    /// Line通知メッセージを送信する
    /// </summary>
    /// <param name="message">送信内容</param>
    /// <returns>Line通知メッセージを送信するタスク</returns>
    public async Task<HttpResponseMessage> SendMessage(string message)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", message } });
        var header = new AuthenticationHeaderValue("Bearer", AccessToken);
        return await PostRequestAsync(AppConfig.GetAppSettingsValue("lineUrl"), content, header);
    }
}

