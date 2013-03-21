using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SlavaGu.ConsoleAppLauncher
{
    public class ConsoleApp : IConsoleApp
    {
        private Process _process;
        private AutoResetEvent _processEvent;
        private ConcurrentQueue<ConsoleOutputEventArgs> _consoleOutputQueue;
        
        private Task _processMonitor;
        private CancellationTokenSource _cancellationTokenSource;
        
        private readonly object _stateLock = new object();
        private readonly Win32.ConsoleCtrlEventHandler _consoleCtrlEventHandler;

        public ConsoleApp(string fileName, string cmdLine)
        {
            FileName = fileName;
            CmdLine = cmdLine;
            
            _consoleCtrlEventHandler += ConsoleCtrlHandler;
        }

        /// <summary>
        /// File name of the app, e.g. "cmd.exe"
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Command line of the app, e.g. "/c dir"
        /// </summary>
        public string CmdLine { get; private set; }

        /// <summary>
        /// Current state of the app.
        /// </summary>
        public AppState State { get; private set; }

        /// <summary>
        /// Exit code of the app.
        /// </summary>
        public int? ExitCode { get; private set; }

        /// <summary>
        /// Time the app has exited.
        /// </summary>
        public DateTime? ExitTime { get; private set; }

        /// <summary>
        /// Start the app.
        /// </summary>
        public void Run()
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            lock (_stateLock)
            {
                if (State == AppState.Running)
                    throw new InvalidOperationException("App is already running.");

                if (State == AppState.Exiting)
                    throw new InvalidOperationException("App is exiting.");

                StartProcessAsync();

                State = AppState.Running;
            }
        }

        /// <summary>
        /// Stop the app.
        /// </summary>
        /// <param name="closeKey">Special key to send to close the app [default=Ctrl-C]</param>
        /// <param name="forceCloseMillisecondsTimeout">Timeout to wait before closing the app forcefully [default=infinite]</param>
        public void Stop(ConsoleSpecialKey closeKey = ConsoleSpecialKey.ControlC, int forceCloseMillisecondsTimeout = Timeout.Infinite)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            lock (_stateLock)
            {
                if (State == AppState.Undefined || State == AppState.Exited)
                    throw new InvalidOperationException("App is not running.");

                if (State == AppState.Exiting)
                    throw new InvalidOperationException("App is already exiting.");

                State = AppState.Exiting;

                Task.Factory.StartNew(() => CloseConsole(closeKey, forceCloseMillisecondsTimeout), 
                    TaskCreationOptions.LongRunning);
            }
        }

        /// <summary>
        /// Wait until the app exits.
        /// </summary>
        /// <param name="millisecondsTimeout">Timeout to wait until the app is exited [default=infinite]</param>
        /// <returns>True if exited or False if timeout elapsed</returns>
        public bool WaitForExit(int millisecondsTimeout = Timeout.Infinite)
        {
            if (_disposed)
                throw new ObjectDisposedException("Object was disposed.");

            if (State == AppState.Undefined || _processMonitor == null)
            {
                Trace.TraceWarning("App hasn't started yet");
                return true;
            }

            Trace.TraceInformation("Waiting until the app exits: Timeout={0}", millisecondsTimeout);
            return _processMonitor.Wait(millisecondsTimeout);
        }

        /// <summary>
        /// Fires when the app exits.
        /// </summary>
        public event EventHandler<EventArgs> Exited;

        /// <summary>
        /// Fires when the console outputs a new line or error.
        /// </summary>
        /// <remarks>The lines are queued and guaranteed to follow the output order</remarks>
        public event EventHandler<ConsoleOutputEventArgs> ConsoleOutput;

        private void StartProcessAsync()
        {
            _process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo =
                {
                    FileName = FileName,
                    Arguments = CmdLine,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                },
            };

            try
            {
                Trace.TraceInformation("Starting app: FileName='{0}', CmdLine={1}", FileName, CmdLine);

                _processEvent = new AutoResetEvent(false);
                _consoleOutputQueue = new ConcurrentQueue<ConsoleOutputEventArgs>();

                _cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = _cancellationTokenSource.Token;
                _processMonitor = new Task(MonitoringHandler, cancellationToken, TaskCreationOptions.LongRunning);
                _processMonitor.Start();

                _process.OutputDataReceived += OnOutputLineReceived;
                _process.ErrorDataReceived += OnErrorLineReceived;
                _process.Exited += OnProcessExited;

                _process.Start();

                _process.BeginErrorReadLine();
                _process.BeginOutputReadLine();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Could not start app: FileName='{0}', Error={1}", FileName, ex);

                FreeProcessResources();
                if (_cancellationTokenSource != null)
                {
                    _cancellationTokenSource.Cancel();
                    _processMonitor = null;
                }
                throw;
            }
        }

        private void MonitoringHandler(object obj)
        {
            var cancellationToken = (CancellationToken)obj;
            var supportedEvents = new [] { _processEvent, cancellationToken.WaitHandle };

            while (!cancellationToken.IsCancellationRequested)
            {
                WaitHandle.WaitAny(supportedEvents);

                // always dispatch output is case more than one event becomes signaled
                DispatchProcessOutput();
            }

            HandleProcessExit();
        }

        private void DispatchProcessOutput()
        {
            ConsoleOutputEventArgs args;
            while (_consoleOutputQueue.TryDequeue(out args))
            {
                try
                {
                    OnConsoleOutput(args);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("OnConsoleOutput exception ignored: FileName='{0}', Error={1}", FileName, ex);
                }
            }
        }

        private void HandleProcessExit()
        {
            Trace.TraceInformation("Handling app exit");

            if (_process == null)
                return;

            lock (_stateLock)
            {
                ExitCode = _process.ExitCode;
                ExitTime = _process.ExitTime;

                FreeProcessResources();
                State = AppState.Exited;
            }
            try
            {
                OnExited(new EventArgs());
            }
            catch (Exception ex)
            {
                Trace.TraceError("OnExited exception ignored: FileName='{0}', Error={1}", FileName, ex);
            }
        }

        private void CloseConsole(ConsoleSpecialKey closeKey, int forceCloseMillisecondsTimeout)
        {
            if (_process == null || _process.HasExited)
                return;

            Trace.TraceInformation("Closing app input by sending Ctrl-Z signal");
            _process.StandardInput.Close();

            if (_process == null || _process.HasExited)
                return;

            Trace.TraceInformation("Trying to close the app gracefully by sending " + closeKey);
            Win32.AttachConsole((uint)_process.Id);
            Win32.SetConsoleCtrlHandler(_consoleCtrlEventHandler, true);
            var ctrlType = closeKey == ConsoleSpecialKey.ControlC ? Win32.CtrlType.CtrlCEvent : Win32.CtrlType.CtrlBreakEvent;
            Win32.GenerateConsoleCtrlEvent(ctrlType, 0);

            if (_process == null || _process.HasExited)
                return;

            // close console forcefully if not finished within allowed timeout
            Trace.TraceInformation("Waiting for app exit: Timeout={0}", forceCloseMillisecondsTimeout);
            var exited = _process.WaitForExit(forceCloseMillisecondsTimeout);
            if (!exited)
            {
                Trace.TraceWarning("Closing app forcefully");
                _process.Kill();
            }
        }

        private static bool ConsoleCtrlHandler(Win32.CtrlType ctrlType)
        {
            const bool ignore = true;
            return ignore;
        }

        private void FreeProcessResources()
        {
            if (_process != null)
            {
                _process.OutputDataReceived -= OnOutputLineReceived;
                _process.ErrorDataReceived -= OnErrorLineReceived;
                _process.Exited -= OnProcessExited;

                _process.Close();
                _process.Dispose();
                _process = null;
            }
        }

        private void OnOutputLineReceived(object sender, DataReceivedEventArgs e)
        {
            _consoleOutputQueue.Enqueue(new ConsoleOutputEventArgs(e.Data, false));
            _processEvent.Set();
        }

        private void OnErrorLineReceived(object sender, DataReceivedEventArgs e)
        {
            _consoleOutputQueue.Enqueue(new ConsoleOutputEventArgs(e.Data, true));
            _processEvent.Set();
        }

        private void OnProcessExited(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
            _processEvent.Set();
        }

        protected virtual void OnConsoleOutput(ConsoleOutputEventArgs e)
        {
            if (e.Line == null)
                return;

            Trace.TraceInformation("Console output: Line='{0}', IsError={1}", e.Line, e.IsError);

            var handler = ConsoleOutput;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnExited(EventArgs e)
        {
            Trace.TraceInformation("App exited: FileName='{0}', ExitCode={1}, ExitTime={2}", FileName, ExitCode, ExitTime);
            
            var handler = Exited;
            if (handler != null)
                handler(this, e);
        }


        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    CloseConsole(ConsoleSpecialKey.ControlBreak, 500);
                    WaitForExit(500);
                    FreeProcessResources();
                }

                _disposed = true;
            }
        }

        ~ConsoleApp()
        {
            Dispose(false);
        }

        #endregion
    }
}