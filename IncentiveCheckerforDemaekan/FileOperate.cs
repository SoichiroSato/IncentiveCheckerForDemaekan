using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Text;

namespace IncentiveCheckerforDemaekan;

/// <summary>
/// ファイル操作クラス
/// </summary>
public class FileOperate
{
    /// <summary>
    /// 実行ディレクトリ
    /// </summary>
    public string LocationPath { get; set; }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="locationPath"></param>
    public FileOperate(string locationPath)
    {
        LocationPath = locationPath;
    }

    /// <summary>
    /// csvファイルを読み取る
    /// </summary>
    /// <param name="file">ファイルのフルパス</param>
    /// <returns>ファイルの中身</returns>
    public DataTable ConvertCsvToDataTable(string file)
    {
        using var txtParser = new TextFieldParser(Path.Combine(LocationPath, file));
        var ret = new DataTable();
        txtParser.SetDelimiters(",");
        IEnumerable<string> columns = txtParser.ReadFields() ?? Enumerable.Empty<string>();
        columns.ToList().ForEach( column => ret.Columns.Add(column));
        while (!txtParser.EndOfData)
        {
            string[] values = txtParser.ReadFields();
            if (values is null) { continue; }
            var row = ret.NewRow();
            ret.Rows.Add(values);
        }
        return ret;
    }

    /// <summary>
    /// txtファイルを読み込んで記載内容を返す
    /// </summary>
    /// <param name="file">ファイルのフルパス</param>
    /// <param name="encoding">エンコードタイプ</param>
    /// <returns>記載内容</returns>
    public string ReadFile(string file, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        using var reader = new StreamReader(Path.Combine(LocationPath, file), encoding);
        return reader.ReadToEnd();
    }

    /// <summary>
    /// ファイルを出力する
    /// </summary>
    /// <param name="file">ファイル名</param>
    /// <param name="contents">ファイルの中身</param>
    /// <param name="append">上書きか追記か</param>
    /// <param name="encoding">エンコードタイプ</param>
    public void WriteFile(string file, string contents,bool append = false, Encoding encoding = null)
    {
        encoding ??= Encoding.UTF8;
        using var streamWriter = new StreamWriter(Path.Combine(LocationPath, file),append, encoding);
        streamWriter.Write(contents);
    }
}

