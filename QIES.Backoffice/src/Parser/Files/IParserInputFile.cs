namespace QIES.Backoffice.Parser.Files
{
    public interface IParserInputFile
    {
        public string Path { get; init; }

        public string[] ReadAllLines();
        public void Create();
    }
}
