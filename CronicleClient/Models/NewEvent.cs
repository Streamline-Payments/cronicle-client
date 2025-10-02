using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

/// <summary>
/// A new event to create in Cronicle.
/// </summary>
public record NewEvent
{
  /// <summary>
  ///   The chain reaction event ID to launch when jobs complete successfully.
  /// </summary>
  [JsonPropertyName("chain")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? Chain { get; set; }

  /// <summary>
  ///   The chain reaction event ID to launch when jobs fail.
  /// </summary>
  [JsonPropertyName("chain_error")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? ChainError { get; set; }

  /// <summary>
  ///   The chain data to send when the event is launched.
  /// </summary>
  [JsonPropertyName("chain_data")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public Dictionary<string, object>? ChainData { get; set; }

  /// <summary>
  ///   The chain params to send when the event is launched.
  /// </summary>
  [JsonPropertyName("chain_params")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public Dictionary<string, object>? ChainParams { get; set; }

  /// <summary>
  ///   Allow jobs to be queued up when they can't run immediately.
  /// </summary>
  [JsonPropertyName("queue")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool? Queue { get; set; }

  /// <summary>
  ///   Maximum queue length, when queue is enabled.
  /// </summary>
  [JsonPropertyName("queue_max")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? QueueMax { get; set; }

  /// <summary>
  ///   Specifies the algorithm to use for picking a server from the target group. See Algorithm.
  /// </summary>
  [JsonPropertyName("algo")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? Algo { get; set; }

  /// <summary>
  ///   Specifies whether the event has Run All Mode enabled or not.
  /// </summary>
  [JsonPropertyName("catch_up")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool? CatchUp { get; set; }

  /// <summary>
  ///   The Category ID to which the event is assigned.
  /// </summary>
  [JsonPropertyName("category")]
  public string Category { get; set; } = null!;

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
  ///   Specifies whether Detached Mode is enabled or not.
  /// </summary>
  [JsonPropertyName("detached")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool? Detached { get; set; }

  /// <summary>
  ///   Specifies whether the event is enabled (active in the scheduler) or not.
  /// </summary>
  [JsonPropertyName("enabled")]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool Enabled { get; set; }

  /// <summary>
  ///   Limit the job log file size to the specified amount, in bytes.
  /// </summary>
  [JsonPropertyName("log_max_size")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? LogMaxSize { get; set; }

  /// <summary>
  ///   The total amount of concurrent jobs allowed to run.
  /// </summary>
  [JsonPropertyName("max_children")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? MaxChildren { get; set; }

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
  ///   Specifies whether the event has Multiplexing mode is enabled or not.
  /// </summary>
  [JsonPropertyName("multiplex")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool? Multiplex { get; set; }

  /// <summary>
  ///   Text notes saved with the event, included in e-mail notifications.
  /// </summary>
  [JsonPropertyName("notes")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? Notes { get; set; }

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
  ///   An object containing the Plugin's custom parameters, filled out with values from the Event Editor.
  /// </summary>
  [JsonPropertyName("params")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public IDictionary<string, object>? Parameters { get; set; }

  /// <summary>
  ///   The ID of the Plugin which will run jobs for the event.
  /// </summary>
  [JsonPropertyName("plugin")]
  public string Plugin { get; set; } = null!;

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
  ///   If Multiplexing is enabled, this specifies the number of seconds to wait between job launches.
  /// </summary>
  [JsonPropertyName("stagger")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? Stagger { get; set; }

  /// <summary>
  ///   Events can target a Server Group (Group ID), or an individual server (hostname).
  /// </summary>
  [JsonPropertyName("target")]
  public string Target { get; set; } = null!;

  /// <summary>
  ///   The maximum allowed run time for jobs, specified in seconds.
  /// </summary>
  [JsonPropertyName("timeout")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public int? Timeout { get; set; }

  /// <summary>
  ///   The timezone for interpreting the event timing settings. Needs to be an IANA timezone string.
  /// </summary>
  [JsonPropertyName("timezone")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? Timezone { get; set; }

  /// <summary>
  ///   An object describing when to run scheduled jobs.
  /// </summary>
  [JsonPropertyName("timing")]
  public Timing? Timing { get; set; }

  /// <summary>
  ///   A display name for the event, shown on the Schedule Tab as well as in reports and e-mails.
  /// </summary>
  [JsonPropertyName("title")]
  public string Title { get; set; } = null!;

  /// <summary>
  ///   An optional URL to hit for the start and end of each job.
  /// </summary>
  [JsonPropertyName("web_hook")]
  [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public string? WebHook { get; set; }
}