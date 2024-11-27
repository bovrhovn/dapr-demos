using System.Net.Mime;
using ATD.SM.Models;
using ATD.SM.Web.Helpers;
using ATD.SM.Web.Options;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ATD.SM.Web.Controllers;

[AllowAnonymous, ApiController, Route(RouteHelper.PersonRoute),
 Produces(MediaTypeNames.Application.Json)]
public class PersonController(
    ILogger<PersonController> logger,
    DaprClient client,
    IOptions<ApiOptions> webSettingsValue) : ControllerBase
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
    
    [HttpGet]
    [Route("byobject/{person}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get([FromState("statestore")] StateEntry<Person> person)
    {
        if (person.Value is null) return NotFound();
        return Ok(person.Value);
    }

    [HttpGet]
    [Route("byemail/{email}")]
    [Produces(typeof(Person))]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByEmailAsync(string email)
    {
        try
        {
            logger.LogInformation($"Getting data for specific {email}");
            var person = await client.GetStateAsync<Person>(webSettingsValue.Value.StoreStateName, email);
            if (person == null)
            {
                logger.LogInformation("No data was found in the system");
                return Ok(new Person());
            }

            logger.LogInformation("Data received: {FullName}", person.FullName);
            return Ok(person);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return NotFound();
        }
    }

    [HttpPost]
    [Route("add")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> SavePersonAsync([FromBody]Person person)
    {
        logger.LogInformation("Saving person {FullName}", person.FullName);
        try
        {
            await client.SaveStateAsync(webSettingsValue.Value.StoreStateName, person.Email, person);
            logger.LogInformation("Person {FullName} was saved", person.FullName);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message, statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        return Ok();
    }
}