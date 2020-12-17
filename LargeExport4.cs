using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    class LargeExport4
    {
        public void Run()
        {
            Log.Information("Watcher Starting large export4.");
            string connectionString = ConfigurationManager.ConnectionStrings["APS.DataAccess.Properties.Settings.gazelleConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                for (int i = 0; i < 100; i++)
                {
                    Log.Information($"Watcher large export4 start loop {i}");
                    using (SqlCommand cmd = new SqlCommand("select top 10 employee_id, count(*) from time_clock_shift group by employee_id order by count(*) desc", connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            // Check is the reader has any rows at all before starting to read.
                            if (reader.HasRows)
                            {
                                // Read advances to the next row.
                                while (reader.Read())
                                {
                                    reader.GetInt32(0);
                                }
                            }
                        }
                    }
                    Log.Information($"Watcher large export4 end loop {i}");
                }
            }
            Log.Information("Watcher Finished large export4.");
        }
    }
}
