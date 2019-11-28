using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
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

    public class CommandBan : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "ban";
        public string Help => "Bans player, use /help to find syntax";
        public string Syntax => "/ban [player] \n/ban [player] [reason]\n/ban [player] [reason] [duration]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "bansystem.ban" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter") + $" Use: {Syntax}", Color.red);
                    return;
                }
                //string charactername;
                //if (!PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
                //{
                //    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"), Color.red);
                //    return;
                //}
                //SteamGameServerNetworking.GetP2PSessionState(targetPlayer.playerID.steamID, out P2PSessionState_t pConnectionState);
                //string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                //string hwid = GlobalBan.Instance.GetHWidString(targetPlayer.playerID.hwid);
                string reason = command.Length == 1 ? "N/A" : command[1];
                uint duration = 0U;

                if (command.Length == 3 && !uint.TryParse(command[2], out duration))
                {
                    if(!uint.TryParse(command[2].Substring(0, 1), out uint mult))
                    {
                        UnturnedChat.Say(caller, "Unabled to ban player: Invalid ban time", Color.red);
                        return;
                    }
                    switch (command[2].Substring(1).ToLower())
                    {
                        case "h":
                            duration = 3600u * mult;
                            break;
                        case "d":
                            duration = 86400U * mult;
                            break;
                        case "w":
                            duration = 604800U * mult;
                            break;
                        case "m":
                            duration = 2628000U * mult;
                            break;
                        case "y":
                            duration = 31536000U * mult;
                            break;
                        default:
                            UnturnedChat.Say(caller, "Unabled to ban player: Invalid ban time", Color.red);
                            return;
                    }
                }
                //System.Console.WriteLine("point 0");
                DatabaseManager.Ban ban = GlobalBan.Instance.Database.GetBan(command[0]);
                //System.Console.WriteLine("point 1");
                if (ban == null)
                {
                    UnturnedChat.Say(caller, "Player not found, try different name or steamID", Color.red);
                    return;
                }
                //System.Console.WriteLine("point 2");
                GlobalBan.Instance.Database.BanPlayer(ban.Player, ban.SteamID, caller.DisplayName, reason, duration);//0=forever
                //System.Console.WriteLine("point 3");
                if (PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
                    Provider.kick(targetPlayer.playerID.steamID, reason);
                //System.Console.WriteLine("point 4");
                UnturnedChat.Say($"{ban.Player} was banned for {reason}", Color.magenta);
                //Embed embed = new Embed
                //{
                //    fields = new Field[]
                //    {
                //        new Field("**Player**", ban.Player, true),
                //        new Field("**\t\t\tSteamID**", ban.SteamID, true),
                //        new Field("**Reason**", reason, true),
                //        new Field("**Duration**", duration == 0U ? "Permanent" : $"{duration} sec.\ntill: {System.DateTime.UtcNow.AddSeconds(duration)} UTC", true),
                //        new Field("**Admin**", caller.DisplayName, true),
                //        new Field("**Map**", $"\t{Provider.map}", true),
                //    },
                //    color = new System.Random().Next(16000000)
                //};
                //GlobalBan.Instance.SendInDiscord(embed, "Ban");
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
