namespace MAF._03.Functions.Api.Models;

public record PendingFunctionApproval(
    string Id,
    string CallId,
    string Name,
    IDictionary<string, object?>? ArgumentsJson
);