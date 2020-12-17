using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    class LargeExport3
    {
        public void Run()
        {
            Log.Information("Watcher Starting large export3.");
            Thread.Sleep(1000 * 90);
            Log.Information("Watcher Finished large export3.");
        }
    }
}
