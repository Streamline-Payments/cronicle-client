using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CronicleClient.Models
{
    internal record JobResponse :BaseEventResponse
    {
        /// <summary>
        /// A job.
        /// </summary>
        [JsonPropertyName("job")]
        public JobData? Job { get; set; }
    }
}
