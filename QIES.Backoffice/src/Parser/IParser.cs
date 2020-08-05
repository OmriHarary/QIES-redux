namespace QIES.Backoffice.Parser
{
    public interface IParser<T>
    {
        public bool TryParseFile(string filePath, T output);
    }
}
