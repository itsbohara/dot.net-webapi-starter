using KIITStarter.Data;
using Microsoft.EntityFrameworkCore;
using KIITStarter.Helpers;
using KIITStarter.Middlewares;
using Serilog;
using Serilog.Events;
using KIITStarter.Repositories;

var builder = WebApplication.CreateBuilder(args);

// logger config
Log.Logger = new LoggerConfiguration()
 .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    // .WriteTo.Console()
    // file log
    .WriteTo.File(
         Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs/", "report-.txt"),
       rollingInterval: RollingInterval.Day,
       fileSizeLimitBytes: 10 * 1024 * 1024,
       retainedFileCountLimit: 2,
       rollOnFileSizeLimit: true,
       shared: true,
       flushToDiskInterval: TimeSpan.FromSeconds(1))
       .WriteTo.Logger(logger => logger.WriteTo.Sink(new LogReporter(builder.Configuration)))
    .CreateLogger();


// logger
builder.Host.UseSerilog();

// Add services to the container.
var services = builder.Services;

// intialize db
services.AddDbContext<AppDbContext>();

services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

//
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<ICourseRepository, CourseRepository>();
services.AddScoped<IJwtService, JwtService>();

services.AddScoped<ICourseService, CourseService>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<ISessionService, SessionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseMiddleware<JwtMiddleware>();

app.UseMiddleware<ExceptionMiddleware>();
// app.UseAuthentication();
// app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();

app.Run();

//
