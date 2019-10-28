//using DiscordTutorialBot.Core;
using Newtonsoft.Json;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Permissions;
using SDG.Unturned;
using Steamworks;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned;
using Rocket.Unturned.Player;

namespace BanSystem
{
    //enum OS
    //{
    //    Windows,
    //    Mac,
    //    Linux
    //}
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        internal static GlobalBan Instance;
        //private File file;
        //private Process serverProcess;
        //private int _botProcessID;
        internal DatabaseManager Database;

        //public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();

        protected override void Load()
        {
            //using (FileStream file = new FileStream($@"/Plugins/{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}/DiscordFunText.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{

            //}
            Instance = this;
            Database = new DatabaseManager();
            if (DateTime.Now.Ticks > 637262027366394042)
            {
                UnloadPlugin();
                Instance = null;
                Database = null;
                Logger.LogError("Plugin is outdated!");
            }
                

            
            //serverProcess = new Process();
            //Console.WriteLine($"server save: {}");
            if (Configuration.Instance.API_Key == "" || !Configuration.Instance.IPcheck_Enabled)
                Logger.LogWarning("[WARNING] VPN/Proxy protection is DISABLED, check your config for correct API!");
            if (Configuration.Instance.Webhook == "" || !Configuration.Instance.WebHook_Enabled)
                Logger.LogWarning("[WARNING] WebHook reports are DISABLED, check your config for correct API!");
            //Arguments = $@"/c dotnet E:\Users\Deniel\Source\Repos\SocketPractiseServer\SocketPractiseServer\bin\Debug\netcoreapp2.1\SocketPractiseServer.dll"
            //if (Configuration.Instance.Bot_Token != "" && Configuration.Instance.Bot_Enabled)
            //{
            //    //run bot if sucess returned continue, if not UnloadPlugin()
            //    OS os = GetOS();
            //    ProcessStartInfo startInfo = new ProcessStartInfo();
            //    if (os == OS.Windows)
            //    {
            //        startInfo.FileName = "cmd";
            //        startInfo.Arguments = @"/c dotnet /Libraries/DiscordBot.dll";
            //        startInfo.WindowStyle = ProcessWindowStyle.Normal;
            //    }
            //    else if (os == OS.Linux)
            //    {
            //        startInfo.FileName = "/bin/bash";
            //        startInfo.Arguments = "-c \" " + "gnome - terminal - x bash - ic 'dotnet $./Libraries/DiscordBot.dll; bash'" + " \"";
            //        //proc.StartInfo.Arguments = "-c \" " + "gnome - terminal - x bash - ic 'cd $HOME; ls; bash'" + " \"";
            //        startInfo.UseShellExecute = false;
            //        startInfo.RedirectStandardOutput = true;

            //        //while (!proc.StandardOutput.EndOfStream)
            //        //{
            //        //    Console.WriteLine(proc.StandardOutput.ReadLine());
            //        //}
            //    }
            //    else
            //    {
            //        startInfo.FileName = "dotnet";
            //        startInfo.Arguments = @"/Libraries/DiscordBot.dll";
            //        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //    }

            //    serverProcess.StartInfo = startInfo;
            //    serverProcess.Start();
            //    _botProcessID = serverProcess.Id;
            //}
            //else
            //{
            //    Rocket.Core.Logging.Logger.LogWarning("[WARNING] Discord bot is DISABLED, check your config for correct token!");
            //    Configuration.Instance.Bot_Enabled = false;
            //}

            
            UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
            Logger.Log($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} loaded!", ConsoleColor.Cyan);
        }

        private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            Console.WriteLine("REJECTED ON SECOND LAYER");
            if (Database.IsBanned(player))
                Provider.kick(player.CSteamID, "You are banned");
        }

        protected override void Unload()
        {
            //Process.GetProcessById(_botProcessID).CloseMainWindow();
            UnturnedPermissions.OnJoinRequested -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
            //Rocket.Core.Logging.Logger.Log($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} by M22 loaded!", ConsoleColor.Cyan);
        }

        private void SendInDiscord(string player, string steamid, string reason, uint duration, string admin, string map)
        {
            Embed embed = new Embed()
            {
                fields = new Field[]
                {
                    new Field("**Player**", $"{player}", true),
                    new Field("**\t\t\tSteamID**", $"{steamid}", true),
                    new Field("**Reason**", $"{reason}", true),
                    new Field("**Duration**", duration == 0U ? "Permanent" : $"{duration} sec.\ntill: {DateTime.Now.AddSeconds(duration).ToUniversalTime()}", true),
                    new Field("**Admin**", $"{admin}", true),
                    new Field("**Map**", $"\t{map}", true),
                },
                color = new Random().Next(16000000),
            };
            try
            {
                SendMessageAsync(new DiscordWebhookMessage() { username = "Ban", embeds = new Embed[] { embed } }, Configuration.Instance.Webhook);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
        }

        private string RandomText()
        {
            throw new NotImplementedException();
        }

        private void SendMessageAsync(DiscordWebhookMessage msg, string url)
        {
            string json = JsonConvert.SerializeObject(msg);
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Set(HttpRequestHeader.ContentType, "application/json");
                wc.UploadString(url, json);
            }
        }

        //private void LaunchBotAsync()
        //{
        //    if(Configuration.Instance.Bot_Token == "")
        //    {
        //        Rocket.Core.Logging.Logger.LogError("There is no bot token in config! Disabling discord reports...");
        //        return;
        //    }
        //    Discord = new DiscordManager();
        //    _client = new DiscordSocketClient(new DiscordSocketConfig
        //    {
        //        LogLevel = LogSeverity.Verbose
        //    });
        //    _client.Log += Log;
        //    _client.LoginAsync(TokenType.Bot, Configuration.Instance.Bot_Token);
        //    _client.StartAsync();
        //    _handler = new CommandHandler();
        //    _handler.InitializeAsync(_client);
        //}

        //private async Task Log(LogMessage msg)
        //{
        //    Rocket.Core.Logging.Logger.Log("Discord bot: " + msg.Message);
        //}


        //private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        //{
        //    SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
        //    string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);

        //    //Console.WriteLine($"connected ip: {ip}");
        //    //Console.Write("connected hwid: ");
        //    //foreach (var item in player.Player.channel.owner.playerID.hwid)
        //    //{
        //    //    Console.Write($"{item}.");
        //    //}//player.CSteamID.ToString(), ip,
        //    if (Database.IsBanned(GetHWidString(player.Player.channel.owner.playerID.hwid)))
        //    {
        //        Provider.kick(player.CSteamID, "");
        //        return;
        //    }
        //    //Console.WriteLine("CONNECTED PLAYER IS NOT BANNED");
        //    GetResponse(player.Player.channel.owner.playerID.characterName, player.Player.channel.owner.playerID.steamID, GetHWidString(player.Player.channel.owner.playerID.hwid), ip);
        //}

        private async Task<bool> IsBadIP(CSteamID steamID)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                SteamGameServerNetworking.GetP2PSessionState(steamID, out P2PSessionState_t pConnectionState);
                string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"http://v2.api.iphub.info/ip/{ip}"))
                {
                    request.Headers.TryAddWithoutValidation("X-Key", $"{Instance.Configuration.Instance.API_Key}");
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string json = await response.Content.ReadAsStringAsync();
                    ConnectedIP client = JsonConvert.DeserializeObject<ConnectedIP>(json);
                    string[] str = client.ip.Split('.');
                    byte.TryParse(str[0], out byte num1);
                    byte.TryParse(str[1], out byte num2);

                    return client.block != 0 || client.countryCode == "ZZ" || IsPrivateIP(num1, num2);
                }
            }
        }

        //private async void GetResponse(string player, CSteamID steamID, string hwid, string ip)
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"http://v2.api.iphub.info/ip/{ip}"))
        //        {
        //            //request.Headers.TryAddWithoutValidation("X-Key", "NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ==");
        //            request.Headers.TryAddWithoutValidation("X-Key", $"{Instance.Configuration.Instance.API_Key}");
        //            HttpResponseMessage response = await httpClient.SendAsync(request);
        //            string json = await response.Content.ReadAsStringAsync();
        //            ConnectedIP client = JsonConvert.DeserializeObject<ConnectedIP>(json);
        //            //Console.WriteLine(json);
        //            //string[] str = json.Split(':');
        //            //string countryCode = str[2].Substring(1, 2);
        //            //string block = str[str.Length - 2].Substring(0, 1);
        //            //Console.WriteLine();
        //            //Console.WriteLine($"client.block: {client.block}, client.ip: {client.ip}");
        //            //string[] str = client.ip.Split('.');
        //            //byte.TryParse(str[0], out byte num1);
        //            //byte.TryParse(str[1], out byte num2);
        //            // || IsPrivateIP(num1, num2) || client.countryCode == "ZZ"
        //            if (client.block != 0)
        //            {
        //                BanDisconnect(player, steamID, ip, hwid, false, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}", $"VPN/Proxy)", 0);
        //                //GlobalBan.Instance.Database.BanPlayer(player.Player.channel.owner.playerID.characterName, player.CSteamID.ToString(), ip, hwid, "Server", "", 0);//0=forever
        //                //UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_private", player.Player.channel.owner.playerID.characterName));
        //                //Provider.kick(player.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
        //                Console.WriteLine();
        //                Console.WriteLine($"Player had Proxy or private IP, block: {client.block}, country code: {client.countryCode}");
        //            }
        //        }
        //    }
        //}

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
                    return number2 >= 16 && number2 < 32;
                case 192:
                    return number2 == 168;
                case 169:
                    return number2 == 254;
                default:
                    return false;
            }
        }

        internal string GetHWidString(byte[] hardwareid)
        {
            string hwid = "";
            foreach (byte item in hardwareid)
                hwid += item.ToString() + '.';
            hwid = hwid.Substring(0, hwid.Length - 1);

            return hwid;
        }

        internal void BanDisconnect(string player, CSteamID steamID, string ip, string hwid, bool publicsay, string admin, string reason, uint duration)
        {
            Instance.Database.BanPlayer(player, steamID.ToString(), ip, hwid, admin, reason, duration);//0=forever
            if (publicsay)
                UnturnedChat.Say(Instance.Translate("command_ban_public_reason", player, reason));
            Provider.kick(steamID, reason == "" ? "Permanent ban" : reason);
            SendInDiscord(player, steamID.ToString(), reason == "" ? "N/A" : reason, duration, admin, Provider.map);
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

        private void Events_OnJoinRequested(CSteamID player, ref ESteamRejection? rejection)
        {
            try
            {
                if (IsBadIP(player).Result || Database.IsBanned(player))
                {
                    rejection = ESteamRejection.AUTH_PUB_BAN;
                    Logger.Log($"VPN Connection rejected");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        //[DllImport("libc")]
        //static extern int Uname(IntPtr buf);

        //private OS GetOS()
        //{
        //    IntPtr buf = IntPtr.Zero;
        //    try
        //    {
        //        buf = Marshal.AllocHGlobal(8192);
        //        // This is a hacktastic way of getting sysname from uname ()
        //        if (Uname(buf) == 0)
        //        {
        //            string os = Marshal.PtrToStringAnsi(buf);
        //            if (os == "Darwin")
        //                return OS.Mac;
        //            if (os == "Linux")
        //                return OS.Linux;
        //            if (Path.DirectorySeparatorChar == '\\')
        //                return OS.Windows;
        //        }
        //    }
        //    catch(Exception e)
        //    {
        //        Console.WriteLine(e.Message);
        //    }
        //    finally
        //    {
        //        if (buf != IntPtr.Zero)
        //            Marshal.FreeHGlobal(buf);
        //    }

        //    throw new Exception("Unable to define Operating System");
        //}

        //private bool IsWin() 
        //{
        //    return Path.DirectorySeparatorChar == '\\';
        //}
    }
}
