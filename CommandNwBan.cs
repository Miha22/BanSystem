﻿using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace BanSystem
{
    public class CommandNWBan : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "nwban";
        public string Help => "Bans player on all server, use /help to find syntax";
        public string Syntax => "/nwban [player] \n/ban [player] [reason]\n/nwban [player] [reason] [duration]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "bansystem.nwban" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            try
            {
                if (command.Length == 0 || command.Length > 3)
                {
                    UnturnedChat.Say(caller, GlobalBan.Instance.Translate("invalid_command", Syntax), Color.red, true);
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
                    if (!uint.TryParse(command[2].Substring(0, 1), out uint mult))
                    {
                        UnturnedChat.Say(caller, "Unabled to ban player: Invalid ban time", Color.red, true);
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
                            UnturnedChat.Say(caller, "Unabled to ban player: Invalid ban time", Color.red, true);
                            return;
                    }
                }
                //System.Console.WriteLine("point 0");
                DatabaseManager.PlayerInfo playerInfo = GlobalBan.Instance.DatabaseManager.GetBan(command[0], true);
                //System.Console.WriteLine("point 1");
                if (playerInfo == null)
                {
                    UnturnedChat.Say(caller, $"{command[0]} was not found in global database, try different name or steamID", Color.red, true);
                    return;
                }
                //System.Console.WriteLine("point 2");
                GlobalBan.Instance.DatabaseManager.UnbanPlayer(playerInfo.steamid, false);
                GlobalBan.Instance.DatabaseManager.BanPlayer(playerInfo.Charactername, playerInfo.steamid, caller.DisplayName, reason, duration, true);//0=forever
                //System.Console.WriteLine("point 3");
                if (PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
                    Provider.kick(targetPlayer.playerID.steamID, GlobalBan.Instance.Translate("ban_private", reason, caller.DisplayName));
                //System.Console.WriteLine("point 4");
                UnturnedChat.Say(GlobalBan.Instance.Translate("ban_public", playerInfo.Charactername, reason, caller.DisplayName), GlobalBan.Instance.GetColor(), true);
                Embed embed = new Embed
                {
                    fields = new Field[]
                    {
                        new Field("**Server**", $"{GlobalBan.ServerName ?? "N/A"}", false),
                        new Field("**Player**", playerInfo.Charactername, true),
                        new Field("**SteamID**", playerInfo.steamid, true),
                        new Field("**Reason**", reason, true),
                        new Field("**Duration**", duration == 0U ? "Permanent" : $"{duration} sec.\ntill: {System.DateTime.UtcNow.AddSeconds(duration)} UTC", true),
                        new Field("**Admin**", caller.DisplayName, true),
                        new Field("**Server**", $"\tAll", true),
                        new Field("**Map**", $"\t{Provider.map}", true)
                    },
                    //color = new System.Random().Next(16000000)discord_bot_ban_color
                    color = int.Parse(GlobalBan.Instance.Translate("discord_bot_ban_color"))
                };
                GlobalBan.Instance.SendInDiscord(embed, GlobalBan.Instance.Translate("discord_bot_globalban_name"));
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
