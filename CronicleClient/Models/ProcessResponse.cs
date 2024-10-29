
using System.Text.Json.Serialization;

namespace CronicleClient.Models;


internal record ProcessResponse 
{
    [property: JsonPropertyName("description")]
    public string? Description { get; set; }

    //TODO; Add a proper CODE mapping rather than a hardcoded value
    [property: JsonPropertyName("code")]
    string Code = "1";

    [property: JsonPropertyName("update_event")]
    UpdateEvent? UpdateEvent = null;

}

internal record UpdateEvent 
{
    [property: JsonPropertyName("timing")]
    Timing Timing;
} 