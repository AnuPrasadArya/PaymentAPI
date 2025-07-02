using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PaymentAPI.API.Middleware;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Application.Services;
using PaymentAPI.Infrastructure.Consumers;
using PaymentAPI.Infrastructure.Data;
using PaymentAPI.Infrastructure.Messaging;
using PaymentAPI.Jobs;
using Quartz;
using Serilog;
using Serilog.Filters;
using System.Text;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);

// Configure JWT Authentication

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Set to true in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Issuer"], // or use "Audience" if defined separately
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

//  Redis 

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; 
    options.InstanceName = "CardValidation:";
});

//Api Versioning

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning) // suppress framework info logs
    .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    // CardValidation logs
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(Matching.WithProperty<string>("RequestPath", path => path.Contains("/api/CardValidation")))
        .WriteTo.File("Logs/cardvalidation.log", rollingInterval: RollingInterval.Day))

    // Payment logs
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(Matching.WithProperty<string>("RequestPath", path => path.Contains("/api/Payment")))
        .WriteTo.File("Logs/payment.log", rollingInterval: RollingInterval.Day))

    // Refund logs
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(Matching.WithProperty<string>("RequestPath", path => path.Contains("/api/Refund")))
        .WriteTo.File("Logs/refund.log", rollingInterval: RollingInterval.Day))
    .CreateLogger();
builder.Host.UseSerilog(); // Plug into ASP.NET Core
builder.Services.AddControllers();
builder.Services.AddLogging();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICardValidation, CardService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IRefundService, RefundService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddSingleton<IMessageBusPublisher, RabbitMqPublisher>();

builder.Services.AddHostedService<PaymentSuccessConsumer>();

// Quartz Backgorund Job
builder.Services.AddQuartz(q =>
{
   // q.UseMicrosoftDependencyInjectionScopedJobFactory();

    var jobKey = new JobKey("AutoConfirmPaymentJob");

    q.AddJob<AutoConfirmPaymentJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AutoConfirmPaymentJobTrigger")
         .WithCronSchedule("0 5 0 * * ?") // 12:05 AM
        //.WithCronSchedule("0 0/1 * * * ?") // 1minute
    );
   
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
