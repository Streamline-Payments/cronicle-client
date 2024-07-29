#  CronicleClient

CronicleClient is a .NET client library for interacting with a Cronicle server. Cronicle is a multi-server task scheduler that can run both scheduled, repeating, and on-demand jobs (it's basically a fancy Cron replacement ;) ).

To learn more about Cronicle, visit the [Cronicle website](https://cronicle.com/) or the [Cronicle GitHub repository](https://github.com/jhuckaby/Cronicle).

## Installation

Visit the [setup documentation](https://github.com/jhuckaby/Cronicle/blob/master/docs/Setup.md) to get a Cronicle instance running.

```bash
dotnet add package CronicleClient
```

## Creating a client instance

```csharp
var cronicleClient = new CronicleClient.Client(
        baseUrl: "http://localhost:3012",
        apiToken: "<API_KEY>",
        logger: myLogger
    );
```

## Supported operations

(All operations should be supported. If you find one that isn't, please open an issue.)

| Cronicle Operation  | Client Class                                                                   |
|---------------------|--------------------------------------------------------------------------------|
| get_schedule        | `cronicleClient.Event.GetSchedule`                                             |
| get_event           | `cronicleClient.Event.GetEventById`<br/>`cronicleClient.Event.GetEventByTitle` |
| create_event        | `cronicleClient.Event.CreateEvent`                                             |
| update_event        | `cronicleClient.Event.UpdateEvent`                                             |
| delete_event        | `cronicleClient.Event.DeleteEvent`                                             |
| get_event_history   | `cronicleClient.Job.GetByEventId`                                              |
| get_history         | `cronicleClient.Event.GetHistory`                                              |
| run_event           | `cronicleClient.Event.RunEventById`<br/>`cronicleClient.Event.RunEventByTitle` |
| get_job_status      | `cronicleClient.Job.GetJobStatus`                                              |
| get_active_jobs     | `cronicleClient.Job.GetActiveJobs`                                             |
| update_job          | `cronicleClient.Job.UpdateJob`                                                 |
| abort_job           | `cronicleClient.Job.AbortJob`                                                  |
| get_master_state    | `cronicleClient.Master.GetMasterState`                                         |
| update_master_state | `cronicleClient.Master.UpdateMasterState`                                      |