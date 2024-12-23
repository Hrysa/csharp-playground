﻿using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace shared;

public static class Helper
{
    public static long MeasureGC(Action action, bool print = true)
    {
        var bytes = GC.GetAllocatedBytesForCurrentThread();

        action();

        var count = GC.GetAllocatedBytesForCurrentThread() - bytes;

        if (print)
        {
            Console.WriteLine($"bytes {count}");
        }

        return count;
    }

    public static double MeasureTime(Action action, bool print = true)
    {
        DateTimeOffset start = DateTimeOffset.UtcNow;
        action();
        var ts = (DateTimeOffset.UtcNow - start).TotalMilliseconds;

        if (print)
        {
            Console.WriteLine($"time spent: {ts}");
        }

        return ts;
    }

    [DllImport("winmm", EntryPoint = "timeBeginPeriod")]
    public static extern void TimeBeginPeriod(int t);

    [Conditional("NET9_0")]
    public static void Log(string s)
    {
        Console.WriteLine($"{DateTime.Now:O} {Thread.CurrentThread.ManagedThreadId:000000} I {s}");
    }

    // [Conditional("NET9_0")]
    public static void Warn(string s)
    {
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine($"{DateTime.Now:O} {Thread.CurrentThread.ManagedThreadId:000000} W {s}");
        Console.ResetColor();
    }

    // [Conditional("NET9_0")]
    public static void Error(string s)
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.WriteLine($"{DateTime.Now:O} {Thread.CurrentThread.ManagedThreadId:000000} E {s}");
        Console.ResetColor();
    }

    public static SocketAddress Clone(this SocketAddress self)
    {
        var copy = new SocketAddress(self.Family, self.Size);

#if NET6_0_OR_GREATER
        self.Buffer.CopyTo(copy.Buffer);
#else
            for (int i = 0; i < self.Size; i++)
            {
                copy[i] = self[i];
            }
#endif
        return copy;
    }

    public static void Loop(Action action, string tag = "")
    {
        Task.Run(() =>
        {
            while (true)
            {
                action();

                // DateTimeOffset start = DateTimeOffset.UtcNow;
                // action();
                //
                // var ts = DateTimeOffset.UtcNow - start;
                // if (ts > TimeSpan.FromMilliseconds(1))
                // {
                //     Console.WriteLine($"{tag} {ts.TotalMilliseconds}ms");
                // }

                Thread.Sleep(1);
            }
        });
    }
}