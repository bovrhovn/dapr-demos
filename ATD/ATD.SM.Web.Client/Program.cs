using System.Net;
using ATD.SM.Web.Client.Options;
using ATD.SM.Web.Client.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions<WebOptions>()
    .Bind(builder.Configuration.GetSection("WebOptions"));
builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options => options.Conventions.AddPageRoute("/Info/Index", ""));
builder.Services.AddHttpClient<StateApiClientCall>();
builder.Services.AddHealthChecks();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();
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
app.MapHealthChecks($"/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();
app.MapRazorPages();
app.Run();