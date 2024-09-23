using Blogplace.Web;
using Blogplace.Web.Auth;
using Blogplace.Web.Exceptions;
using Blogplace.Web.Services;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using static System.Net.Mime.MediaTypeNames;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

try
{
    Log.Information("Starting web application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog((context, loggerConfiguration) =>
    {
        loggerConfiguration.WriteTo.Console();
        loggerConfiguration.ReadFrom.Configuration(context.Configuration);
    });
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddMemoryCache();

    builder.Services.AddExceptionHandler<CustomExceptionHandler>();

    builder.Services
        .SetupAuth(builder.Configuration)
        .SetupMediatr()
        .SetupRepositories()
        .SetupEmail(builder.Configuration);

    var app = builder.Build();
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger().UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors(x => x
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials()
        .WithOrigins("http://localhost:3000" /*all frontend urls*/));
    app.UseCookiePolicy(new CookiePolicyOptions()
    {
        MinimumSameSitePolicy = SameSiteMode.None
    });
    app.UseAuthentication();
    app.UseAuthorization();

    app.UseExceptionHandler(_ => { });

    app.Use(async (ctx, next) => 
    {
        var sessionStorage = ctx.RequestServices.GetService<ISessionStorage>()!;
        sessionStorage.SetupHttpContext(ctx);
        await next();
    });

    app.MapControllers();
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program
{
}