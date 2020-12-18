using QIES.Backoffice.Parser.Files;

namespace QIES.Backoffice.Parser
{
    public interface IParser<T>
    {
        public bool TryParseFile(IParserInputFile file, T output);
    }
}
