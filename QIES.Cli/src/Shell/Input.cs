using System;

namespace QIES.Cli.Shell
{
    public class Input
    {
        public string Prompt { get; set; }

        public Input(string prompt) => Prompt = prompt;

        public string TakeInput(string message)
        {
            Console.WriteLine(message);
            Console.Write($"[{Prompt}]  ");
            return Console.ReadLine() ?? string.Empty;
        }

        public int TakeNumericInput(string message)
        {
            var strIn = TakeInput(message);
            if (!int.TryParse(strIn.Trim(), out int result))
            {
                throw new System.IO.InvalidDataException();
            }
            return result;
        }
    }
}
