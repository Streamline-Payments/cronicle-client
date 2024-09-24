using CronicleClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CronicleClient.Interfaces
{
    public interface ICronicleJob
    {
        /// <summary>
        /// Fetches the event history for a specific event.
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="numToFetch"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<JobData[]> GetByEventId(string eventId, int numToFetch, int offset = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches previously completed jobs for all events.
        /// </summary>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<JobData[]> GetHistory(int limit, int offset = 0, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the status for a job currently in progress or already completed.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<JobData?> GetJobStatus(string jobId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches the status for all active jobs.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<Dictionary<string, JobData>?> GetActiveJobs(CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates a job that is already in progress.
        /// </summary>
        /// <param name="jobData"></param>
        /// <param name="cancellationToken"></param>
        Task Update(JobDataUpdateRequest jobData, CancellationToken cancellationToken = default);

        /// <summary>
        /// Aborts a running job given its ID.
        /// </summary>
        /// <param name="jobId"></param>
        /// <param name="cancellationToken"></param>
        Task AbortJob(string jobId, CancellationToken cancellationToken = default);
    }
}
