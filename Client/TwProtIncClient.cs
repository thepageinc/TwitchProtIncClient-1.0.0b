using System.Net.Sockets;

using TwitchProtIncClient.Config;

namespace TwitchProtIncClient {
  public class TwProtIncClient {
    
    private const char    CHAN_PREF       = ConfSet.CHAN_PREF;

    private const string  EOL             = ConfSet.EOL,
                          PING_QUERY      = ConfSet.PING_QUERY,
                          PING_REPLY      = ConfSet.PING_REPLY,
                          URi             = ConfSet.URi;

    private const int     JOIN_DELAY      = ConfSet.JOIN_DELAY,
                          USR_MIN_LENGTH  = ConfSet.USERNAME_MIN_LENGTH,
                          PORT            = ConfSet.PORT,
                          TOKEN_LENGTH    = ConfSet.TOKEN_LENGTH;


    private StreamReader?           _reader;
    private StreamWriter?           _writer;

    private bool?                   _debug;
    private string?                 _twToken, _username;

    private List<string>            _chanList = new();
    private TcpClient               _socket   = new();
    private CancellationTokenSource _dcToken  = new();
    private List<Task>              _waitFor  = new();
    
    // Events needs a delegate to be Async
    public  delegate  Task  ChatMessageInEventHandler(Object sender, ChatMessageInEventArgs e);
    public  delegate  Task  ClientConnectedEventHandler(Object? sender, ClientConnectedEventArgs e);

    // Event to trigger from
    public  event ChatMessageInEventHandler?    ChatMessageIn;
    public  event ClientConnectedEventHandler?  ClientConnected;
    
    public TwProtIncClient(TwProtIncClientOptions e) {
      Console.WriteLine($"{new ABOUT().Full}{EOL}");
      ValidateConfSet(options: e).GetAwaiter().GetResult();
      Connect();
    }

    ///<summary>
    ///   Method <c>Connect</c>:
    ///   connects and lauches listening processing.
    ///</summary>
    ///<remarks>
    ///</remarks>
    private async void Connect() {
      try {
        await _socket.ConnectAsync(URi, PORT, _dcToken.Token);

        if (_socket.Connected) {
          var c = new ClientConnectedEventArgs();
          
          c.Url     = $"{URi}";
          c.Port    = PORT;
          c.Address = $"{URi}:{PORT}";

          _reader = new StreamReader(_socket.GetStream());
          _writer = new StreamWriter(_socket.GetStream()) { NewLine = EOL, AutoFlush = true };

          await OnClientConnected(c);
        } else {
          throw new Exception("Unable to connect.");
        }

        while (!_dcToken.IsCancellationRequested) {
          string? Line;

          // THESE return some ? at the end of each line received.. probably an
          // encoding issue..?
          //char[] buffer = new char[1024];
          //var Received  = await _reader.ReadAsync(buffer, _dcToken.Token);
          //    Line      = Convert.ToString(buffer);

          Line = await _reader.ReadLineAsync();

          if (Line != null) {
            if (_twToken != null)
              Line.Replace(_twToken, "***");
            
            Logger(Line);

            if (Line.Contains("001") && Line.Contains("Welcome") && (!Line.Contains("PRIVMSG"))) {
              
              //Client is logged in.
              
              var Arr   = Line.Split(' ');
              var Nick  = Arr[2].Trim();

              if (_username != null)
                if (_username.ToLower() != Nick.ToLower()) {
                  Logger("WARNING: Twitch username differs then username entered. Using Twitch username from now on.");
                }
              
              _username = Nick;

              // Bot's channel joined is the one of the connection.
              _chanList.Insert(0, $"{CHAN_PREF}{_username}");
              
              JoinChannels();
            }

            if (Line.StartsWith(PING_QUERY)) {
              await SendAsync(PING_REPLY);
              Logger($"{PING_QUERY}! {PING_REPLY}!");
            }
          }
        }

      } catch(Exception ex) {
        Logger($"{ex}");
      }
    }

    ///<summary>
    ///   Method <c>JoinChannels</c>:
    ///   joins channels listed after connection.
    ///</summary>
    private void JoinChannels() {
      int Round = 1;
      foreach (var chan in _chanList) {
        new System.Timers.Timer(JOIN_DELAY * Round)
          { AutoReset = false, Enabled = true }.Elapsed +=  (async (s, e) => { await SendAsync($"JOIN {chan}"); });
        
        Round++;
      }
    }

    ///<summary>
    ///   Method <c>Logger</c>:
    ///   log <paramref name="message" /> to console
    ///</summary>
    ///<param name="message">message to log</param>
    public void Logger(string message) {
      Console.WriteLine($"[{DateTime.Now}] {message}");
    }

    ///<summary>
    ///   Method <c>LoginAsync</c> (Task):
    ///   send loging request to Twitch server.
    ///</summary>
    private async void LoginAsync() {
      await SendAsync($"PASS oauth:{_twToken}");
      await SendAsync($"NICK {_username}");
    }

    ///<summary>
    ///   Method <c>SendAsync</c> (Task):
    ///   send <paramref name="message" /> to Twitch server.
    ///</summary>
    ///<param name="message">The message to send</param>
    ///<exception cref="NullReferenceException"></exception>
    public async Task SendAsync(string message) {
      if (_writer == null) 
        throw new NullReferenceException("_writer is null in TCPSocket.SendAsyn()");

      await _writer.WriteLineAsync($"{message}");
    }

    ///<summary>
    ///   Method <c>ValidateConfSet</c> (Task):
    ///   validates configuration and settings before connecting.
    ///</summary>
    ///<remarks>
    ///   Validates the token, the username and the channels list to join on connect.
    ///</remarks>
    ///<param name="options">The TwitchProtIncClientOption object</param>
    ///<exception cref="ArgumentException"></exception>
    ///<exception cref="ArgumentNullException"></exception>
    ///<exception cref="FormatException"></exception>
    private Task ValidateConfSet(TwProtIncClientOptions options) {
      // Variables ARE NOT all declared at the top
      var Username  = options.Username;
      var Token     = options.Token;
      var Channels  = options.Channels;
      var CmptModed = 0;

      if (Token is null)
        throw new ArgumentNullException("You need a token to connect! Quick fix, visit: https://twitchapps.com/tmi/");
      
      if (Token.Count() < TOKEN_LENGTH)
        throw new FormatException($"The token seems invalid since it is less than {TOKEN_LENGTH} characters long.{EOL}"+
                                  "Quick fix get one at : https://twitchapps.com/tmi/");
     
      _twToken = Token;
      
      if (Username is not null) {
        if (Username.Count() < USR_MIN_LENGTH)
          throw new ArgumentException($"Username seems invalid. It requires at least {USR_MIN_LENGTH} characters.");

        if (!ValidateName(Username))
          throw new FormatException("Invalid username. It must contain only letters, numbers and underscores.");
        
        _username = Username;
      }

      // All channels need to start with '#'
      // Channels can only contains letters, numbers and underscores
      Logger("Validating channels list");

      if (Channels is null) {
        Logger("WARNING: NO CHANNEL TO JOIN. Bot will join it's own chat room only!");
      } else {
        var tmpList = Channels.Split(" ");
        for (var i = 0; i < tmpList.Count(); i++) {
          if (!tmpList[i].StartsWith(CHAN_PREF)) {
            tmpList[i] = $"{CHAN_PREF}{tmpList[i]}";
            CmptModed++;
          }
          if (!ValidateName(tmpList[i], true)) {
            throw new FormatException($"Channel name can contain only letters, numbers, undercores and must start with a '{CHAN_PREF}'{EOL}" +
                                      $"Make sure, also, channels are separated by a space.");
          }
        }
      
        Logger($"{CmptModed} channel(s) needed to be modified.");
        _chanList = _chanList.Concat(tmpList).ToList();
      }

      return Task.CompletedTask;
    }

    ///<summary>
    ///   Method <c>ValidateName</c>:
    ///   validates that <paramref name="name" /> contains only allowed
    ///   characters.
    ///</summary>
    ///<remarks>
    ///   Name can only contain letters, numbers and underscores. When it is a
    ///   channel name, '#' is also allowed.
    ///</remarks>
    ///<returns>A boolean indicating if the name is valid or not</returns>
    ///<param name="name">a string with a name to validate</param>
    ///<param name="isChannel">if the name is a channel name or not</param>
    private bool ValidateName(string name, bool isChannel = false) {
      if (isChannel)
        return name.All(c => Char.IsLetterOrDigit(c) || c == '_' || c == CHAN_PREF);
      else
        return name.All(c => Char.IsLetterOrDigit(c) || c == '_');
    }


    protected async virtual Task OnChatMessageIn(ChatMessageInEventArgs e) {
      ChatMessageInEventHandler? handler = ChatMessageIn;

      if (handler is not null)
        await handler(this, e);
    }

    protected async virtual Task OnClientConnected(ClientConnectedEventArgs e) {
      ClientConnectedEventHandler? handler = ClientConnected;
      LoginAsync();

      if (handler is not null)
        await handler(this, e);

    }
  }

}