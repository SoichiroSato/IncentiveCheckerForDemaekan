using Microsoft.VisualBasic.FileIO;

namespace IncentiveCheckerforDemaekan
{
    internal class File
    {
        public static List<string[]> ReadTargetPlace(string path)
        {
            using (TextFieldParser txtParser = new(path))
            {
                List<string[]> ret = new();   
                txtParser.SetDelimiters(",");
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
