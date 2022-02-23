namespace TwitchProtIncClient.Config {
  public class ConfSet {
    public  const   char          CHAN_PREF           = '#';

    public  const   string        EOL                 = "\r\n",
                                  PING_QUERY          = "PING",
                                  PING_REPLY          = "PONG",
                                  URi                 = "irc.chat.twitch.tv";

    public  const   int           JOIN_DELAY          = 2000,
                                  USERNAME_MIN_LENGTH = 3,
                                  PORT                = 6667,
                                  TOKEN_LENGTH        = 30;

  }
}