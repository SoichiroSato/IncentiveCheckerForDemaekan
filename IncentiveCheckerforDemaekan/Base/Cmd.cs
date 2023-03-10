using System.Diagnostics;

namespace IncentiveCheckerforDemaekan.Base
{
    /// <summary>
    /// 外部アプリケーション実行クラス
    /// </summary>
    public class Cmd : IDisposable
    {
        /// <summary>
        /// プロセスフィールド
        /// Process.Startがnullを返すケースがあるためusingが使えないので
        /// IDisposeで対応する
        /// </summary>
        public Process Process { get; set; }

        /// <summary>
        /// コマンドで外部プロセスを起動する
        /// </summary>
        /// <param name="processStartInfo">実行するコマンド</param>
        public string ExecuteFile(ProcessStartInfo processStartInfo)
        {
            Process = Process.Start(processStartInfo);
            string res = Process.StandardOutput.ReadToEnd();
            //実行が終わるまで待機
            Process.WaitForExit();
            return res;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose基底処理
        /// </summary>
        /// <param name="disposing">disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Process != null)
                {
                    Process.Dispose();
                }
            }
        }
    }
}
