using FluentMigrator.Runner;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using SSAReplacement.Api.Common.JobLogWriter;
using SSAReplacement.Api.Features.Auth;
using SSAReplacement.Api.Features.Auth.Infrastructure;
using SSAReplacement.Api.Features.Dashboard;
using SSAReplacement.Api.Features.Executables;
using SSAReplacement.Api.Features.Executables.Infrastructure;
using SSAReplacement.Api.Features.JobRuns;
using SSAReplacement.Api.Features.JobRuns.Infrastructure;
using SSAReplacement.Api.Features.Jobs;
using SSAReplacement.Api.Features.Jobs.Infrastructure;
using SSAReplacement.Api.Features.Admin;
using SSAReplacement.Api.Features.Schedules;
using SSAReplacement.Api.Features.Schedules.Infrastructure;
using SSAReplacement.Api.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var executablePath = builder.Configuration["Executables:Path"]
    ?? throw new Exception("Please defined Executables:Path environment variable.");

Directory.CreateDirectory(executablePath);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// FluentMigrator
builder.Services.AddFluentMigratorCore()
    .ConfigureRunner(rb => rb
        .AddSqlServer()
        .WithGlobalConnectionString(builder.Configuration.GetConnectionString("DefaultConnection"))
        .ScanIn(typeof(Program).Assembly).For.Migrations());

// Hangfire
builder.Services.AddHangfire(config => config.UseInMemoryStorage());
builder.Services.AddHangfireServer();

// Job log queue (singleton) and background writer
builder.Services.AddSingleton<JobLogQueue>();
builder.Services.AddSingleton<IJobLogQueue>(sp => sp.GetRequiredService<JobLogQueue>());
builder.Services.AddHostedService<JobLogWriterBackgroundService>();

// User tracking
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, HttpContextCurrentUserService>();
builder.Services.AddScoped<AuditService>();

// Application services
builder.Services.AddSingleton<JobCancellationService>();
builder.Services.AddScoped<IExecutableStorage, FileSystemExecutableStorage>();
builder.Services.AddScoped<JobRunnerService>();
builder.Services.AddScoped<ScheduleRunnerService>();
builder.Services.AddScoped<IScheduleHangfireSyncService, ScheduleHangfireSyncService>();

// Auth services
builder.Services.AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();
builder.Services.AddScoped<TokenService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
            ?? throw new InvalidOperationException("Jwt settings are not configured.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/runs"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddNegotiate();

builder.Services.AddAuthorization();

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration["AllowedOrigins"] ?? "https://localhost:7094";
        policy.WithOrigins(allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
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
app.UseAuthentication();
app.UseAuthorization();
app.MapHangfireDashboard();

app.MapAdminEndpoints();
app.MapScheduleEndpoints();
app.MapExecutableEndpoints();
app.MapExecutableVersionEndpoints();
app.MapJobEndpoints();
app.MapJobRunEndpoints();
app.MapDashboardEndpoints();
app.MapAuthEndpoints();

app.Run();
