﻿using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    class AutoTextService
    {
        public void Run()
        {
            Log.Information("WatcherHangfire Auto Text Service.");
        }
    }
}
