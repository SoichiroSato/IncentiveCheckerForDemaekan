using Microsoft.VisualBasic.FileIO;
using System.Text;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// ファイル操作クラス
    /// </summary>
    public class FileOparate
    {
        /// <summary>
        /// 実行ディレクトリ
        /// </summary>
        public string LocationPath { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="locationPath"></param>
        public FileOparate(string locationPath)
        {
            LocationPath = locationPath;
        }

        /// <summary>
        /// csvファイルを読み取る
        /// </summary>
        /// <param name="file">ファイルのフルパス</param>
        /// <param name="index">読み取りスタート行のインデックス</param>
        /// <returns>ファイルの中身</returns>
        public List<string[]> ReadTargetPlace(string file,int index = 0)
        {
            using var txtParser = new TextFieldParser(Path.Combine(LocationPath, file));
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
        /// ファイルを出力する
        /// </summary>
        /// <param name="file">ファイル名</param>
        /// <param name="contents">ファイルの中身</param>
        /// <param name="append">上書きか追記か</param>
        /// <param name="encoding">エンコードタイプ</param>
        public void WriteFile(string file, string contents,bool append = false, Encoding? encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.UTF8;
            }
            using var streamWriter = new StreamWriter(Path.Combine(LocationPath, file),append, encoding);
            streamWriter.Write(contents);
        }
    }
}
