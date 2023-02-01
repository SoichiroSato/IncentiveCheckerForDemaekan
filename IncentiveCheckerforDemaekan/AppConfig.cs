using System.Configuration;

namespace IncentiveCheckerForDemaekan
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
            return ConfigurationManager.AppSettings[key] ?? "";
        }
    }
}
