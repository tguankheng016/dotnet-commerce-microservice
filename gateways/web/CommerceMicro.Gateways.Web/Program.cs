using Microsoft.AspNetCore.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Commerce Micro Gateway");

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("Yarp"));

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(async (context, next) =>
    {
        // Before proxying
        var originalPath = context.Request.Path;
        var originalUrl = context.Request.GetDisplayUrl();

        await next(context);

        // After proxying
        var proxyPath = context.Request.Path;
        var proxyUrl = context.Request.GetDisplayUrl();

        // Log before and after
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("Original URL: {OriginalUrl}", originalUrl);
        logger.LogInformation("Proxy URL: {ProxyUrl}", proxyUrl);
    });
});

app.Run();
