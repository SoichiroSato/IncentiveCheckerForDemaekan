using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// ファイル操作クラス
    /// </summary>
    public class File : IDisposable
    {
        /// <summary>
        /// csvファイルを読み取る
        /// </summary>
        /// <param name="path">ファイルのフルパス</param>
        /// <param name="index">読み取りスタート行のインデックス</param>
        /// <returns>ファイルの中身</returns>
        public static List<string[]> ReadTargetPlace(string path,int index = 0)
        {
            using var txtParser = new TextFieldParser(path);
            var ret = new List<string[]>();
            txtParser.SetDelimiters(",");
            for(int i = 0; i < index; i++)
            {
                txtParser.ReadFields();
            }
            while (!txtParser.EndOfData)
            {
                string[]? value = txtParser.ReadFields();
                if (value != null)
                {
                    ret.Add(value);
                }
            }
            return ret;
        }

        /// <summary>
        /// プロセスフィールド
        /// Process.Startがnullを返すケースがあるためusingが使えないので
        /// Idisposeで対応する
        /// </summary>
        public Process? Process { get; set; }

        /// <summary>
        /// コマンドで外部プロセスを起動する
        /// </summary>
        /// <param name="arguments">実行ファイルのコマンド</param>
        /// <param name="redirectStandardInput">入力書き込み</param>
        /// <param name="redirectStandardOutput">出力読み取り</param>
        /// <param name="useShellExecute">管理者権限</param>
        /// <param name="createNoWindow">ウィンドウ表示</param>
        public void ExcuteFile(string arguments, bool redirectStandardInput = false, bool redirectStandardOutput = true, bool useShellExecute = false, bool createNoWindow = true)
        {
            var ProcessStartInfo = new ProcessStartInfo
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
