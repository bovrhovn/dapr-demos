using System.Net;
using ATD.SM.Web.Helpers;
using ATD.SM.Web.Options;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddDapr();
builder.Services.AddOpenApi();
builder.Services.AddOptions<ApiOptions>()
    .Bind(builder.Configuration.GetSection("ApiOptions"));
var app = builder.Build();

if (app.Environment.IsDevelopment()) app.MapOpenApi();

app.UseAuthorization();
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