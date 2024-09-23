using CronicleClient.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace CronicleClient.Interfaces
{
    public interface ICronicleEvent
    {
        Task<IEnumerable<EventData>> GetSchedule(int limit = 50, int offset = 0, CancellationToken cancellationToken = default);
        Task<EventData?> GetById(string eventId, CancellationToken cancellationToken = default);
        Task<EventData?> GetByTitle(string eventTitle, CancellationToken cancellationToken = default);
        Task<string> Create(NewEvent eventData, CancellationToken cancellationToken = default);
        Task Update(EventData eventData, bool resetCursor = false, bool abortJobs = false, CancellationToken cancellationToken = default);
        Task Delete(string eventId, CancellationToken cancellationToken = default);
        Task<string[]?> RunEventById(string eventId, CancellationToken cancellationToken = default);
        Task<string[]?> RunEventById(string eventId, EventData eventData, CancellationToken cancellationToken = default);
        Task<string[]?> RunEventByTitle(string eventTitle, CancellationToken cancellationToken = default);
    }
}
