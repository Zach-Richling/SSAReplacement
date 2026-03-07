using FluentMigrator.Runner;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SSAReplacement.Api.Endpoints;
using SSAReplacement.Api.Infrastructure;
using SSAReplacement.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var executablePath = builder.Configuration["Executables:Path"]
    ?? throw new Exception("Please defined Executables:Path environment variable.");

Directory.CreateDirectory(executablePath);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// FluentMigrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSQLite()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(Program).Assembly).For.Migrations());

// Hangfire
builder.Services.AddHangfire(config => config.UseInMemoryStorage());
builder.Services.AddHangfireServer();

// HTTP client for webhooks
builder.Services.AddHttpClient();

// Application services
builder.Services.AddScoped<IExecutableStorage, FileSystemExecutableStorage>();
builder.Services.AddScoped<IJobNotificationSender, WebhookNotificationSender>();
builder.Services.AddScoped<JobRunnerService>();
builder.Services.AddScoped<ScheduleRunnerService>();
builder.Services.AddScoped<IScheduleHangfireSyncService, ScheduleHangfireSyncService>();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// Run migrations at startup
using (var scope = app.Services.CreateScope())
{
    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
    runner.MigrateUp();

    var sync = scope.ServiceProvider.GetRequiredService<IScheduleHangfireSyncService>();
    await sync.SyncAllSchedulesAsync();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors();
app.UseHttpsRedirection();
app.MapHangfireDashboard();

app.MapScheduleEndpoints();
app.MapExecutableEndpoints();
app.MapExecutableVersionEndpoints();
app.MapJobEndpoints();
app.MapJobRunEndpoints();

app.Run();
