using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CronicleClient.Models;

namespace CronicleClient;

internal static class Common
{
  public static void EnsureSuccessStatusCode(this BaseEventResponse? response)
  {
    if (response is null) throw new Exception("No response was provided from Cronicle server");

    if ((string.Equals(response.Code, "event") && response.Description?.Contains("Failed to locate event") == true) ||
        (string.Equals(response.Code, "job") && response.Description?.Contains("Failed to locate job") == true)) throw new KeyNotFoundException();

    if (!string.Equals(response.Code, "0"))
      throw new Exception(
        string.Format(
          CultureInfo.InvariantCulture,
          "Cronicle API error: ({0}){1} ",
          response.Code,
          response.Description));
  }

  public class IntToBoolJsonConverter : JsonConverter<bool>
  {
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      return reader.GetInt32() == 1;
    }

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
      writer.WriteNumberValue(value ? 1 : 0);
    }
  }

  public class HandleBoolForObjJsonConverter : JsonConverter<Timing?>
  {
    public override Timing? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      // Cronicle has been known to set obj to "false"
      // When it happens, translate that to a null object
      return null;
    }

    public override void Write(Utf8JsonWriter writer, Timing? value, JsonSerializerOptions options)
    {
      writer.WriteNullValue();
    }
  }

  public class IntToStringJsonConverter : JsonConverter<string>
  {
    public override string? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
      return reader.TokenType switch
      {
        JsonTokenType.Number => reader.GetInt32().ToString(),
        JsonTokenType.String => reader.GetString(),
        _ => throw new JsonException()
      };
    }

    public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
    {
      writer.WriteNumberValue(int.Parse(value));
    }
  }
}