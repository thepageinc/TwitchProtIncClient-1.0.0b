namespace TwitchProtIncClient {
  public class TwProtIncClientOptions {
    public  string?       Token     { get; set; } = null;
    public  string?       Username  { get; set; } = "Jerry";
    public  string?       Channels  { get; set; } = null;
    public  bool          Debug     { get; set; } = true;
    public  bool          SSL       { get; set; } = false;
    
    public TwProtIncClientOptions(string? token = null, string? username = null, string? channels = null, bool ssl = false, bool debug = false) {
      if (token is not null)
        Token = token;

      if (channels is not null)
        Channels = channels;

      if (username is not null)
        Username = username;
      
      Debug = debug;
      SSL   = ssl;
    }
  }
}