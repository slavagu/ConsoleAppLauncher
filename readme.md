# Introduction

ConsoleAppLauncher is a Windows Console Application wrapper for .NET written in C#.

It captures all the output generated in the console and provides simple 
interface to start and close console application. The ConsoleOutput 
event is fired every time when a new line is written by the console to 
standard/error output. The lines are queued and guaranteed to follow the 
output order. 

For rationale [see this SO question](http://stackoverflow.com/questions/186822/capturing-console-output-from-a-net-application-c).

# Samples

Usage samples are available in the ConsoleAppLauncher.Samples project. 

        // Run simplest shell command and return its output.
        public static string GetWindowsVersion()
        {
            return ConsoleApp.Run("cmd", "/c ver").Output.Trim();
        }

        // Run ipconfig.exe and return matching line.
        public static string GetIpAddress()
        {
            var output = ConsoleApp.Run("ipconfig").Output;
            var match = Regex.Match(output, "IPv4 Address.*: (?<addr>.*)");
            return match.Success ? match.Groups["addr"].Value : "<undefined>";
        }

        // Run ping.exe asynchronously and return roundtrip times in a callback
	// sample call:
        //    Action<string> action = Console.WriteLine;
        //    PingUrl("google.com", action);         
        public static void PingUrl(string url, Action<string> replyHandler)
        {
            var regex = new Regex("(time=|Average = )(?<time>.*?ms)", RegexOptions.Compiled);
            var app = new ConsoleApp("ping", url);
            app.ConsoleOutput += (o, args) =>
            {
                var match = regex.Match(args.Line);
                if (match.Success)
                {
                    var roundtripTime = match.Groups["time"].Value;
                    replyHandler(roundtripTime);
                }
            };
            app.Run();
            app.WaitForExit();
        }

        // Get Windows firewall rule
        public static string GetFirewallRule(string ruleName)
        {
            var cmdLine = string.Format("advfirewall firewall show rule \"{0}\" verbose", ruleName);
            return ConsoleApp.Run("netsh", cmdLine).Output.Trim();
        }


# License

This library is released under The MIT License (MIT).

	Copyright (c) 2013 Slava Guzenko 

	Permission is hereby granted, free of charge, to any person obtaining a 
	copy of this software and associated documentation files (the 
	"Software"), to deal in the Software without restriction, including 
	without limitation the rights to use, copy, modify, merge, publish, 
	distribute, sublicense, and/or sell copies of the Software, and to 
	permit persons to whom the Software is furnished to do so, subject to 
	the following conditions: 

	The above copyright notice and this permission notice shall be included 
	in all copies or substantial portions of the Software. 

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
	OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
	MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
	IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
	CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
	TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
	SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 
