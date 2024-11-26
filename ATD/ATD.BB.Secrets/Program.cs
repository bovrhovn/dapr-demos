using Spectre.Console;

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule($"[maroon]Hello Dapr[/]").RuleStyle("grey"));
AnsiConsole.WriteLine();

var whichSecretStore = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Which sidecar to [green]call[/]?")
        .PageSize(3)
        .AddChoices("Local", "Azure KeyVault"));

AnsiConsole.MarkupLine(
    $"Your selection is [blue]{whichSecretStore}[/], issuing call to that sidecar to get data..");
var url = whichSecretStore == "Local"
    ? "http://localhost:3900/v1.0/secrets/envvar-secret-store/mysecret"
    : "http://localhost:3900/v1.0/secrets/azurekeyvaultsecretstore/mysecret";
            
using var httpClient = new HttpClient();
            
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