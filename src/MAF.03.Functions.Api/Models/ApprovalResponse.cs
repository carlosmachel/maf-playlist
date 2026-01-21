namespace MAF._03.Functions.Api.Models;

public record ApprovalResponse(
    string ThreadId,
    string ApprovalId,
    string CallId,
    string FunctionName,
    IDictionary<string, object?>? ArgumentsJson,
    bool Approved
);