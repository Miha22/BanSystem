using Rocket.API;
using SDG.Unturned;
using System.Collections.Generic;
using System;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;

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
            get { return "<player> [reason]"; }
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
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
                return;
            }
            UnturnedPlayer playerToKick = UnturnedPlayer.FromName(command[0]);
            if (playerToKick == null)
            {
                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                return;
            }
            string reason = command.Length == 1 ? "N/A" : command[1];
            UnturnedChat.Say(GlobalBan.Instance.Translate("command_kick_public_reason", playerToKick.CharacterName, reason));
            Provider.kick(playerToKick.CSteamID, reason);
            //Embed embed = new Embed
            //{
            //    fields = new Field[]
            //    {
            //            new Field("**Player**", playerToKick.CharacterName, true),
            //            new Field("**\t\t\tSteamID**", playerToKick.CSteamID.ToString(), true),
            //            new Field("**Reason**", reason, true),
            //            new Field("**Admin**", caller.DisplayName, true),
            //            new Field("**Map**", $"\t{Provider.map}", true)
            //    },
            //    color = new Random().Next(16000000)
            //};
            //GlobalBan.Instance.SendInDiscord(embed, "Kick");
        }
    }
}
