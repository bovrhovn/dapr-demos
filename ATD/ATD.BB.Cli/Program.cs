using ATD.BB.Cli;
using Dapr.Client;
using Spectre.Console;

AnsiConsole.WriteLine();
AnsiConsole.Write(new Rule($"[maroon]Dapr Building Blocks[/]").RuleStyle("grey"));
AnsiConsole.WriteLine();
//var pubsubName = "servicebus-pubsub";
var pubsubName = "servicebus-pubsub";
//publish events via service bus
using var client = new DaprClientBuilder().Build();

var cancellationToken = CancellationToken.None;
AnsiConsole.MarkupLine("Sending random events to the topic");

try
{
    for (int counter = 1; counter <= 20; counter++)
    {
        var eventData = new AmmountMessage {Id = counter, Ammount = 100 * counter};
        await client.PublishEventAsync(pubsubName, "messages", eventData, cancellationToken);
        AnsiConsole.WriteLine($"We published an event with id {counter}");
    }
}
catch (Exception e)
{
    AnsiConsole.WriteException(e);
}

AnsiConsole.MarkupLine("Finished sending [green]all[/] events");