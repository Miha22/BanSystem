using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace BanSystem
{
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
                if (!PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"), Color.red);
                    return;
                }
                SteamGameServerNetworking.GetP2PSessionState(targetPlayer.playerID.steamID, out P2PSessionState_t pConnectionState);
                string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                string hwid = GlobalBan.Instance.GetHWidString(targetPlayer.playerID.hwid);
                string reason = command.Length == 1 ? "N/A" : command[1];
                uint duration = 0U;

                if (command.Length == 3 && !uint.TryParse(command[2], out duration))
                {
                    switch (command[2].ToLower())
                    {
                        case "hour":
                            duration = 3600;
                            break;
                        case "day":
                            duration = 86400;
                            break;
                        case "week":
                            duration = 604800;
                            break;
                        case "month":
                            duration = 2628000;
                            break;
                        case "year":
                            duration = 31536000;
                            break;
                        default:
                            UnturnedChat.Say(caller, "Unabled to ban player: Invalid ban time", Color.red);
                            return;
                    }
                }
                GlobalBan.Instance.Database.BanPlayer(targetPlayer.playerID.characterName.ToLower(), targetPlayer.playerID.steamID.ToString(), caller.DisplayName, reason, duration);//0=forever
                Provider.kick(targetPlayer.playerID.steamID, reason);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", targetPlayer.playerID.characterName, reason));
                Embed embed = new Embed()
                {
                    fields = new Field[]
                    {
                        new Field("**Player**", targetPlayer.playerID.characterName, true),
                        new Field("**\t\t\tSteamID**", targetPlayer.playerID.steamID.ToString(), true),
                        new Field("**Reason**", reason, true),
                        new Field("**Duration**", duration == 0U ? "Permanent" : $"{duration} sec.\ntill: {System.DateTime.UtcNow.AddSeconds(duration)} UTC", true),
                        new Field("**Admin**", caller.DisplayName, true),
                        new Field("**Map**", $"\t{Provider.map}", true),
                    },
                    color = new System.Random().Next(16000000)
                };
                GlobalBan.Instance.SendInDiscord(embed, "Ban");
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
