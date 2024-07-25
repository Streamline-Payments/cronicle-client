using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace CronicleClient.Models
{
    internal record JobsResponse: BaseEventResponse
    {
        /// <summary>
        /// Collection of jobs
        /// </summary>
        [JsonPropertyName("jobs")]
        public Dictionary<string, JobData> Jobs { get; set; }
    }
}
