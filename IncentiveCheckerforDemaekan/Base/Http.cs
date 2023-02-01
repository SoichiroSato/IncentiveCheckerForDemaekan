using System.Net.Http.Headers;

namespace IncentiveCheckerForDemaekan.Base
{
    /// <summary>
    /// http操作クラス
    /// </summary>
    public class Http
    {
        /// <summary>
        /// HttpClientオブジェクト
        /// </summary>
        public HttpClient HttpClient { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Http(int timeout = 100000)
        {
            HttpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMilliseconds(timeout)
            };
        }

        /// <summary>
        /// コンストラクタ
        /// CookieとかAllowRedirectとかはここで設定する
        /// </summary>
        public Http(HttpMessageHandler httpMessageHandler, int timeout = 100000)
        {
            HttpClient = new HttpClient(httpMessageHandler)
            {
                Timeout = TimeSpan.FromMilliseconds(timeout)
            };
        }

        /// <summary>
        /// Getリクエストをする
        /// </summary>
        /// <param name="url">リクエスト先</param>
        /// <param name="authenticationHeaderValue">basic認証</param>
        /// <returns>レスポンス結果</returns>
        public async Task<HttpResponseMessage> GetRequestAsync(string url, AuthenticationHeaderValue? authenticationHeaderValue = null)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            if (authenticationHeaderValue != null)
            {
                request.Headers.Authorization = authenticationHeaderValue;
            }
            var res = await HttpClient.SendAsync(request);
            return res;
        }

        /// <summary>
        /// Postリクエストをする
        /// </summary>
        /// <param name="url">リクエスト先</param>
        /// <param name="contentType">ContentType</param>
        /// <param name="authenticationHeaderValue">basic認証</param>
        /// <returns>レスポンス結果</returns>
        public async Task<HttpResponseMessage> PostRequestAsync(string url, HttpContent content, AuthenticationHeaderValue? authenticationHeaderValue = null)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(url),

            };
            if (authenticationHeaderValue != null)
            {
                request.Headers.Authorization = authenticationHeaderValue;
            }
            request.Content = content;
            var res = await HttpClient.SendAsync(request);
            return res;
        }
    }
}