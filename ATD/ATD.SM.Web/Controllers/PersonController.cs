using System.Net.Mime;
using ATD.SM.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ATD.SM.Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.PersonRoute),
 Produces(MediaTypeNames.Application.Json)]
public class PersonController(ILogger<PersonController> logger) : ControllerBase
{
    [HttpGet]
    [Route(RouteHelper.HealthRoute)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult IsAlive()
    {
        logger.LogInformation("Called alive data endpoint at {DateCalled}", DateTime.UtcNow);
        return new ContentResult
            { StatusCode = 200, Content = $"I am alive at {DateTime.Now} on {Environment.MachineName}" };
    }
}