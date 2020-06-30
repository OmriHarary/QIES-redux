using System;

namespace QIES.Frontend.Session
{
    public class Input
    {
        public string Prompt
        {
            get => prompt;
            set => prompt = $"[{value}] ";
        }
        private string prompt;

        public Input(string prompt)
        {
            this.Prompt = prompt;
        }

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
    }
}
