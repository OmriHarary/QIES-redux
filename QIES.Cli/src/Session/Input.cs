using System;

namespace QIES.Cli.Session
{
    public class Input
    {
        public string Prompt { get; set; }

        public Input(string prompt) => this.Prompt = prompt;

        public string TakeInput(string message)
        {
            Console.WriteLine(message);
            Console.Write("[{Prompt}]  ");
            return Console.ReadLine() ?? string.Empty;
        }

        public int TakeNumericInput(string message)
        {
            var strIn = TakeInput(message);
            int result;
            if (!int.TryParse(strIn.Trim(), out result))
            {
                throw new System.IO.InvalidDataException();
            }
            return result;
        }
    }
}
