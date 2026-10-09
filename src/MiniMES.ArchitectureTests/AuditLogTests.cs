using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Logging;
using MiniMES.Application.Features.Audit.GetAuditLogs;
using MiniMES.Infrastructure.Authentication;
using MiniMES.Infrastructure.Persistence;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class AuditLogTests
{
    [Fact]
    public async Task Security_audit_service_persists_queryable_login_and_operation_events()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        var userId = Guid.NewGuid();
        var service = new SecurityAuditService(db, new TestRequestContext(userId));

        await service.LogAsync(new AuditEvent("Login", "Login", true, "admin", "User", userId.ToString()));
        await service.LogAsync(new AuditEvent("Operation", "User.Update", false, TargetType: "User", TargetId: "target", Details: "denied"));

        var result = await new GetAuditLogsQueryHandler(db)
            .Handle(new GetAuditLogsQuery("Operation", "User.Update", 1, 20), default);

        Assert.True(result.IsSuccess);
        var item = Assert.Single(result.Value!.Items);
        Assert.False(item.Success);
        Assert.Equal("admin", item.UserName);
        Assert.Equal(userId, item.UserId);
        Assert.Equal("trace-1", item.TraceId);
    }

    private sealed class TestRequestContext(Guid userId) : ICurrentRequestContext
    {
        public string TraceId => "trace-1";
        public string? UserId => userId.ToString();
        public string? UserName => "admin";
        public string ClientIp => "127.0.0.1";
    }
}
