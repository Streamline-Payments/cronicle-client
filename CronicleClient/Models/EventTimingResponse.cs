using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CronicleClient.Models
{
    internal record EventTimingResponse
    {
        /// <summary>
        ///   This will be set to 0 upon success, or any other value if an error occurred. In the event of an error, a description property will also be included,
        ///   containing the error message itself.
        /// </summary>
        [JsonPropertyName("code")]
        [JsonConverter(typeof(Common.IntToStringJsonConverter))]
        public string Code { get; set; } = null!;

        /// <summary>
        ///   In the event of an error, this will contain a description of the error.
        /// </summary>
        [JsonPropertyName("timing")]
        public Timing? Timing { get; set; }
    }
}
