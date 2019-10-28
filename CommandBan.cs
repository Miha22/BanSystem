using Rocket.API;
using Rocket.Core.Logging;
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
        public string Help
        {
            get { return "Bans player, use /help to find syntax"; }
        }

        public string Name
        {
            get { return "ban"; }
        }

        public string Syntax
        {
            get { return "/ban [player] \n/ban [player] [reason]\n/ban [player] [reason] [duration]"; }
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
                return new List<string>() { "bansystem.ban" };
            }
        }
        //curl http://v2.api.iphub.info/ip/194.47.41.154 -H "X-Key: NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ=="
        //curl http://v2.api.iphub.info/ip/81.92.200.219 -H "X-Key: NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ=="
        //internal static CommandBan Instance;

        //public CommandBan()
        //{
        //    Instance = this;
        //}

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
                //string discord = commandOld[0];
                //string[] command = new string[commandOld.Length - 1];
                //for (byte i = 1; i < commandOld.Length; i++)
                //    command[i - 1] = commandOld[i];

                //bool isOnline = false;

                //CSteamID steamid;
                //string charactername = null;


                SteamGameServerNetworking.GetP2PSessionState(targetPlayer.playerID.steamID, out P2PSessionState_t pConnectionState);
                string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                string hwid = GlobalBan.Instance.GetHWidString(targetPlayer.playerID.hwid);

                //System.Console.WriteLine($"banned hwid: {hwid}");
                //System.Console.WriteLine($"banned ip: {ip}");
                //System.Console.WriteLine($"pConnectionState.m_nRemoteIP: {pConnectionState.m_nRemoteIP}");

                //UnturnedPlayer otherPlayer = UnturnedPlayer.FromName(command[0]);
                //ulong? otherPlayerID = command.GetCSteamIDParameter(0);
                //if (otherPlayer == null || otherPlayer.CSteamID.ToString() == "0" || caller != null && otherPlayer.CSteamID.ToString() == caller.Id)
                //{
                //    KeyValuePair<CSteamID, string> player = GlobalBan.GetPlayer(command[0]);
                //    if (player.Key.ToString() != "0")
                //    {
                //        steamid = player.Key;
                //        charactername = player.Value;
                //    }
                //    else
                //    {
                //        if (otherPlayerID != null)
                //        {
                //            steamid = new CSteamID(otherPlayerID.Value);
                //            Profile playerProfile = new Profile(otherPlayerID.Value);
                //            charactername = playerProfile.SteamID;
                //        }
                //        else
                //        {
                //            UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                //            return;
                //        }
                //    }
                //}
                //else
                //{
                //    isOnline = true;
                //    steamid = otherPlayer.CSteamID;
                //    charactername = otherPlayer.CharacterName;
                //}


                string adminName = caller == null ? "Console" : caller.DisplayName;
                uint duration = 0;

                if (command.Length == 3 && !uint.TryParse(command[2], out duration))
                {
                    switch (command[2].ToLower())
                    {
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
                            UnturnedChat.Say(caller, "Unrecognized provided ban time");
                            return;
                    }
                }
                GlobalBan.Instance.BanDisconnect(targetPlayer.playerID.characterName.ToLower(), targetPlayer.playerID.steamID, ip, hwid, true, adminName, command.Length == 1 ? "" : command[1], command.Length == 1 || command.Length == 2 ? 0U : duration);
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
