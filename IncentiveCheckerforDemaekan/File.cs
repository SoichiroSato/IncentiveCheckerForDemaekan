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
    }
}
