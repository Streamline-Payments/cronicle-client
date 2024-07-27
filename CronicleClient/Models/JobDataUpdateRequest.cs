using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A structured object containing the request to update job data.
/// </summary>
public class JobDataUpdateRequest
{
  /// <summary>
  ///   A unique ID assigned to the job when it was first created.
  /// </summary>
  [JsonPropertyName("id")]
  public string Id { get; set; } = null!;

  /// <summary>
  ///   The maximum allowed run time for jobs, specified in seconds.
  /// </summary>
  [JsonPropertyName("timeout")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? Timeout { get; set; }

    /// <summary>
    ///   The number of retries to allow before reporting an error.
    /// </summary>
    [JsonPropertyName("retries")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? Retries { get; set; }

    /// <summary>
    ///   Optional delay between retries, in seconds.
    /// </summary>
    [JsonPropertyName("retry_delay")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? RetryDelay { get; set; }

    /// <summary>
    ///   The chain reaction event ID to launch when jobs complete successfully.
    /// </summary>
    [JsonPropertyName("chain")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? Chain { get; set; }

    /// <summary>
    ///   List of e-mail recipients to notify upon job failure (CSV).
    /// </summary>
    [JsonPropertyName("notify_fail")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? NotifyFail { get; set; }

    /// <summary>
    ///   List of e-mail recipients to notify upon job success (CSV).
    /// </summary>
    [JsonPropertyName("notify_success")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? NotifySuccess { get; set; }

    /// <summary>
    ///   An optional URL to hit for the start and end of each job.
    /// </summary>
    [JsonPropertyName("web_hook")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? WebHook { get; set; }

    /// <summary>
    ///   Limit the CPU to the specified percentage (100 = 1 core), abort if exceeded.
    /// </summary>
    [JsonPropertyName("cpu_limit")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? CpuLimit { get; set; }

    /// <summary>
    ///   Only abort if the CPU limit is exceeded for this many seconds.
    /// </summary>
    [JsonPropertyName("cpu_sustain")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? CpuSustain { get; set; }

    /// <summary>
    ///   Limit the memory usage to the specified amount, in bytes.
    /// </summary>
    [JsonPropertyName("memory_limit")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? MemoryLimit { get; set; }

    /// <summary>
    ///   Only abort if the memory limit is exceeded for this many seconds.
    /// </summary>
    [JsonPropertyName("memory_sustain")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? MemorySustain { get; set; }

    /// <summary>
    ///   Limit the job log file size to the specified amount, in bytes.
    /// </summary>
    [JsonPropertyName("log_max_size")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? LogMaxSize { get; set; }
}