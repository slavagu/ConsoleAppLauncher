using System;
using System.Globalization;
using System.Threading;
using NDesk.Options;

namespace DummyConsoleApplication
{
    public class Program
    {
        static void Main(string[] args)
        {
            string outputLine = null;
            int repeat = 10;
            int delay = 100;
            bool unstoppable = false;

            var optionSet = new OptionSet
            {
                { "o|output=",  "[required] output line format, e.g. 'Line #', where # is replaced with line number.", s => outputLine = s },
                { "r|repeat=",  "number of {TIMES} to output the line.", (int v) => repeat = v },
                { "d|delay=", "number of milliseconds to {DELAY} between lines.", (int v) => delay = v },
                { "u|unstoppable=", "flag to ignore cancellation via Ctrl-Break.", (bool v) => unstoppable = v },
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
                Console.WriteLine("{0} key pressed, {1}...", e.SpecialKey.ToString(), unstoppable ? "ignoring" : "exiting");
                try
                {
                    e.Cancel = unstoppable;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not ignore: {0}", ex.ToString());
                }
            };

            for (int i = 1; i <= repeat; i++)
            {
                Console.WriteLine(outputLine.Replace("#", i.ToString(CultureInfo.InvariantCulture)));
                if (delay > 0)
                {
                    Thread.Sleep(delay);
                }
            }
        }

    }
}
