using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    class LargeExport2
    {
        public void Run()
        {
            Log.Information("Watcher Starting large export2.");
            Thread.Sleep(1000 * 70);
            Log.Information("Watcher Finished large export2.");
        }
    }
}
