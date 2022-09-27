using System.Diagnostics;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// 外部アプリケーション実行クラス
    /// </summary>
    public class Cmd :IDisposable
    {
        /// <summary>
        /// プロセスフィールド
        /// Process.Startがnullを返すケースがあるためusingが使えないので
        /// Idisposeで対応する
        /// </summary>
        public Process? Process { get; set; }

        /// <summary>
        /// プロセスインフォ
        /// </summary>
        public ProcessStartInfo ProcessStartInfo { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="arguments">実行ファイルのコマンド</param>
        /// <param name="redirectStandardInput">入力書き込み</param>
        /// <param name="redirectStandardOutput">出力読み取り</param>
        /// <param name="useShellExecute">管理者権限</param>
        /// <param name="createNoWindow">ウィンドウ表示</param>
        public Cmd(string arguments, bool redirectStandardInput = false, bool redirectStandardOutput = true, bool useShellExecute = false, bool createNoWindow = true)
        {
            ProcessStartInfo = new ProcessStartInfo
            {
                FileName = Environment.GetEnvironmentVariable("ComSpec"),
                Arguments = arguments,
                RedirectStandardInput = redirectStandardInput,
                RedirectStandardOutput = redirectStandardOutput,
                UseShellExecute = useShellExecute,
                CreateNoWindow = createNoWindow
            };
            if (useShellExecute)
            {
                //管理者権限で実行するときのおまじない
                ProcessStartInfo.Verb = "runas";
            }
        }

        /// <summary>
        /// コマンドで外部プロセスを起動する
        /// </summary>
       
        public void ExcuteFile()
        {
            Process = Process.Start(ProcessStartInfo);
            if (Process != null)
            {
                //実行が終わるまで待機
                Process.WaitForExit();
            }
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
