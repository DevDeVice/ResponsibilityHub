using ResponsibilityHub.ServiceBus;
public record EventGridModel(
    string Topic,
    string Subject,
    string EventType,
    DateTime EventTime,
    string Id,
    Data Data,
    string DataVersion,
    string MetadataVersion);
public record Data(
    string Api,
    string ClientRequestId,
    string RequestId,
    string ETag,
    string ContentType,
    int contentLength,
    string BlobType,
    string Url,
    string Sequencer,
    StorageDiagnostics storageDiagnostics);
public record StorageDiagnostics(string BatchId);