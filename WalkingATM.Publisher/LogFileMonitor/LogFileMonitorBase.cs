using System.Timers;
using Microsoft.Extensions.Logging;
using WalkingATM.Publisher.Extensions;
using Timer = System.Timers.Timer;

#pragma warning disable CS8618

namespace WalkingATM.Publisher.LogFileMonitor;

public interface ILogFileMonitor
{
    /// <summary>
    /// \n or \r\n(windows), default \n(linux)
    /// </summary>
    string Delimiter { get; set; }

    void Start(string path);
    void Stop();
    void OnLineCallback(EventHandler<LogFileMonitorLineEventArgs> onLine);
}

public abstract class LogFileMonitorBase : ILogFileMonitor
{
    private readonly object _syncRoot = new();

    private readonly ILogger<LogFileMonitorBase> _logger;

    // buffer for storing data at the end of the file that does not yet have a delimiter
    private string _buffer = string.Empty;

    // get the current size
    private long _currentSize;

    // are we currently checking the log (stops the timer going in more than once)
    private bool _isCheckingLog;

    // file path + delimiter internals
    private string _path;

    // timer object
    private Timer? _t;

    protected LogFileMonitorBase(ILogger<LogFileMonitorBase> logger)
    {
        _logger = logger;
    }

    public string Delimiter { get; set; } = "\n";

    public void Start(string path)
    {
        lock (_syncRoot)
        {
            _path = path;

            // get the current size
            var fileInfo = new FileInfo(_path);
            if (!fileInfo.Exists)
            {
                using (fileInfo.Create())
                {
                }
            }

            _currentSize = fileInfo.Length;

            // start the timer
            _t = new Timer();
            _t.Elapsed += CheckLog!;
            _t.AutoReset = true;
            _t.Start();
        }
    }

    /// <summary>
    /// stop timer and clean up
    /// </summary>
    public void Stop()
    {
        lock (_syncRoot)
        {
            if (_t == null)
                return;

            _t.Stop();
            _t.Elapsed -= CheckLog!;
            _t.Dispose();
            _t = null;

            if (OnLine.IsRegistered())
            {
                OnLine = null;
            }

            _isCheckingLog = false;
            _buffer = string.Empty;
        }
    }

    private event EventHandler<LogFileMonitorLineEventArgs>? OnLine;

    public void OnLineCallback(EventHandler<LogFileMonitorLineEventArgs> onLine)
    {
        lock (_syncRoot)
        {
            if (!OnLine.IsRegistered(onLine))
            {
                OnLine += onLine;
            }
        }
    }

    private bool StartCheckingLog()
    {
        lock (_syncRoot)
        {
            if (_isCheckingLog)
                return true;

            _isCheckingLog = true;
            return false;
        }
    }

    private void DoneCheckingLog()
    {
        lock (_syncRoot)
            _isCheckingLog = false;
    }

    private void CheckLog(object s, ElapsedEventArgs e)
    {
        if (StartCheckingLog())
        {
            try
            {
                // get the new size
                var newSize = new FileInfo(_path).Length;

                // if they are the same then continue.. if the current size is bigger than the new size continue
                if (_currentSize >= newSize)
                    return;

                // read the contents of the file
                using (var stream = File.Open(_path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var sr = new StreamReader(stream))
                {
                    // seek to the current file position
                    sr.BaseStream.Seek(_currentSize, SeekOrigin.Begin);

                    // read from current position to the end of the file
                    var newData = _buffer + sr.ReadToEnd();

                    // if we don't end with a delimiter we need to store some data in the buffer for next time
                    if (!newData.EndsWith(Delimiter))
                    {
                        // we don't have any lines to process so save in the buffer for next time
                        if (newData.IndexOf(Delimiter, StringComparison.Ordinal) == -1)
                        {
                            _buffer += newData;
                            newData = string.Empty;
                        }
                        else
                        {
                            // we have at least one line so store the last section (without lines) in the buffer
                            var pos = newData.LastIndexOf(Delimiter, StringComparison.Ordinal) + Delimiter.Length;
                            _buffer = newData.Substring(pos);
                            newData = newData.Substring(0, pos);
                        }
                    }

                    // split the data into lines
                    var lines = newData.Split(new[] { Delimiter }, StringSplitOptions.RemoveEmptyEntries);

                    // send back to caller, NOTE: this is done from a different thread!
                    OnLine?.Invoke(this, new LogFileMonitorLineEventArgs { Lines = lines });
                }

                // set the new current position
                _currentSize = newSize;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }

            // we done..
            DoneCheckingLog();
        }
    }
}