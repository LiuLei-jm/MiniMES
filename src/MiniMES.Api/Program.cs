using MiniMES.Api;
using MiniMES.Api.Common.Middleware;
using MiniMES.Api.Configuration;
using MiniMES.Application;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Infrastructure;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Persistence.Seed;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("Testing"))
{
    DevelopmentJwtConfiguration.EnsureSigningKey(builder.Configuration);
}

builder.Host.UseSerilog((ctx, services, config) =>
{
    config.ReadFrom.Configuration(ctx.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MiniMES")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30);
});


builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.EnablePersistAuthorization();
        options.UseRequestInterceptor(
            "(request) => {" +
            "const headers = request.headers || {};" +
            "const header = Object.keys(headers).find((name) => name.toLowerCase() === 'authorization');" +
            "if (!header) return request;" +
            "const token = String(headers[header]).match(/^(?:Bearer +)+([A-Za-z0-9_-]+[.][A-Za-z0-9_-]+[.][A-Za-z0-9_-]+)/i)?.[1];" +
            "if (token) headers[header] = `Bearer ${token}`;" +
            "return request;" +
            "}");
        options.UseResponseInterceptor(
            "(response) => {" +
            "try {" +
            "const path = new URL(response.url, window.location.origin).pathname.toLowerCase();" +
            "const tokenEndpoint = path === '/api/auth/login' || path === '/api/auth/refresh';" +
            "if (tokenEndpoint && response.status === 200) {" +
            "const body = response.obj || JSON.parse(response.text || '{}');" +
            "const token = body?.accessToken;" +
            "if (typeof token === 'string' && /^[A-Za-z0-9_-]+[.][A-Za-z0-9_-]+[.][A-Za-z0-9_-]+$/.test(token)) " +
            "window.ui.preauthorizeApiKey('Bearer', token);" +
            "}" +
            "} catch (error) { console.warn('Swagger could not apply the access token.', error); }" +
            "return response;" +
            "}");
    });
}
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseSerilogRequestLogging(options =>
{
    options.EnrichDiagnosticContext = (ctx, http) =>
    {
        ctx.Set("TraceId", http.TraceIdentifier);
        ctx.Set("Layer", "HTTP");
    };
    options.IncludeQueryInRequestPath = false;
    options.MessageTemplate =
    "HTTP {requestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});
app.UseHttpsRedirection();
app.UseCors(CorsPolicyNames.Frontend);
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

    // SQLite is the development/test store and has its own SQL Server-independent schema.
    // The existing SQL Server migrations remain the production migration history.
    if (DatabaseProvider.IsSqlite(builder.Configuration))
    {
        await db.Database.EnsureCreatedAsync();
    }

    var adminSeedOptions = builder.Configuration
        .GetSection(AdminSeedOptions.SectionName)
        .Get<AdminSeedOptions>() ?? new AdminSeedOptions();
    var allowGeneratedAdminPassword =
        app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing");
    var seedResult = await DataSeeder.SeedAsync(
        db,
        passwordHasher,
        adminSeedOptions,
        allowGeneratedAdminPassword);

    if (seedResult is { Created: true, GeneratedPassword: not null })
    {
        Console.WriteLine();
        Console.WriteLine("Default administrator account created.");
        Console.WriteLine($"Username: {seedResult.UserName}");
        Console.WriteLine($"Temporary password: {seedResult.GeneratedPassword}");
        Console.WriteLine("Set DataSeed__Admin__Password before first startup to choose a fixed development password.");
        Console.WriteLine();
    }
}

Log.Information("MiniMES Started");
app.Run();

