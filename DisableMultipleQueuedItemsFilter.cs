using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using Hangfire.States;
using Hangfire.Storage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatcherHangfireDemo
{
    public class DisableMultipleQueuedItemsFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private readonly List<string> _jobNames;

        public DisableMultipleQueuedItemsFilterAttribute(string jobName)
        {
            _jobNames = new List<String>{
                jobName
            };
        }

        public DisableMultipleQueuedItemsFilterAttribute(List<string> jobNames)
        {
            _jobNames = jobNames;
        }

        public void OnStateElection(ElectStateContext context)
        {
            var backgroundJobTypeName = context.BackgroundJob.Job.Type.Name;
            if (_jobNames.Contains(backgroundJobTypeName))
            {
                var queuedJobs = context.Storage.GetMonitoringApi().EnqueuedJobs("default", 0, 2000);
                var processingJobs = context.Storage.GetMonitoringApi().ProcessingJobs(0, 2000);
                
                foreach (var processingJob in processingJobs)
                {
                    var job = processingJob.Value.Job;
                    if (job != null)
                    {
                        var typeName = job.Type.Name;
                        if (typeName == backgroundJobTypeName && !context.CandidateState.IsFinal)
                        {
                            Log.Information("Removed Queued Job {backgroundJobTypeName}", backgroundJobTypeName);
                            context.CandidateState = new DeletedState
                            {
                                Reason = $"Job already running since {processingJob.Value.StartedAt.Value}. It is not allowed to perform multiple same tasks."
                            };
                            return;
                        }
                    }
                }
                foreach (var queuedJob in queuedJobs)
                {
                    var job = queuedJob.Value.Job;
                    if (job != null)
                    {
                        var typeName = job.Type.Name;
                        if (typeName == backgroundJobTypeName && !context.CandidateState.IsFinal)
                        {
                            Log.Information("Removed Queued Job {backgroundJobTypeName}", backgroundJobTypeName);
                            context.CandidateState = new DeletedState
                            {
                                Reason = $"Job already queued since {queuedJob.Value.EnqueuedAt.Value}. It is not allowed to perform multiple same tasks."
                            };
                            return;
                        }
                    }
                }
            }
        }
    }
}
