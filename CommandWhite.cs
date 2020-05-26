using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;
using System.Text.RegularExpressions;

namespace BanSystem
{
    public class CommandWhite : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "whitelist";
        public string Help => "whitelists: name, ip, hwid, steamid";
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

                DatabaseManager.Ban ban = GlobalBan.Instance.Database.GetBan(command[0].Trim().ToLower());
                if (ban == null)
                {
                    //Regex regex = new Regex("^((25[0-5]|2[0-4][0-9]|[1]?[0-9][0-9]?).){3}(25[0-5]|2[0-4][0-9]|[1]?[0-9][0-9]?)$", RegexOptions.Compiled);
                    UnturnedChat.Say(caller, $"{command[0]} was not found in database, try different name, ip, hwid, steamid", Color.red);
                    return;
                }
                

                if (!GlobalBan.Instance.Database.WhiteList(ban.ip))
                {
                    UnturnedChat.Say(caller, $"{ban.Player} is already whitelisted!", Color.red);
                    return;
                }
                UnturnedChat.Say(caller, $"{ban.Player} was whitelisted by ip: {ban.ip}!", Color.white);
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
