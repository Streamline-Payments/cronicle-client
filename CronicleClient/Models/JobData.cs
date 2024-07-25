using System;
using System.Text.Json.Serialization;

namespace CronicleClient.Models;

public record JobData : EventData
{
  /// <summary>
  /// The hostname of the server currently running, or the server who ran the job.
  /// </summary>
  [JsonPropertyName("hostname")]
  public string? HostName { get; set; }
  
  /// <summary>
  /// If the job was started manually via user or API, this will contain a text string identifying who it was.
  /// </summary>
  [JsonPropertyName("source")]
  public string? Source { get; set; }
  
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("log_file_size")]
  public int? LogFileSize { get; set; }
  
  /// <summary>
  /// A local filesystem path to the job's log file (only applicable if job is in progress).
  /// </summary>
  [JsonPropertyName("log_file")]
  public string? LogFile { get; set; }
  
  /// <summary>
  /// The main PID of the job process that was spawned.
  /// </summary>
  [JsonPropertyName("pid")]
  public int? Pid { get; set; }
  
  /// <summary>
  /// Current progress of the job, from 0.0 to 1.0, as reported by the Plugin (optional).
  /// </summary>
  [JsonPropertyName("progress")]
  public decimal? Progress { get; set; }
  
  /// <summary>
  /// Will be set to 1 when the job is complete, omitted if still in progress.
  /// </summary>
  [JsonPropertyName("complete")]
  [JsonConverter(typeof(Common.BoolToIntJsonConverter))]
  public bool? Complete { get; set; }
  
  /// <summary>
  /// A code representing job success (0) or failure (any other value). Only applicable for completed jobs.
  /// </summary>
  [JsonPropertyName("code")]
  public int? Code { get; set; }
  
  /// <summary>
  /// If the job failed, this will contain the error message. Only applicable for completed jobs.
  /// </summary>
  [JsonPropertyName("description")]
  public string? Description { get; set; }

  /// <summary>
  /// If the job is abort, this will contain the reason. Only applicable for aborted jobs.
  /// </summary>
  [JsonPropertyName("abort_reason")]
  public string? AbortReason { get; set; }

  /// <summary>
  /// Performance metrics for the job, if reported by the Plugin (optional). Only applicable for completed jobs.
  /// </summary>
  [JsonPropertyName("perf")]
  public string? Perf { get; set; }
  
  /// <summary>
  /// A Unix Epoch timestamp of when the job started.
  /// </summary>
  [JsonPropertyName("time_start")]
  public double? TimeStart { get; set; }
  
  /// <summary>
  /// A Unix Epoch timestamp of when the job completed. Only applicable for completed jobs.
  /// </summary>
  [JsonPropertyName("time_end")]
  public double? TimeEnd { get; set; }
  
  /// <summary>
  /// The elapsed time of the job, in seconds.
  /// </summary>
  [JsonPropertyName("elapsed")]
  public double? Elapsed { get; set; }
  
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("action")]
  public string? Action { get; set; }
  
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("epoch")]
  public double? Epoch { get; set; }
  
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("event_title")]
  public string? EventTitle { get; set; }
  
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("category_title")]
  public string? CategoryTitle { get; set; }
  
  /// <summary>
  /// 
  /// </summary>
  [JsonPropertyName("plugin_title")]
  public string? PluginTitle { get; set; }
  
  /// <summary>
  /// If the job has a TimeStart value, this will be a DateTime object of that value, in UTC.
  /// </summary>
  [JsonIgnore]
  public DateTime? TimeStartUtc => (TimeStart.HasValue ? new DateTime(1970, 01, 01, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(TimeStart.Value) : null) ;
}