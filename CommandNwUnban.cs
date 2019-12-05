using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;

namespace BanSystem
{
    public class CommandNwUnban : IRocketCommand
    {
        public string Help
        {
            get { return "Unbanns player globaly"; }
        }

        public string Name
        {
            get { return "nwunban"; }
        }

        public string Syntax
        {
            get { return "/nwunban [player]\n/unban [steamid]"; }
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
                return new List<string>() { "bansystem.nwunban" };
            }
        }
        //internal static CommandUnban Instance;

        //public CommandUnban()
        //{
        //    Instance = this;
        //}

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("invalid_command", Syntax), UnityEngine.Color.red);
                return;
            }
            DatabaseManager.UnbanResult unban = GlobalBan.Instance.Database.UnbanPlayer(command[0], true);
            //if (!SteamBlacklist.unban(new CSteamID(ulong.Parse(name.Id))) || string.IsNullOrEmpty(name.Name))
            //{
            //    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
            //    return;
            //}
            if (unban == null)
            {
                UnturnedChat.Say(caller, $"{command[0]} was not found in local database, try different name or steamID", UnityEngine.Color.red);
                return;
            }
            UnturnedChat.Say(GlobalBan.Instance.Translate("unban_public", unban.Player, caller.DisplayName), UnityEngine.Color.yellow);
            Embed embed = new Embed
            {
                fields = new Field[]
                {
                    new Field("**Player**", unban.Player, true),
                    new Field("**SteamID**", unban.SteamID, true),
                    new Field("**Admin**", caller.DisplayName, true)
                },
                color = int.Parse(GlobalBan.Instance.Translate("discord_bot_unban_color"))
            };
            GlobalBan.Instance.SendInDiscord(embed, GlobalBan.Instance.Translate("discord_bot_globalunban_name"));
        }
    }
}
