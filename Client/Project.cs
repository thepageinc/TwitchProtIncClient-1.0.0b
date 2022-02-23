using System;

namespace TwitchProtIncClient.Config {
  public class ABOUT {
    private int     COR = 1;
    private string  EOL = Environment.NewLine;

    ///<summary>Project's name</summary>
    public  string  Name          { get =>  "TwitchProtIncClient 5000"; }
    ///<summary>Project's version</summary>
    public  string  Version       { get =>  "1.0.0b"; }
    ///<summary>Resume of the project</summary>
    public  string  Description   { get =>  "Prototype for Twitch IRC connections"; }
    ///<summary>Author(s) of this project</summary>
    public  string  Author        { get =>  "ThePageinc & friends."; }
    ///<summary>Compatibility informations</summary>
    public  string  Compatibility { get =>  "Developed on Linux Debian Bullseye with dotnet 6.0."; }
    ///<summary>Date this project started</summary>
    public  string  DateCreated   { get =>  "February 18th 2022"; }
    ///<summary>Date the project was released to public</summary>
    public  string  DateReleased  { get =>  "February 23rd 2022"; }
    ///<summary>License informations</summary>
    public  string  License       { get =>  "The Unlicense (see file)"; }
    ///<summary>Link for all informations about this project</summary>
    public  string  Link          { get =>  "https://github.com/thepageinc/TwitchProtIncClient-1.0.0b"; }
    ///<summary>Special thanks to people helping/collaborating in this project</summary>
    public  string  SpecialThanks { get =>  "Bu_Gee, CaLLowCreation, Josh n.k.a calmprogramming, deadcell, jimyyc, junicus,reaper_bs,  runtime_env, shyamgurunath for Twitch coding and moral supports"; }
    ///<summary>Full About..</summary>
    public  string  Full          { get =>  $"{Name} {Version}{EOL}" +
                                            $"{new String("-").PadLeft((Name.Length + Version.Length + COR), '-')}{EOL}" +
                                            $"{Description}{EOL}" +
                                            $"{Compatibility}{EOL}{EOL}" +
                                            $"Created on    : {DateCreated}{EOL}"+
                                            $"Released on   : {DateReleased}{EOL}" +
                                            $"Author(s)     : {Author}{EOL}" +
                                            $"License       : {License}{EOL}" +
                                            $"Download page : {Link}{EOL}" +
                                            $"Special thanks to {SpecialThanks}";
                                            
                                  }

    public ABOUT() {  }
  }
}