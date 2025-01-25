using CommerceMicro.IdentityService.Application.Startup;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider((context, options) =>
{
    // Service provider validation
    // Used for check got any DI misconfigured
    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
    options.ValidateOnBuild = true;
});

var assembly = typeof(Program).Assembly;
builder.AddInfrastructure(assembly);

var app = builder.Build();

app.UseInfrastructure();

app.Run();