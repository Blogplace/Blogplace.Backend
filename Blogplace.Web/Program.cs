using Blogplace.Web;
using Blogplace.Web.Auth;
using Blogplace.Web.Background.Jobs;
using Blogplace.Web.Commons.Logging;
using Blogplace.Web.Exceptions;
using Hangfire;
using Hangfire.Common;
using Serilog;
try
{
    var builder = WebApplication.CreateBuilder(args).SetupSerilog();

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddMemoryCache();
    builder.Services.AddExceptionHandler<CustomExceptionHandler>();

    builder
        .Services
        .SetupAuth(builder.Configuration)
        .SetupMediatr()
        .SetupRepositories()
        .SetupEmail(builder.Configuration)
        .SetupBackground();

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

    app.UseHangfireDashboard();
    Recurring<ImportBlogArticlesJob>(app, Cron.Daily(16));

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

void Recurring<T>(WebApplication app, string cronExpression)
    where T : IJob
{
    var name = typeof(T).Name;
    var job = app!.Services.GetService<T>()!;
    var jobsClient = app!.Services.GetService<IRecurringJobManagerV2>();
    jobsClient.AddOrUpdate(name, () => job.Run(), cronExpression);
}

public partial class Program
{
}
