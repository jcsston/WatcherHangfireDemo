using Hangfire;
using Microsoft.Owin.Hosting;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    public class WatcherService
    {
        private bool dashboardStarted = false;
        private BackgroundJobServer _server = null;
        private object jobLock = new object();
        private Dictionary<String, Expression<Action>> jobs = new Dictionary<string, Expression<Action>>();
        

        public WatcherService()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("WatcherHangfireDemo.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            GlobalConfiguration.Configuration
                .UseSqlServerStorage("APS.DataAccess.Properties.Settings.gazelleConnectionString")
                .UseSerilogLogProvider();

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            
            jobs.Add("AutoEmailService", () => new AutoEmailService().Run());
            jobs.Add("AutoTextService", () => new AutoTextService().Run());
            jobs.Add("LargeExport", () => new LargeExport().Run());
            jobs.Add("LargeExport2", () => new LargeExport2().Run());
            jobs.Add("LargeExport3", () => new LargeExport3().Run());
            jobs.Add("LargeExport4", () => new LargeExport4().Run());
            GlobalJobFilters.Filters.Add(new DisableMultipleQueuedItemsFilterAttribute(jobs.Keys.ToList()));

            RecurringJob.AddOrUpdate("UpdateScheduleFromDatabase", () => UpdateScheduleFromDatabase(), Cron.Minutely);
        }

        public void UpdateScheduleFromDatabase()
        {
            lock (jobLock) // we grab a lock just to make sure we aren't trying to update the job schedules from two different threads
            {
                Log.Information("Refreshing schedule from database");
                string connectionString = ConfigurationManager.ConnectionStrings["APS.DataAccess.Properties.Settings.gazelleConnectionString"].ConnectionString;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT job_name, cron_expression, inactive FROM hangfire_schedule", connection))
                    {
                        connection.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Check is the reader has any rows at all before starting to read.
                            if (reader.HasRows)
                            {
                                // Read advances to the next row.
                                while (reader.Read())
                                {
                                    // To avoid unexpected bugs access columns by name.
                                    var jobName = reader.GetString(reader.GetOrdinal("job_name"));
                                    var cronExpression = reader.GetString(reader.GetOrdinal("cron_expression"));
                                    var inactive = reader.GetString(reader.GetOrdinal("inactive"));

                                    if (inactive == "Y")
                                    {
                                        Log.Warning("Disabling job {jobName}", jobName);
                                        RecurringJob.RemoveIfExists(jobName);
                                    }
                                    else
                                    {
                                        if (jobs.ContainsKey(jobName))
                                        {
                                            Log.Debug("Update job {jobName} with cron {cronExpression}", jobName, cronExpression);
                                            RecurringJob.AddOrUpdate(jobName, jobs[jobName], cronExpression, TimeZoneInfo.Local);
                                        }
                                        else
                                        {
                                            Log.Error("Invalid job name in database: " + jobName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void StartService()
        {
            if (_server != null)
                Log.Error("Service already started");
            else
            {
                // go ahead and setup initial schedules
                UpdateScheduleFromDatabase();
                var options = new BackgroundJobServerOptions
                {
                    WorkerCount = 2
                };
                _server = new BackgroundJobServer(options);
            }
        }

        public void StopService()
        {
            if (_server != null)
            {
                _server.Dispose();
                _server = null;
            }
        }

        public void StartDashboard()
        {
            if (!dashboardStarted)
            {
                string baseAddress = "http://localhost:9000/";
                // Start OWIN host 
                WebApp.Start<Startup>(url: baseAddress);
                dashboardStarted = true;
            }
        }
    }
}
