using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    class LargeExport
    {
        public void Run()
        {
            Log.Information("Watcher Starting large export.");
            Thread.Sleep(1000 * 60);
            Log.Information("Watcher Finished large export.");
        }
    }
}
