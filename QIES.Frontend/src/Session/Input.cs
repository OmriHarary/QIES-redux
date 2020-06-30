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
            return Console.ReadLine();
        }
    }
}
