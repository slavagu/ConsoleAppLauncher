using System;
using System.Text.RegularExpressions;

namespace SlavaGu.ConsoleAppLauncher.Samples
{
    public class SysInfo
    {
        public static string GetWindowsVersion()
        {
            return ConsoleApp.Run("cmd", "/c ver").Output.Trim();
        }

        public static string GetIpAddress()
        {
            var output = ConsoleApp.Run("ipconfig").Output;
            var match = Regex.Match(output, "IPv4 Address.*: (?<addr>.*)");
            return match.Success ? match.Groups["addr"].Value : "<undefined>";
        }

        public static void PingUrl(string url, Action<string> replyHandler)
        {
            var regex = new Regex("(time=|Average = )(?<time>.*?ms)", RegexOptions.Compiled);
            var app = new ConsoleApp("ping", "google.com");
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
        }

        public static string GetFirewallRule(string ruleName)
        {
            var cmdLine = string.Format("advfirewall firewall show rule \"{0}\" verbose", ruleName);
            return ConsoleApp.Run("netsh", cmdLine).Output.Trim();
        }
    }
}
