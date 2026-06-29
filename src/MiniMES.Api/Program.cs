using MiniMES.Api;
using MiniMES.Api.Common.Exceptions;
using MiniMES.Application;
using MiniMES.Application.Common.Security;
using MiniMES.Infrastructure;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Persistence.Seed;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
    await DataSeeder.SeedAsync(db, passwordHasher);
}

app.Run();

