using System;
using System.Threading;

namespace SlavaGu.ConsoleAppLauncher
{
    public enum AppState
    {
        Undefined = 0,
        Running = 1,
        Exiting = 2,
        Exited = 3,
    }

    /// <summary>
    /// Defines an interface to a console app wrapper
    /// </summary>
    public interface IConsoleApp : IDisposable
    {
        /// <summary>
        /// File name of the app, e.g. "cmd.exe"
        /// </summary>
        string FileName { get; }

        /// <summary>
        /// Command line of the app, e.g. "/c dir"
        /// </summary>
        string CmdLine { get; }

        /// <summary>
        /// Current state of the app.
        /// </summary>
        AppState State { get; }

        /// <summary>
        /// Exit code of the app.
        /// </summary>
        int? ExitCode { get; }

        /// <summary>
        /// Timestamp the app has exited.
        /// </summary>
        DateTime? ExitTime { get; }

        /// <summary>
        /// Start the app.
        /// </summary>
        void Run();

        /// <summary>
        /// Stop the app.
        /// </summary>
        /// <param name="forceCloseMillisecondsTimeout">Timeout to wait before closing the app forcefully [default=infinite]</param>
        /// <param name="forceCloseKey">Special key to send if closing the app forcefully [default=Ctrl-C]</param>
        void Stop(int forceCloseMillisecondsTimeout = Timeout.Infinite, ConsoleSpecialKey forceCloseKey = ConsoleSpecialKey.ControlC);

        /// <summary>
        /// Wait until the app exits.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout to wait until the app is exited [default=infinite]</param>
        /// <returns>True if exited or False if timeout elapsed</returns>
        bool WaitForExit(int millisecondsTimeout = Timeout.Infinite);

        /// <summary>
        /// Fires when the console outputs a new line or error.
        /// </summary>
        /// <remarks>The lines are queued and guaranteed to follow the output order</remarks>
        event EventHandler<ConsoleOutputEventArgs> ConsoleOutput;

        /// <summary>
        /// Fires when the app exits.
        /// </summary>
        event EventHandler<EventArgs> Exited;
    }
}
