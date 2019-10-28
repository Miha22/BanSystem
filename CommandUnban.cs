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
            get { return "<player>"; }
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
                return new List<string>() { "globalban.unban" };
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
                if (caller == null)
                    GlobalBan.Instance.Discord.SendEmbedError("Invalid command usage, you must not have any parameters, but only player name");
                else
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }

            DatabaseManager.UnbanResult name = GlobalBan.Instance.Database.UnbanPlayer(command[0]);
            if (!SteamBlacklist.unban(new CSteamID(name.Id)) && string.IsNullOrEmpty(name.Name))
            {
                if (caller == null)
                    GlobalBan.Instance.Discord.SendEmbedError($"Failed to find player called: {command[0]}");
                else
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            if (caller == null)
                GlobalBan.Instance.Discord.SendChannelUnbanMessage($"The player " + name.Name + " was unbanned");
            else
                UnturnedChat.Say("The player " + name.Name + " was unbanned");
        }
    }
}
