using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

namespace BanSystem
{
    //public class Command : IRocketCommand
    //{
    //    public AllowedCaller AllowedCaller => AllowedCaller.Both;

    //    public string Name => "gg";

    //    public string Help => "";

    //    public string Syntax => "";

    //    public List<string> Aliases => new List<string>();

    //    public List<string> Permissions => new List<string>();

    //    public void Execute(IRocketPlayer caller, string[] command)
    //    {
    //        System.Console.WriteLine(caller);
    //        System.Console.WriteLine(caller.Id);
    //        System.Console.WriteLine(caller.DisplayName);
    //        System.Console.WriteLine(caller.IsAdmin);
    //        Rocket.API.ConsolePlayer
    //        Console
    //        Console
    //        True
    //    }
    //}

    public class ShowConnect : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "showconnect";

        public string Help => "Shows detailed info about connecting player";

        public string Syntax => "/showconnect /showcon";

        public List<string> Aliases => new List<string> { "showcon" };

        public List<string> Permissions => new List<string> { "bansystem.showconnect" };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 0)
            {
                UnturnedChat.Say(caller, $"Invalid command usage. Syntax: {Syntax}");
                return;
            }
            GlobalBan.Instance.Configuration.Instance.ShowConnectInfo = !GlobalBan.Instance.Configuration.Instance.ShowConnectInfo;
            UnturnedChat.Say(caller, $"{(GlobalBan.Instance.Configuration.Instance.ShowConnectInfo ? "Showing detailed info on connecting players" : "Disabling detailed info show on connected players")}");
        }
    }
}
