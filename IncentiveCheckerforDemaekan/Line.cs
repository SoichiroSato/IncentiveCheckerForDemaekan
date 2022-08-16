using System.Net.Http.Headers;

namespace IncentiveCheckerforDemaekan
{
    
    public class Line
    {
        public string AccessToken { get; }
        public Line(string accessToken)
        {
            AccessToken = accessToken;
        }

        public async Task SendMessage(string message)
        {
            using (HttpClient client = new())
            {
                var content = new FormUrlEncodedContent(new Dictionary<string, string> { { "message", message } });
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
                var result = await client.PostAsync("https://notify-api.line.me/api/notify", content);
            }
        }


    }
}
