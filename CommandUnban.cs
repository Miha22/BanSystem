using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;

namespace BanSystem
{
    public class CommandUnban : IRocketCommand
    {
        public string Help
        {
            get { return "Unbanns a player"; }
        }

        public string Name
        {
            get { return "unban"; }
        }

        public string Syntax
        {
            get { return "/unban [player]\n/unban [steamid]"; }
        }

        public List<string> Aliases
        {
            get { return new List<string>(); }
        }

        public AllowedCaller AllowedCaller
        {
            get { return AllowedCaller.Both; }
        }

        public List<string> Permissions
        {
            get
            {
                return new List<string>() { "bansystem.unban" };
            }
        }
        internal static CommandUnban Instance;

        public CommandUnban()
        {
            Instance = this;
        }

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter") + $" Correct usage: {Syntax}");
                return;
            }
            DatabaseManager.UnbanResult unban = GlobalBan.Instance.Database.UnbanPlayer(command[0]);
            //if (!SteamBlacklist.unban(new CSteamID(ulong.Parse(name.Id))) || string.IsNullOrEmpty(name.Name))
            //{
            //    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
            //    return;
            //}
            if(unban == null)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            UnturnedChat.Say($"The player {unban.Player} was unbanned by {caller.DisplayName}", UnityEngine.Color.magenta);
            //Embed embed = new Embed
            //{
            //    fields = new Field[]
            //    {
            //        new Field("**Player**", unban.Player, true),
            //        new Field("**SteamID**", unban.SteamID, true),
            //        new Field("**Admin**", caller.DisplayName, true)
            //    },
            //    color = new Random().Next(16000000)
            //};
            //GlobalBan.Instance.SendInDiscord(embed, "Unban");
        }
    }
}
