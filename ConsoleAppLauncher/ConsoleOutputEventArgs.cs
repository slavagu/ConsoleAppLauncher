using System;

namespace SlavaGu.ConsoleAppLauncher
{
    public class ConsoleOutputEventArgs : EventArgs
    {
        public ConsoleOutputEventArgs(string line, bool isError)
        {
            Line = line;
            IsError = isError;
        }

        public string Line { get; private set; }
        public bool IsError { get; private set; }
    }
}