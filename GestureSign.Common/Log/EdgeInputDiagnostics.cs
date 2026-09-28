using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace GestureSign.Common.Log
{
    // Opt-in portable diagnostic only. Never write files from the mouse hook.
    public static class EdgeInputDiagnostics
    {
        public static readonly bool Enabled = File.Exists(Path.Combine(AppContext.BaseDirectory, "edge-input-diagnostics.enabled"));
        private static readonly ConcurrentQueue<string> Pending = new ConcurrentQueue<string>();
        private static readonly object Gate = new object();
        private static readonly string DirectoryPath = Path.Combine(AppContext.BaseDirectory, "Diagnostics");
        private static readonly string Session = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Environment.ProcessId;
        private static readonly Timer Writer = Enabled ? new Timer(_ => Flush(), null, 500, 500) : null;
        private static long _sequence;
        private static int _part;
        [DllImport("user32.dll")] private static extern short GetAsyncKeyState(int key);
        [DllImport("user32.dll")] private static extern IntPtr GetForegroundWindow();

        static EdgeInputDiagnostics()
        {
            if (Enabled)
            {
                AppDomain.CurrentDomain.ProcessExit += (_, __) => Flush();
                Record("SESSION", "EdgeInputDiagnostic-v1; non-injected does not identify the physical device; HookHandled is GestureSign's decision, not proof of game delivery.");
            }
        }

        public static void Record(string stage, string detail)
        {
            if (!Enabled || Pending.Count >= 8192) return;
            Pending.Enqueue($"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} Seq={Interlocked.Increment(ref _sequence)} Tick={Environment.TickCount64} Stage={stage} Shift={(GetAsyncKeyState(0x10) & 0x8000) != 0} Foreground=0x{GetForegroundWindow().ToInt64():X} {detail}");
        }

        private static void Flush()
        {
            if (!Enabled || !Monitor.TryEnter(Gate)) return;
            try
            {
                Directory.CreateDirectory(DirectoryPath);
                var path = Path.Combine(DirectoryPath, $"edge-input-{Session}-{_part}.log");
                if (File.Exists(path) && new FileInfo(path).Length > 4 * 1024 * 1024)
                {
                    _part = (_part + 1) % 3;
                    path = Path.Combine(DirectoryPath, $"edge-input-{Session}-{_part}.log");
                    File.WriteAllText(path, "");
                }
                using var writer = new StreamWriter(path, true);
                while (Pending.TryDequeue(out var line)) writer.WriteLine(line);
            }
            catch { /* Diagnostic I/O must never interfere with input. */ }
            finally { Monitor.Exit(Gate); }
        }
    }
}
