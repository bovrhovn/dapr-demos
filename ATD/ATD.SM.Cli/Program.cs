using ATD.SM.Cli;
using Dapr.Client;
using Spectre.Console;
const string storeName = "tablestorage";
const string stateKeyName = "person-key";
AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule("[maroon]State management[/]").RuleStyle("grey"));
AnsiConsole.WriteLine();

using var client = new DaprClientBuilder().Build();

var person = new Person {FullName = "Igor TheRockStar", Email = "igor@rockstar.eu", Age = 30};

AnsiConsole.MarkupLine($"Adding person [green]{person.FullName}[/] to the state");

try
{
    var cancellationToken = CancellationToken.None;
    await client.SaveStateAsync(storeName, stateKeyName, person, cancellationToken: cancellationToken);

    AnsiConsole.MarkupLine("Person added - let's retrieve the value");
    person = await client.GetStateAsync<Person>(storeName, stateKeyName, cancellationToken: cancellationToken);

    var table = new Table()
        .AddColumn("Full name")
        .AddColumn(new TableColumn("Email").Centered())
        .AddColumn("Age");

    table.AddRow(person.FullName, person.Email, person.Age.ToString());
    AnsiConsole.Write(table);

    AnsiConsole.MarkupLine("Person will be deleted");
   // await client.DeleteStateAsync(storeName, stateKeyName, cancellationToken: cancellationToken);
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}
AnsiConsole.MarkupLine("Person was deleted from state.");