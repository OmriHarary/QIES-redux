using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;
using QIES.Core;
using QIES.Core.Config;
using QIES.Core.Services;
using QIES.Core.Users;
using QIES.Infra;
using QIES.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddJsonConsole();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resourceBuilder => resourceBuilder
        .AddService(serviceName: builder.Environment.ApplicationName))
    .WithTracing(builder => builder
        .AddAspNetCoreInstrumentation()
        .AddOtlpExporter())
    .StartWithHost();

builder.Services.Configure<ValidServicesListOptions>(
    builder.Configuration.GetSection(ValidServicesListOptions.Section));
builder.Services.Configure<TransactionSummaryOptions>(
    builder.Configuration.GetSection(TransactionSummaryOptions.Section));
builder.Services.AddSingleton<IServicesList, ValidServicesList>();
builder.Services.AddSingleton<IUserManager>(sp => new UserManager());
builder.Services.AddTransient<ISummaryWriter, SummaryWriter>();
builder.Services.AddTransactions();
builder.Services.AddControllers()
    .AddJsonOptions(opts => opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseOpenApi();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
