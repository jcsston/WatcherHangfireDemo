using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    class AutoEmailService
    {
        public void Run()
        {
            Log.Information("WatcherHangfire Auto Email Service.");
        }
    }
}
