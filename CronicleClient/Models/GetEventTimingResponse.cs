using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace CronicleClient.Models
{
    internal record GetEventTimingResponse
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

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        public GetEventTimingResponse FromJson(string eventJson)
        {
            return JsonSerializer.Deserialize<GetEventTimingResponse>(eventJson) ?? throw new ArgumentNullException(nameof(eventJson));
        }
    }
}
