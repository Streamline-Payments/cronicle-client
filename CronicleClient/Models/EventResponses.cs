using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CronicleClient.Models
{
    internal record EventResponse
    {
        [JsonPropertyName("code")]
        [JsonConverter(typeof(Common.IntToStringJsonConverter))]
        public string Code { get; set; } = null!;

        [Required]
        [JsonPropertyName("eventData")]
        [JsonConverter(typeof(EventData))]
        public string Event { get; set; }

    }
        /*
         * these ones might require special conditions
        internal record AddResponse(object Code, string Id = default!);

        internal record DeleteResponse(object Code, string Description);
        internal record UpdateEventResponse(bool IsSucess, string Id = default!);
        internal record UpdateResponse(int Code, string Id = default!);
        internal record DisableResponse(object Code, string Description);
        internal record ProcessResponse(string Description, string EventId = default!, string Code = Utility.CodeSuccess);
    */
}
