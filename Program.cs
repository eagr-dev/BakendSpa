using BackendSpa.Application.Common.Behavior;
using BackendSpa.Application.Features.Citas.Querys;
using BackendSpa.Infrastructure;
using BackendSpa.Middlewares;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// MediatR — escanea todos los handlers de Application
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(GetCrearCita).Assembly);
    cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ExceptionHandlingBehavior<,>)); 
});

// Infrastructure — DbContext, MP, Twilio
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddRateLimitingPolicies(builder.Configuration);

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var ex = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        if (ex is not null)
        {
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                mensaje = ex.Error.Message,
                data = (object?)null
            });
        }
    });
});

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => "Bienvenido a la volta vida");

app.Run();