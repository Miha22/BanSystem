//using Rocket.API;
//using Rocket.Unturned.Chat;
//using SDG.Unturned;
//using Steamworks;
//using System.Collections.Generic;
//using UnityEngine;

//namespace fr34kyn01535.GlobalBan
//{
//    public class CommandExit : IRocketCommand
//    {
//        public AllowedCaller AllowedCaller => AllowedCaller.Both;

//        public string Name => "ss";

//        public string Help => "help";

//        public string Syntax => "/ss";

//        public List<string> Aliases => new List<string>();

//        public List<string> Permissions => new List<string>() { "rocket.ss" };

//        public void Execute(IRocketPlayer caller, string[] command)
//        {
//            Provider.onServerShutdown();
//            Application.Quit();
//        }
//    }
//    public class CommandSlay : IRocketCommand
//    {
//        public string Help
//        {
//            get { return  "Banns a player for a year"; }
//        }

//        public string Name
//        {
//            get { return "slay"; }
//        }

//        public string Syntax
//        {
//            get { return "<player>"; }
//        }

//        public List<string> Aliases
//        {
//            get { return new List<string>(); }
//        }

//        public AllowedCaller AllowedCaller
//        {
//            get { return AllowedCaller.Both; }
//        }

//        public List<string> Permissions
//        {
//            get
//            {
//                return new List<string>() { "globalban.slay" };
//            }
//        }

//        public void Execute(IRocketPlayer caller, params string[] command)
//        {
//            if (command.Length == 0 || command.Length > 2)
//            {
//                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_invalid_parameter"));
//                return;
//            }

//            //bool isOnline = false;
//            //CSteamID steamid;
//            //string charactername = null;
//            if (PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
//            {
//                SteamGameServerNetworking.GetP2PSessionState(targetPlayer.playerID.steamID, out P2PSessionState_t pConnectionState);
//                string ip = "ip: ";
//                ip += Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
//                byte[] hd = targetPlayer.playerID.hwid;
//                string hwid = "";
//                foreach (var item in hd)
//                    hwid += item.ToString() + '.';
//                //hwid = hwid.Substring(0, hwid.Length - 1);
//                System.Console.WriteLine($"banned hwid: {hwid}");
//                System.Console.WriteLine($"banned ip: {ip}");

//            }
//            else
//            {
//                UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
//                return;
//            }


//            //if (command.Length >= 2)
//            //{
//            //    GlobalBan.Instance.Database.BanPlayer(targetPlayer.playerID.characterName, targetPlayer.playerID.steamID.ToString(), ip, hwid, "server", command[1], 31536000);
//            //    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public_reason", targetPlayer.playerID.characterName, command[1]));
//            //    Provider.kick(targetPlayer.playerID.steamID, command[1]);
//            //}
//            //else
//            //{
//            //    GlobalBan.Instance.Database.BanPlayer(targetPlayer.playerID.characterName, targetPlayer.playerID.steamID.ToString(), ip, hwid, "server", command[1], 31536000);
//            //    UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_public", targetPlayer.playerID.characterName));
//            //    Provider.kick(targetPlayer.playerID.steamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
//            //}
//        }
//    }
//}
