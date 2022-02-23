using TwitchProtIncClient;
using TwitchProtIncClient.Tokens;

namespace x {
  class Program {
    private static TwProtIncClientOptions Options = new TwProtIncClientOptions();
     public static TwProtIncClient?       _client;
    
    public static Task OnClientConnected(object? sender, ClientConnectedEventArgs e) {

      Console.WriteLine($"{e.Message} on {e.Address}");

      return Task.CompletedTask;
    }

    public static void Main(string[] args)
      => new Program().MainAsync(args).GetAwaiter().GetResult();

    public async Task MainAsync(string[] args) {

      Options.Username  = "stefanieb";
      Options.Token     = Token.TwitchBot;
      Options.Channels  = "#thepageinc callowcreation";
      
      _client = new TwProtIncClient(Options);
      _client.ClientConnected += OnClientConnected;

      await Task.Delay(-1);

    }

  }
}
