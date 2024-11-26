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
    
    [HttpGet("byobject/{person}")]
    public ActionResult<Person> Get([FromState("statestore")] StateEntry<Person> person)
    {
        if (person.Value is null) return NotFound();
        return person.Value;
    }

    [HttpGet("byemail/{email}")]
    [Produces(typeof(Person))]
    public async Task<ActionResult<Person>> GetByEmail(string email)
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

            logger.LogInformation($"Data received: {person.FullName}");
            return person;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return NotFound();
        }
    }

    [HttpPost("add")]
    public async Task<ActionResult<Person>> SavePerson(Person person)
    {
        logger.LogInformation($"Saving person {person.FullName}");
        try
        {
            await client.SaveStateAsync(webSettingsValue.Value.StoreStateName, person.Email, person);
            logger.LogInformation($"Person {person.FullName} was saved");
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return Problem(e.Message);
        }

        return Ok();
    }
}