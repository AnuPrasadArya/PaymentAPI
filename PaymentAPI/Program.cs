using Microsoft.EntityFrameworkCore;
using PaymentAPI.Application.Interfaces;
using PaymentAPI.Application.Services;
using PaymentAPI.Infrastructure.Data;
using PaymentAPI.Jobs;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
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


builder.Services.AddQuartz(q =>
{
   // q.UseMicrosoftDependencyInjectionScopedJobFactory();

    var jobKey = new JobKey("AutoConfirmPaymentJob");

    q.AddJob<AutoConfirmPaymentJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("AutoConfirmPaymentTrigger")
        .WithSchedule(CronScheduleBuilder
            .DailyAtHourAndMinute(0, 0) // 12
            .InTimeZone(TimeZoneInfo.Utc) // 
        )
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

app.UseAuthorization();

app.MapControllers();

app.Run();
