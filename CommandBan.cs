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
    //public class Command : IRocketCommand
    //{
    //    public AllowedCaller AllowedCaller => AllowedCaller.Both;

    //    public string Name => "gg";

    //    public string Help => "";

    //    public string Syntax => "";

    //    public List<string> Aliases => new List<string>();

    //    public List<string> Permissions => new List<string>();

    //    public void Execute(IRocketPlayer caller, string[] command)
    //    {
    //        System.Console.WriteLine(caller);
    //        System.Console.WriteLine(caller.Id);
    //        System.Console.WriteLine(caller.DisplayName);
    //        System.Console.WriteLine(caller.IsAdmin);
    //        Rocket.API.ConsolePlayer
    //        Console
    //        Console
    //        True
    //    }
    //}

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
                //GlobalBan.Instance.BanDisconnect(targetPlayer.playerID.characterName, targetPlayer.playerID.steamID, ip, hwid, true, caller.DisplayName, command.Length == 1 ? "" : command[1], command.Length == 1 || command.Length == 2 ? 0U : duration);
                
                GlobalBan.Instance.Database.BanPlayer(targetPlayer.playerID.characterName.ToLower(), targetPlayer.playerID.steamID.ToString(), ip, hwid, caller.DisplayName, reason, duration);//0=forever
                Provider.kick(targetPlayer.playerID.steamID, reason);
                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", targetPlayer.playerID.characterName, reason));
                Embed embed = new Embed()
                {
                    fields = new Field[]
                    {
                        new Field("**Player**", targetPlayer.playerID.characterName, true),
                        new Field("**\t\t\tSteamID**", targetPlayer.playerID.steamID.ToString(), true),
                        new Field("**Reason**", reason, true),
                        new Field("**Duration**", duration == 0U ? "Permanent" : $"{duration} sec.\ntill: {System.DateTime.Now.AddSeconds(duration).ToUniversalTime()}", true),
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
