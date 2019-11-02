using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

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

    public class CommandGetBan : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "bans";

        public string Help => "get ban info of a player";

        public string Syntax => "/bans [player name]\n/bans [steamid]";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string> { "bansystem.bans" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter") + $" Use: {Syntax}", Color.red);
                return;
            }
            DatabaseManager.Ban ban = GlobalBan.Instance.Database.GetBan(command[0]);
            if(ban == null)
            {
                UnturnedChat.Say(caller, "Player not found, try different name or steamID", Color.yellow);
                return;
            }
            if(caller.DisplayName == "")
            Console.WriteLine();
            Logger.Log("------------------------------------------------------", ConsoleColor.Yellow);
            Logger.Log(ban.Duration.Ticks < DateTime.Now.Ticks || ban.Duration == DateTime.MinValue ? "| Player: NOT BANNED" : "| Player: BANNED", ConsoleColor.Yellow);
            Logger.Log($"| Player: {ban.Player}\tReason: {ban.Reason}\tAdmin: {ban.Admin}", ConsoleColor.Yellow);
            Logger.Log($"| Ban Date: {(ban.BanDate == DateTime.MinValue ? "none" : ban.BanDate.AddHours(-GlobalBan.Instance.UTCoffset).ToString())} UTC\tBanned till: {(ban.Duration == DateTime.MaxValue ? "Permanent" : ban.Duration.Ticks < DateTime.Now.Ticks || ban.Duration == DateTime.MinValue ? "none" : ban.Duration.AddHours(-GlobalBan.Instance.UTCoffset).ToString())} UTC", ConsoleColor.Yellow);
            Logger.Log($"| UTC Time now: {DateTime.UtcNow}", ConsoleColor.Yellow);
            Logger.Log("------------------------------------------------------", ConsoleColor.Yellow);
            Console.WriteLine();
        }
    }
}
