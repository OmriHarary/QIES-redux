using System.IO;

namespace QIES.Backoffice.Parser.Files
{
    public class ParserInputFile : IParserInputFile
    {
        public string Path { get; init; }

        public ParserInputFile(string path)
        {
            Path = path;
        }

        public string[] ReadAllLines() => File.ReadAllLines(Path);

        public void Create() => File.CreateText(Path).Close();
    }
}
