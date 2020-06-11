using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BanSystem
{

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
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("invalid_command", Syntax), Color.red);
                return;
            }
            DatabaseManager.PlayerInfo banG = GlobalBan.Instance.DatabaseManager.GetBan(command[0], true);
            DatabaseManager.PlayerInfo banL = GlobalBan.Instance.DatabaseManager.GetBan(command[0], false);

            if (banG == null)
            {
                UnturnedChat.Say(caller, $"{command[0]} was not found in global database", Color.red);
            }
            else
            {
                ConsoleColor def = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"" +
                    $"\n| \t BAN CHECK\n" +
                    $"\n| Player Status: {(banG.Duration.Ticks < DateTime.Now.Ticks || banG.Duration == DateTime.MinValue ? "UNBANNED" : "BANNED")} " +
                    $"\n| Name: {banG.Charactername} \t Reason: {banG.Reason} \t Admin: {banG.Admin} " +
                    $"\n| Ban date: {(banG.BanDate == DateTime.MinValue ? "None" : banG.BanDate.AddHours(-GlobalBan.Instance.UTCoffset).ToString())} UTC \t Banned till: {(banG.Duration == DateTime.MaxValue ? "Permanent" : banG.Duration.Ticks < DateTime.Now.Ticks || banG.Duration == DateTime.MinValue ? "none" : banG.Duration.AddHours(-GlobalBan.Instance.UTCoffset).ToString())} UTC " +
                    $"\n| UTC Time now: {DateTime.UtcNow}");
                Console.ForegroundColor = def;
            }

            if (banL == null)
            {
                UnturnedChat.Say(caller, $"{command[0]} was not found in local database", Color.red);
            }
            else
            {
                ConsoleColor def = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"" +
                    $"\n| \t LOCAL CHECK\n" +
                    $"\n| Player Status: {(banL.Duration.Ticks < DateTime.Now.Ticks || banL.Duration == DateTime.MinValue ? "UNBANNED" : "BANNED")} " +
                    $"\n| Name: {banL.Charactername} \t Reason: {banL.Reason} \t Admin: {banL.Admin} " +
                    $"\n| Ban date: {(banL.BanDate == DateTime.MinValue ? "None" : banL.BanDate.AddHours(-GlobalBan.Instance.UTCoffset).ToString())} UTC \t Banned till: {(banL.Duration == DateTime.MaxValue ? "Permanent" : banL.Duration.Ticks < DateTime.Now.Ticks || banL.Duration == DateTime.MinValue ? "none" : banL.Duration.AddHours(-GlobalBan.Instance.UTCoffset).ToString())} UTC " +
                    $"\n| UTC Time now: {DateTime.UtcNow}");
                Console.ForegroundColor = def;
            }
        }
    }
}
