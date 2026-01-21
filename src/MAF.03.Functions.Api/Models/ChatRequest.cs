namespace MAF._03.Functions.Api.Models;

public record ChatRequest(
    string Message,
    bool RequireApproval = false,
    string? ThreadId = null
);