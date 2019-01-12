using Ryujinx.Common.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Ryujinx
{
    static class FileLog
    {
        private static string _path;

        private static Thread _messageThread;

        private static BlockingCollection<LogEventArgs> _messageQueue;

        private static StreamWriter _logWriter;

        static FileLog()
        {
            if (!Logger.EnableFileLog)
                return;

            _path = Path.Combine(Environment.CurrentDirectory, "Ryujinx.log");

            if (File.Exists(_path))
            {
                File.Delete(_path);
            }

            _messageQueue = new BlockingCollection<LogEventArgs>(10);

            _messageThread = new Thread(() =>
            {
                while (!_messageQueue.IsCompleted)
                {
                    try
                    {
                        PrintLog(_messageQueue.Take());
                    }
                    catch (InvalidOperationException)
                    {
                        // IOE means that Take() was called on a completed collection.
                        // Some other thread can call CompleteAdding after we pass the
                        // IsCompleted check but before we call Take.
                        // We can simply catch the exception since the loop will break
                        // on the next iteration.
                    }
                }
            });

            _logWriter = new StreamWriter(File.OpenWrite(_path));

            _messageThread.IsBackground = true;
            _messageThread.Start();
        }

        public static void Log(object sender, LogEventArgs e)
        {
            if (!_messageQueue.IsAddingCompleted)
            {
                _messageQueue.Add(e);
            }
        }

        private static void PrintLog(LogEventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat(@"{0:hh\:mm\:ss\.fff}", e.Time);
            sb.Append(" | ");
            sb.AppendFormat("{0:d4}", e.ThreadId);
            sb.Append(' ');
            sb.Append(e.Message);

            if (e.Data != null)
            {
                PropertyInfo[] props = e.Data.GetType().GetProperties();

                sb.Append(' ');

                foreach (var prop in props)
                {
                    sb.Append(prop.Name);
                    sb.Append(": ");
                    sb.Append(prop.GetValue(e.Data));
                    sb.Append(" - ");
                }

                // We remove the final '-' from the string
                if (props.Length > 0)
                {
                    sb.Remove(sb.Length - 3, 3);
                }
            }

            _logWriter.WriteLine(sb.ToString());
        }

        public static void Close()
        {
            if (!Logger.EnableFileLog)
                return;

            _messageQueue.CompleteAdding();

            _messageThread.Join();

            _logWriter.Flush();
            _logWriter.Close();
            _logWriter.Dispose();
        }
    }
}
