# cronicle-client

# Creating a new cronicle client

## Installation

```bash
dotnet add package CronicleClient
```

## Creating a new instance

```csharp
using CronicleClient;

var cronicleClient = new Client(new Uri("http://localhost:3012"), "<API_KEY>");
```

## Other examples

[Create a new event](./examples/CreateEvent.cs)
[Get event by id](./examples/GetEvent.cs)
