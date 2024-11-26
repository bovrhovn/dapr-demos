using Spectre.Console;

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule($"[maroon]Hello Dapr[/]").RuleStyle("grey"));
AnsiConsole.WriteLine();

var whichSideCarToCall = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Which sidecar to [green]call[/]?")
        .PageSize(3)
        .AddChoices("Default sidecar", "With custom components"));

AnsiConsole.MarkupLine(
    $"Your selection is [blue]{whichSideCarToCall}[/], issuing call to that sidecar to get data..");

// before calling the below code, call dapr to initiate for the first call
// dapr run --app-id hello-dapr --dapr-http-port 3600
// or for the second call
// dapr run --app-id hello-dapr-custom --dapr-http-port 3700 --resources-path D:\Projects\atd-demos-dapr\ATD\ATD.Hello\dev-components

var url = whichSideCarToCall == "Default sidecar"
    ? "http://localhost:3600/v1.0/state/statestore/name"
    : "http://localhost:3700/v1.0/state/dev-statestore/key2";
using var httpClient = new HttpClient();
httpClient.DefaultRequestHeaders.Accept.Add(
    new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
try
{
    AnsiConsole.MarkupLine($"Issuing http call to [green]{url}[/]");
    var responseMessage = await httpClient.GetAsync(url);
    responseMessage.EnsureSuccessStatusCode();
    var keyValue = await responseMessage.Content.ReadAsStringAsync();
    AnsiConsole.WriteLine(keyValue);
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}