using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using NUnit.Framework;
using SlavaGu.ConsoleAppLauncher;

namespace ConsoleAppLauncher.Tests
{
    [TestFixture]
    public class ConsoleAppTest
    {
        private static IConsoleApp GetDummyApp(string outputLine, int repeat, int delay, bool unstoppable, string prompt = null)
        {
            var cmdLine = string.Format("-output=\"{0}\" -repeat={1} -delay={2} -unstoppable={3} -prompt=\"{4}\"", outputLine, repeat, delay, unstoppable, prompt);
            var currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return new ConsoleApp(Path.Combine(currentDirectory, "DummyConsoleApplication.exe"), cmdLine);
        }

        [Test]
        public void should_get_all_lines_with_slow_console_output()
        {
            // Arrange
            const int repeat = 10;
            const int delay = 100;
            const string outputLine = "Line #";
            const bool unstoppable = false;

            var app = GetDummyApp(outputLine, repeat, delay, unstoppable);
            var expected = Enumerable.Range(1, repeat).Select(i => outputLine.Replace("#", i.ToString(CultureInfo.InvariantCulture))).ToArray();
            var actual = new List<string>();
            app.ConsoleOutput += (sender, args) => actual.Add(args.Line);

            // Act
            app.Run();
            var exited = app.WaitForExit(10000);

            // Assert
            Assert.IsTrue(exited, "App hasn't exited within allowed timeout");
            Assert.AreEqual(0, app.ExitCode, "Unexpected exit code");
            Assert.IsTrue(expected.SequenceEqual(actual), "Captured output is not as expected");
        }

        [Test]
        public void should_get_all_lines_with_fast_console_output()
        {
            // Arrange
            const int repeat = 1000;
            const int delay = 0;
            const string outputLine = "Line #";
            const bool unstoppable = false;

            var app = GetDummyApp(outputLine, repeat, delay, unstoppable);
            var expected = Enumerable.Range(1, repeat).Select(i => outputLine.Replace("#", i.ToString(CultureInfo.InvariantCulture))).ToArray();
            var actual = new List<string>();
            app.ConsoleOutput += (sender, args) => actual.Add(args.Line);

            // Act
            app.Run();
            var exited = app.WaitForExit(10000);

            // Assert
            Assert.IsTrue(exited, "App hasn't exited within allowed timeout");
            Assert.AreEqual(0, app.ExitCode, "Unexpected exit code");
            Assert.IsTrue(expected.SequenceEqual(actual), "Captured output is not as expected");
        }

        [Test]
        public void should_stop_properly_while_running()
        {
            // Arrange
            const int repeat = 1000;
            const int delay = 100;
            const string outputLine = "Line #";
            const bool unstoppable = false;

            var app = GetDummyApp(outputLine, repeat, delay, unstoppable);
            var actual = new List<string>();
            app.ConsoleOutput += (sender, args) => actual.Add(args.Line);

            // Act
            app.Run();
            Thread.Sleep(500);
            app.Stop(ConsoleSpecialKey.ControlC, 500);
            var exited = app.WaitForExit(10000);

            // Assert
            Assert.IsTrue(exited, "App hasn't exited within allowed timeout");
            Assert.AreEqual(-1, app.ExitCode, "App should have been terminated by CTRL-C");
            Assert.IsTrue(actual.Any(), "No output captured");
            Assert.AreNotEqual(repeat, actual.Count(), "App hasn't been stopped");
        }

        [Test]
        public void should_abort_if_nonstoppable()
        {
            // Arrange
            const int repeat = 1000;
            const int delay = 100;
            const string outputLine = "Line #";
            const bool unstoppable = true;

            var app = GetDummyApp(outputLine, repeat, delay, unstoppable);
            var actual = new List<string>();
            app.ConsoleOutput += (sender, args) => actual.Add(args.Line);

            // Act
            app.Run();
            Thread.Sleep(500);
            app.Stop(ConsoleSpecialKey.ControlC, 500);
            var exited = app.WaitForExit(10000);

            // Assert
            Assert.IsTrue(exited, "App hasn't exited within allowed timeout");
            Assert.AreEqual(-1, app.ExitCode, "App should have been terminated forcefully");
            Assert.IsTrue(actual.Any(), "No output captured");
            Assert.AreNotEqual(repeat, actual.Count(), "App hasn't been stopped");
        }

        [Test]
        public void should_get_input_redirect()
        {
            // Arrange
            const int repeat = 0;
            const int delay = 0;
            const string outputLine = "Line #";
            const bool unstoppable = false;
            const string prompt = "Enter anything and press Enter...";

            var app = GetDummyApp(outputLine, repeat, delay, unstoppable, prompt);
            var expected = new List<string> { prompt, "Hello, World!" };
            var actual = new List<string>();
            app.ConsoleOutput += (sender, args) => actual.Add(args.Line);

            // Act
            app.Run();
            app.Write("Hello, ");
            app.WriteLine("World!");
            var exited = app.WaitForExit(10000);

            // Assert
            Assert.IsTrue(exited, "App hasn't exited within allowed timeout");
            Assert.AreEqual(0, app.ExitCode, "Unexpected exit code");
            Assert.IsTrue(expected.SequenceEqual(actual), "Captured output is not as expected");
        }

    }
}
