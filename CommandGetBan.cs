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
            System.Console.WriteLine();
            Logger.Log("------------------------------------------------------", ConsoleColor.Yellow);
            Logger.Log(ban.Duration.Ticks == ban.BanDate.Ticks || ban.Duration.Ticks < DateTime.Now.AddHours(-GlobalBan.Instance.UTCoffset).Ticks ? "| Result: NOT BANNED" : "| Result: BANNED", ConsoleColor.Yellow);
            Logger.Log($"| Player: {ban.Player}\tReason: {ban.Reason}\tAdmin: {ban.Admin}", ConsoleColor.Yellow);
            Logger.Log($"| Ban Date: { ban.BanDate.ToLongDateString()} UTC\tBanned till: {ban.Duration.ToLongDateString()}", ConsoleColor.Yellow);
            Logger.Log("------------------------------------------------------", ConsoleColor.Yellow);
            System.Console.WriteLine();
        }
    }
}
