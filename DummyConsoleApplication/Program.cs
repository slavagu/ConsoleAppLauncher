using System;
using System.Globalization;
using System.Threading;
using NDesk.Options;

namespace DummyConsoleApplication
{
    public class Program
    {
        public enum ExitCode
        {
            Success = 0,
            AbortedWithCtrlC = 1,
            AbortedWithCtrlBreak = 2,
            Unexpected = 3,
        }

        static void Main(string[] args)
        {
            string outputLine = null;
            int repeat = 10;
            int delay = 100;
            bool unstoppable = false;
            string prompt = null;

            var optionSet = new OptionSet
            {
                { "o|output=", "[required] output line format, e.g. 'Line #', where # is replaced with line number.", s => outputLine = s },
                { "r|repeat=", "number of {TIMES} to output the line.", (int v) => repeat = v },
                { "d|delay=", "number of milliseconds to {DELAY} between lines.", (int v) => delay = v },
                { "u|unstoppable=", "flag to ignore cancellation via Ctrl-Break.", (bool v) => unstoppable = v },
                { "p|prompt=", "prompt to ask for user input.", s => prompt = s },
            };

            try
            {
                optionSet.Parse(args);

                if (outputLine == null)
                    throw new OptionException("Required parameter is missing", "output");
            }
            catch (OptionException e)
            {
                Console.WriteLine("Invalid command line arguments: {0}; name: '{1}'", e.Message, e.OptionName);
                Console.WriteLine();
                Console.WriteLine("Supported arguments:");
                optionSet.WriteOptionDescriptions(Console.Out);
                return;
            }

            Console.CancelKeyPress += (sender, e) =>
            {
                if (e.SpecialKey == ConsoleSpecialKey.ControlC)
                {
                    if (unstoppable)
                    {
                        Console.WriteLine("Ctrl-C pressed, ignoring...");
                        e.Cancel = true;
                    }
                    else
                    {
                        Console.WriteLine("Ctrl-C pressed, exiting...");
                        Environment.Exit((int)ExitCode.AbortedWithCtrlC);
                    }
                }
                else if (e.SpecialKey == ConsoleSpecialKey.ControlBreak)
                {
                    Console.WriteLine("Ctrl-Break pressed, can't avoid exiting...");
                    Environment.Exit((int)ExitCode.AbortedWithCtrlBreak);
                }
                else
                {
                    Console.WriteLine("Unexpected signal {0} received, exiting...", e.SpecialKey.ToString());
                    Environment.Exit((int)ExitCode.Unexpected);
                }
            };

            if (!string.IsNullOrEmpty(prompt))
            {
                Console.WriteLine(prompt);
                char ch = (char)Console.Read();
                string str = Console.ReadLine();
                Console.WriteLine("{0}{1}", ch, str);
            }

            for (int i = 1; i <= repeat; i++)
            {
                Console.WriteLine(outputLine.Replace("#", i.ToString(CultureInfo.InvariantCulture)));
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }

            Environment.Exit((int)ExitCode.Success);
        }

    }
}
