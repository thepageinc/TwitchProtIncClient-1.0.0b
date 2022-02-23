# TwitchProtIncClient

## WHAT IS THIS?

This is a basic connection between a TCP Client and the IRC server of Twitch. This prototype is also using events.

### WHY?

I created it to start my own asynchronous Twitch library, but also to study the different socket connections offered with C#.

### WHY SHOULD I USE THIS?

- For educational purposes
- For entertainment puposes
- For development purposes.

..or any other cool thing you plan.

WARNING : malicious usages of this prototype are already discouraged, condemned, .. and could wake-up demons, which will torure you in every possible and unimaginable ways for the rest of your days.. or until you stop doing so.

## HOW-TO

```csharp
using TwitchProtIncClient;

private static TwProtIncClientOptions Options = new TwProtIncClientOptions();
public  static TwProtIncClient?       _client;

Options.Username  = "stefaneeb";                  // will be updated after connecting anyway
Options.Token     = Token.TwitchBot;              // Need more details? :P
Options.Channels  = "#thepageinc callowcreation"; // Join on connect (bot's added automatically)
Options.Debug     = true;                         // Doesn't change a thing, not set (see below)

_client = new TwProtIncClient(Options);
_client.ClientConnected += OnClientConnected;

await Task.Delay(-1);   // ..or whatever keeps the software active.

```

And for the handler..

```csharp
public static Task OnClientConnected(object? sender, ClientConnectedEventArgs e) {

  Console.WriteLine($"{e.Message} on {e.Address}");

  return Task.CompletedTask;
}
```

Yup! Coded for event handlers to be type of 'Task'.

## Not done a/p specs and warnings and to do for a full and complete bot connection..

- Not requesting membership
- Not connecting one SSL (port 6697)
- Debug option not working but ready to do so..
- Incoming chat messages (ready, not handled)
