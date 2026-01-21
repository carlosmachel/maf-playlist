namespace MAF._03.Functions.Api.Models;

public record ChatResponse(
    bool Success,
    string? Message,
    string? ThreadId,
    List<PendingFunctionApproval>? PendingApprovals
);