using Microsoft.EntityFrameworkCore;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using InvoiceSystem.Application.Services;
using InvoiceSystem.Application.Validators;
using InvoiceSystem.Application.Mappings;
using InvoiceSystem.Domain.Interfaces;
using InvoiceSystem.Domain.Entities;
using InvoiceSystem.Infrastructure.Data;
using InvoiceSystem.Infrastructure.Repositories;
using InvoiceSystem.Infrastructure.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Invoice System API", Version = "v1" });
});

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Hangfire configuration
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(c => c.UseNpgsqlConnection(connectionString)));
builder.Services.AddHangfireServer();

// AutoMapper configuration
builder.Services.AddAutoMapper(typeof(MappingProfile));

// FluentValidation configuration
builder.Services.AddValidatorsFromAssemblyContaining<CreateInvoiceValidator>();

// Register repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();

// Register services
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<NotificationJob>();

// CORS configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

app.UseAuthorization();

app.MapControllers();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

// Schedule recurring job for overdue notifications (daily at 9 AM)
RecurringJob.AddOrUpdate<NotificationJob>(
    "send-overdue-notifications",
    job => job.SendOverdueNotifications(),
    builder.Configuration["Hangfire:RecurringJobCron"] ?? "0 9 * * *");

app.Run();

// Hangfire authorization filter for development (allow all in dev)
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    public bool Authorize(DashboardContext context)
    {
        // In production, implement proper authorization
        return true;
    }
}
