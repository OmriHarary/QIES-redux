using System;

namespace QIES.Cli.Session
{
    public class Input
    {
        public string Prompt
        {
            get => prompt;
            set => prompt = $"[{value}]  ";
        }
        private string prompt;

        public Input(string prompt) => this.Prompt = prompt;

        public string TakeInput(string message)
        {
            Console.WriteLine(message);
            Console.Write(Prompt);
            string input = Console.ReadLine();
            if (input == null)
            {
                // FIXME: This is just to immitate the behaviour of java.util.Scanner.nextLine()
                //          when the input stream ends, since for some reason the original tests
                //          depend on it.
                throw new System.IO.EndOfStreamException();
            }
            return input;
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
