using System.Configuration;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// App.config関係処理
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// AppSettingsのvalue値を取得する
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>value</returns>

        public static string GetAppSettingsValue(string key)
        {
            string? value = ConfigurationManager.AppSettings[key];
            return value == null ? "" : value;
        }
    }
}
