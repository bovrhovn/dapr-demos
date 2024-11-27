using System.Net;
using System.Text.Json.Serialization;
using ATD.SM.Web.Helpers;
using ATD.SM.Web.Options;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDaprClient();
builder.Services.AddControllers().AddDapr().AddJsonOptions(options =>
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddOpenApi();
builder.Services.AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection("ApiOptions"));
builder.Services.AddHealthChecks();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(new CorsPolicyBuilder()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        .Build());
});
var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseRouting();
app.UseCors();
app.UseExceptionHandler(options =>
{
    options.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        context.Response.ContentType = "application/json";
        var exception = context.Features.Get<IExceptionHandlerFeature>();
        if (exception != null)
        {
            var message = $"{exception.Error.Message}";
            await context.Response.WriteAsync(message).ConfigureAwait(false);
        }
    });
});
app.MapHealthChecks($"/{RouteHelper.HealthRoute}", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();
app.MapControllers();
app.Run();