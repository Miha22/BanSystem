using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace BanSystem
{
    public class CommandWhite : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "whitelist";
        public string Help => "whitelists player";
        public string Syntax => "/white [player]";
        public List<string> Aliases => new List<string> { "white" };
        public List<string> Permissions => new List<string> { "bansystem.white" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                if (command.Length != 1)
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter") + $" Use: {Syntax}", Color.red);
                    return;
                }

                if (!PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
                {
                    UnturnedChat.Say(caller, $"Unable to find player: {command[0]}", Color.red);
                    return;
                }
                if (!GlobalBan.Instance.Database.WhiteList(targetPlayer.playerID.steamID))
                {
                    UnturnedChat.Say(caller, $"{targetPlayer.playerID.characterName} is already whitelisted!", Color.red);
                    return;
                }
                UnturnedChat.Say(caller, $"{targetPlayer.playerID.characterName} was whitelisted!", Color.white);
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
