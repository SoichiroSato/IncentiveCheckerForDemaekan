using Microsoft.VisualBasic.FileIO;

namespace IncentiveCheckerforDemaekan
{
    /// <summary>
    /// ファイル操作クラス
    /// </summary>
    public class File
    {
        /// <summary>
        /// csvファイルを読み取る
        /// </summary>
        /// <param name="path">ファイルのフルパス</param>
        /// <returns>ファイルの中身</returns>
        public static List<string[]> ReadTargetPlace(string path)
        {
            using (TextFieldParser txtParser = new(path))
            {
                List<string[]> ret = new();   
                txtParser.SetDelimiters(",");
                //一行目はヘッダー
                txtParser.ReadFields();
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
        }
    }
}
