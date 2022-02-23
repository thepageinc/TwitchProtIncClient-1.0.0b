namespace TwitchProtIncClient {
  
  public class ChatMessageInEventArgs : EventArgs {
    public string?  Message { get; set; }
  }

  public class ClientConnectedEventArgs : EventArgs {
    public string?  Message { get; set; } = "The Client is connected";
    public string?  Url     { get; set; }
    public int      Port    { get; set; }
    public string?  Address { get; set; }
  }
}