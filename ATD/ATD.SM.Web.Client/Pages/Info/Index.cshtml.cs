using ATD.SM.Models;
using ATD.SM.Web.Client.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ATD.SM.Web.Client.Pages.Info;

public class IndexPageModel(ILogger<IndexPageModel> logger, StateApiClientCall stateApi) : PageModel
{
    public async Task OnGet(string email)
    {
        logger.LogInformation("Loaded info page");
        if (!string.IsNullOrEmpty(email))
        {
            logger.LogInformation($"Getting person by email {email}");
            var person = await stateApi.GetPersonAsync(email);
            RetrievedPerson = person;
            logger.LogInformation($"Person retrieved {RetrievedPerson.FullName}");
            Message = "";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        logger.LogInformation("Adding person to the API");
        try
        {
            await stateApi.SavePersonAsync(CurrentPerson);
            Message = $"Person {CurrentPerson.FullName} has been added!";
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            Message = "There has been an error! Check logs";
        }

        return RedirectToPage("/Info/Index", new {email = CurrentPerson.Email});
    }

    [TempData] public string Message { get; set; }
    [BindProperty] public Person CurrentPerson { get; set; } = new();
    [BindProperty] public Person RetrievedPerson { get; set; } = new();
    [BindProperty(SupportsGet = true)] public string Email { get; set; }
}