using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using UnityEngine;

namespace BanSystem
{
    public class CommandKick : IRocketCommand
    {
        public string Help
        {
            get { return "Kicks a player"; }
        }

        public string Name
        {
            get { return "kick"; }
        }

        public string Syntax
        {
            get { return "/kick [player] [reason] or /kick [player]"; }
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
                return new List<string> { "bansystem.kick" };
            }
        }

        public void Execute(IRocketPlayer caller, params string[] command)
        {

            if (command.Length == 0 || command.Length > 2)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("invalid_command", Syntax), Color.red);
                return;
            }
            UnturnedPlayer playerToKick = UnturnedPlayer.FromName(command[0]);
            if (playerToKick == null)
            {
                UnturnedChat.Say(caller, $"{command[0]} was not found on server", Color.red);
                return;
            }
            string reason = command.Length == 1 ? "N/A" : command[1];
            UnturnedChat.Say(GlobalBan.Instance.Translate("kick_public", playerToKick.CharacterName, reason, caller.DisplayName), Color.green);
            Provider.kick(playerToKick.CSteamID, GlobalBan.Instance.Translate("kick_private", reason, caller.DisplayName));
            Embed embed = new Embed
            {
                fields = new Field[]
                {
                        new Field("**Server**", $"{GlobalBan.ServerName ?? "N/A"}", false),
                        new Field("**Player**", playerToKick.DisplayName, true),
                        new Field("**SteamID**", playerToKick.CSteamID.ToString(), true),
                        new Field("**Reason**", reason, true),
                        new Field("**Admin**", caller.DisplayName, true),
                        new Field("**Map**", $"\t{Provider.map}", true)
                },
                //color = new System.Random().Next(16000000)
                color = int.Parse(GlobalBan.Instance.Translate("discord_bot_kick_color"))
            };
            GlobalBan.Instance.SendInDiscord(embed, GlobalBan.Instance.Translate("discord_bot_kick_name"));
        }
    }
}
