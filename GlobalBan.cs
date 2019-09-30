using Discord.WebSocket;
using Discord;
//using DiscordTutorialBot.Core;
using Newtonsoft.Json;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Permissions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace fr34kyn01535.GlobalBan
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        internal static GlobalBan Instance;
        internal DatabaseManager Database;
        internal DiscordManager Discord;
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        //public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            if (Configuration.Instance.API_Key == "")
            {
                Rocket.Core.Logging.Logger.LogError("There is no API key in config! Disabling VPN/Proxy protection... Add your token in config to get full protection");
                Configuration.Instance.VPN_Proxy_Protection = false;
            }
            LaunchBotAsync();
            Instance = this;
            Database = new DatabaseManager();
            UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
            Rocket.Core.Logging.Logger.Log($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} by M22 loaded!", ConsoleColor.Cyan);
        }


        protected override void Unload()
        {
            UnturnedPermissions.OnJoinRequested -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
            //Rocket.Core.Logging.Logger.Log($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} by M22 loaded!", ConsoleColor.Cyan);
        }

        private void LaunchBotAsync()
        {
            if(Configuration.Instance.Bot_Token == "")
            {
                Rocket.Core.Logging.Logger.LogError("There is no bot token in config! Disabling discord reports...");
                return;
            }
            Discord = new DiscordManager();
            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            _client.Log += Log;
            _client.LoginAsync(TokenType.Bot, Configuration.Instance.Bot_Token);
            _client.StartAsync();
            _handler = new CommandHandler();
            _handler.InitializeAsync(_client);
        }

        private async Task Log(LogMessage msg)
        {
            Rocket.Core.Logging.Logger.Log("Discord bot: " + msg.Message);
        }


        private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
            string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);

            Console.WriteLine($"connected ip: {ip}");
            Console.Write("connected hwid: ");
            foreach (var item in player.Player.channel.owner.playerID.hwid)
            {
                Console.Write($"{item}.");
            }//player.CSteamID.ToString(), ip, 
            if (Database.IsBanned(GetHWidString(player.Player.channel.owner.playerID.hwid)))
            {
                Provider.kick(player.CSteamID, "");
                return;
            }
            //Console.WriteLine("CONNECTED PLAYER IS NOT BANNED");
            GetResponse(player.Player.channel.owner.playerID.characterName, player.Player.channel.owner.playerID.steamID, GetHWidString(player.Player.channel.owner.playerID.hwid), ip);
        }

        private async void GetResponse(string player, CSteamID steamID, string hwid, string ip)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"http://v2.api.iphub.info/ip/{ip}"))
                {
                    //request.Headers.TryAddWithoutValidation("X-Key", "NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ==");
                    request.Headers.TryAddWithoutValidation("X-Key", $"{Instance.Configuration.Instance.API_Key}");
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string json = await response.Content.ReadAsStringAsync();
                    Client client = JsonConvert.DeserializeObject<Client>(json);
                    Console.WriteLine(json);
                    //string[] str = json.Split(':');
                    //string countryCode = str[2].Substring(1, 2);
                    //string block = str[str.Length - 2].Substring(0, 1);
                    Console.WriteLine();
                    Console.WriteLine($"client.block: {client.block}, client.ip: {client.ip}");
                    //string[] str = client.ip.Split('.');
                    //byte.TryParse(str[0], out byte num1);
                    //byte.TryParse(str[1], out byte num2);
                    // || IsPrivateIP(num1, num2) || client.countryCode == "ZZ"
                    if (client.block != 0)
                    {
                        BanDisconnect(player, steamID, ip, hwid, false, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}", $"VPN/Proxy)", 0);
                        //GlobalBan.Instance.Database.BanPlayer(player.Player.channel.owner.playerID.characterName, player.CSteamID.ToString(), ip, hwid, "Server", "", 0);//0=forever
                        //UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_private", player.Player.channel.owner.playerID.characterName));
                        //Provider.kick(player.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
                        Console.WriteLine();
                        Console.WriteLine($"Player had Proxy or private IP, block: {client.block}, country code: {client.countryCode}");
                    }
                }
            }
        }

        internal bool IsPrivateIP(byte number1, byte number2)
        {
            //$pri_addrs = array(
            // '10.0.0.0|10.255.255.255', // single class A network
            // '172.16.0.0|172.31.255.255', // 16 contiguous class B network
            // '192.168.0.0|192.168.255.255', // 256 contiguous class C network
            // '169.254.0.0|169.254.255.255', // Link-local address also refered to as Automatic Private IP Addressing
            // '127.0.0.0|127.255.255.255' // localhost
            //);

            switch (number1)
            {
                case 10:
                    return true;
                case 127:
                    return true;
                case 172:
                    if (number2 >= 16 && number2 < 32)
                        return true;
                    return false;
                case 192:
                    if (number2 == 168)
                        return true;
                    return false;
                case 169:
                    if (number2 == 254)
                        return true;
                    return false;
                default:
                    return false;
            }
        }

        internal string GetHWidString(byte[] hardwareid)
        {
            string hwid = "";
            foreach (var item in hardwareid)
                hwid += item.ToString() + '.';
            hwid = hwid.Substring(0, hwid.Length - 1);

            return hwid;
        }

        internal void BanDisconnect(string player, CSteamID steamID, string ip, string hwid, bool publicsay, string admin, string reason = "", uint duration = 0U)
        {
            Instance.Database.BanPlayer(player, steamID.ToString(), ip, hwid, admin, reason, duration);//0=forever
            if (publicsay && reason != "")
            {
                UnturnedChat.Say(Instance.Translate("command_ban_public_reason", player, reason));
            }
            else if (publicsay)
            {
                UnturnedChat.Say(Instance.Translate("command_ban_public", player));

            }
            Instance.Discord.SendChannelBanMessage(player, admin, reason, duration == 0U ? "forever" : Convert.ToString(duration));

            Provider.kick(steamID, reason != "" ? reason : Instance.Translate("command_ban_private_default_reason"));
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList() {
                    {"default_banmessage","you were banned by {0} on {1} for {2} seconds, contact the staff if you feel this is a mistake."},
                    {"command_generic_invalid_parameter","Invalid parameter"},
                    {"command_generic_player_not_found","Player not found"},
                    {"command_ban_public_reason", "The player {0} was banned for: {1}"},
                    {"command_ban_public","The player {0} was banned"},
                    {"command_ban_private_default_reason","you were banned from the server"},
                    {"command_kick_public_reason", "The player {0} was kicked for: {1}"},
                    {"command_kick_public","The player {0} was kicked"},
                    {"command_kick_private_default_reason","you were kicked from the server"},
                };
            }
        }

        //public static KeyValuePair<CSteamID, string> GetPlayer(string search)
        //{
        //    foreach (KeyValuePair<CSteamID, string> pair in Players)
        //    {
        //        if (pair.Key.ToString().ToLower().Contains(search.ToLower()) || pair.Value.ToLower().Contains(search.ToLower()))
        //        {
        //            return pair;
        //        }
        //    }
        //    return new KeyValuePair<CSteamID, string>(new CSteamID(0), null);
        //}

        //void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        //{
        //    if (!Players.ContainsKey(player.CSteamID))
        //        Players.Add(player.CSteamID, player.CharacterName);
            
        //    if (Configuration.Instance.KickInsteadReject)
        //    {
        //        DatabaseManager.Ban ban = Database.GetBan(player.Id);
        //        if(ban != null && (ban.Duration == -1 || ban.Time.AddSeconds(ban.Duration) > DateTime.Now))
        //            StartCoroutine(KickPlayer(player,ban));
        //    }
        //}
        //IEnumerator KickPlayer(UnturnedPlayer player,DatabaseManager.Ban ban)
        //{
        //    yield return new WaitForSeconds(Instance.Configuration.Instance.KickInterval);
        //    player.Kick(Translate("default_banmessage",ban.Admin,ban.Time.ToString(),ban.Duration == -1 ? "" : ban.Duration.ToString()));
        //}

        public void Events_OnJoinRequested(CSteamID player, ref ESteamRejection? rejection)
        {
            try
            {
                if (Database.IsBanned(player))
                {
                    rejection = ESteamRejection.AUTH_PUB_BAN;
                    //Console.WriteLine($"Tried to join: {player}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
