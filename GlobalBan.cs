﻿//using DiscordTutorialBot.Core;
using Newtonsoft.Json;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SDG.Unturned;
using Steamworks;
using System;
using Logger = Rocket.Core.Logging.Logger;
using Rocket.Unturned;
using Rocket.Unturned.Player;
using System.Net;
using System.IO;
using UnityEngine;

namespace BanSystem
{
    public class GlobalBan : RocketPlugin<GlobalBanConfiguration>
    {
        internal static GlobalBan Instance;
        //private File file;
        //private Process serverProcess;
        //private int _botProcessID;
        internal DatabaseManager DatabaseManager;
        internal int UTCoffset { get; private set; }
        internal static string ServerName;

        //public static Dictionary<CSteamID, string> Players = new Dictionary<CSteamID, string>();
        private string GetServerName()
        {
            try
            {
                using StreamReader sr = new StreamReader(new FileStream(@"../Server/Commands.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                //string[] data = sr.ReadToEnd().Split(' ');
                while (!sr.EndOfStream)
                {
                    string row = sr.ReadLine();
                    string[] data = row.Split(' ');
                    if(data[0].ToLower() == "name")
                        return row.Substring(data[0].Length + 1);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected override void Load()
        {
            Instance = this;
            UTCoffset = (int)System.Math.Ceiling((DateTime.Now - DateTime.UtcNow).TotalHours);
            DatabaseManager = new DatabaseManager();
            ServerName = GetServerName();
            //serverProcess = new Process();
            //Console.WriteLine($"server save: {}");
            if (Configuration.Instance.API_Key == "")
                Logger.LogWarning("[WARNING] VPN/Proxy protection is DISABLED, check your config for correct API!");
            if (Configuration.Instance.Webhook == "")
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


            //UnturnedPermissions.OnJoinRequested += Events_OnJoinRequested;
            U.Events.OnPlayerConnected += RocketServerEvents_OnPlayerConnected;
            Logger.Log($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} loaded!", ConsoleColor.Cyan);
            //UnturnedPermissions.OnJoinRequested += UnturnedPermissions_OnJoinRequested;
        }

        //private void UnturnedPermissions_OnJoinRequested(CSteamID player, ref ESteamRejection? rejectionReason)
        //{
        //    rejectionReason = ESteamRejection.
        //}

        private void Reject(CSteamID steamID, string reason)
        {
            for (int index = 0; index < Provider.pending.Count; ++index)
            {
                if (Provider.pending[index].playerID.steamID == steamID)
                {
                    if (Provider.pending[index].inventoryResult != SteamInventoryResult_t.Invalid)
                    {
                        SteamGameServerInventory.DestroyResult(Provider.pending[index].inventoryResult);
                        Provider.pending[index].inventoryResult = SteamInventoryResult_t.Invalid;
                    }
                    Provider.pending.RemoveAt(index);
                    break;
                }
            }
            byte[] bytes = SteamPacker.getBytes(0, out int size, (object)(byte)10, (object)reason);
            Provider.send(steamID, ESteamPacket.DISCONNECTED, bytes, size, 0);
            SteamGameServer.EndAuthSession(steamID);
        }


        private void RocketServerEvents_OnPlayerConnected(UnturnedPlayer player)
        {
            SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
            //Console.WriteLine("REJECTED ON SECOND LAYER");
            //if (Instance.Configuration.Instance.ShowConnectInfo)
            //{
            //    SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
            //    string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
            //    Logger.LogWarning($"Trying to connect: \nName: {player.CharacterName} \nSteamID{player.CSteamID} \nIP: {ip} \n HWID: {GetHWidString(player.Player.channel.owner.playerID.hwid)}");
            //}
            try
            {
                if (DatabaseManager.IsWhite(player.CSteamID.ToString()))
                    return;
                if (IsBadIP(player))
                {
                    //Provider.kick(player.CSteamID, Translate("join_vpn_detected"));
                    //Provider.clients[0].
                    Reject(player.CSteamID, Translate("join_vpn_detected"));
                    return;
                }
                if (DatabaseManager.IsBanned(player, out DateTime date, out string reason, out bool global))
                {
                    Reject(player.CSteamID, $"{(date == DateTime.MaxValue ? (global ? Translate("join_global_ban_message_permanent", reason) : Translate("join_server_ban_message_permanent", reason)) : global ? Translate("join_global_ban_message", date.AddHours(-UTCoffset).ToString("dddd, dd MMMM yyyy HH:mm:ss"), reason) : Translate("join_server_ban_message", date.AddHours(-UTCoffset).ToString("dddd, dd MMMM yyyy HH:mm:ss"), reason))}");
                    return;
                }

                //if (Database.IsBanned(player, out DateTime date2, false, out string reason2))
                //{
                //    //Provider.kick(player.CSteamID, $"{(date2 == DateTime.MaxValue ? Translate("join_server_ban_message_permanent", reason2) : Translate("join_server_ban_message", date2.AddHours(-UTCoffset), reason2))}");
                //    Reject(player.CSteamID, $"{(date2 == DateTime.MaxValue ? Translate("join_server_ban_message_permanent", reason2) : Translate("join_server_ban_message", date2.AddHours(-UTCoffset), reason2))}");
                //    return;
                //}
                    
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            

            //Provider.kick(player.CSteamID, $"{date == DateTime.MaxValue ? "You are permanently banned" : }"); date.AddHours(-UTCoffset).ToString()
        }

        protected override void Unload()
        {
            //Process.GetProcessById(_botProcessID).CloseMainWindow();
            //UnturnedPermissions.OnJoinRequested -= Events_OnJoinRequested;
            U.Events.OnPlayerConnected -= RocketServerEvents_OnPlayerConnected;
            //Rocket.Core.Logging.Logger.Log($"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name} by M22 loaded!", ConsoleColor.Cyan);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList {
                    {"invalid_command","Invalid command usage. Do like this: {0}"},
                    {"join_vpn_detected","Private connections like VPN are not allowed on this server!"},
                    {"join_global_ban_message_permanent","You are permanently banned on all servers for: {0}"},
                    {"join_global_ban_message","You are banned on all servers till: {0} for: {1}"},
                    {"join_server_ban_message_permanent","You are permanently banned on this server for: {0}"},
                    {"join_server_ban_message","You are banned on this server till: {0} for: {1}"},
                    {"ban_public", "{0} was banned for: {1} by {2}"},
                    {"ban_private","You were banned for: {0} by {1}"},
                    {"unban_public", "{0} was unbanned by: {1}"},
                    {"kick_public", "{0} was kicked for: {1} by {2}"},
                    {"kick_private","{0} by {1}"},
                    {"discord_bot_globalban_name","Global Ban"},
                    {"discord_bot_localban_name","Local Ban"},
                    {"discord_bot_kick_name","Kick"},
                    {"discord_bot_globalunban_name","Global Unban"},
                    {"discord_bot_localunban_name","Local Unban"},
                    {"discord_bot_ban_color","16711680"},
                    {"discord_bot_unban_color","65280"},
                    {"discord_bot_kick_color","3875327"}
                };
            }
        }

        internal void SendInDiscord(Embed embed, string username)
        {
            try
            {
                SendMessageAsync(new DiscordWebhookMessage() { username = username, embeds = new Embed[] { embed } }, Configuration.Instance.Webhook);
            }
            catch (Exception e)
            {
                Logger.LogException(e);
            }
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

        private bool IsBadIP(UnturnedPlayer player)
        {
            SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
            string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create($"http://v2.api.iphub.info/ip/{ip}");
            request.Headers.Add("X-Key", $"{Configuration.Instance.API_Key}");
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    ConnectedIP client = JsonConvert.DeserializeObject<ConnectedIP>(reader.ReadToEnd());
                    string[] str = client.ip.Split('.');
                    byte.TryParse(str[0], out byte num1);
                    byte.TryParse(str[1], out byte num2);
                    if (Instance.Configuration.Instance.ShowConnectInfo)
                    {
                        ConsoleColor def = Console.ForegroundColor;
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"\n| Trying to connect: \n| Name: {player.CharacterName} \n| SteamID: {player.CSteamID} \n| IP: {ip} \n| IP reputation: {(client.block == 0 ? "[Safe] Residential or business IP (i.e. safe IP)" : client.block == 1 ? "[Dangerous] Non-residential IP (hosting provider, proxy, etc.)" : "[Dangerous] Non-residential & residential IP")} \n| HWID: {GetHWidString(player.Player.channel.owner.playerID.hwid)} \n| Country: {client.countryName} \n| Internet Provider: {client.isp} \n| Autonomous System Number: {client.asn} \n");
                        Console.ForegroundColor = def;
                    }
                    //Console.WriteLine($"block: {client.block}");
                    //Console.WriteLine($"countryCode: {client.countryCode}");
                    //Console.WriteLine($"IsPrivateIP: {IsPrivateIP(num1, num2)}");
                    //return client.block != 0 || client.countryCode == "ZZ" || IsPrivateIP(num1, num2);
                    return client.block != 0;
                }
            }
        }

        public Color GetColor()
        {
            if(Configuration.Instance.BanChatColor == null)
                return Color.magenta;
            switch (Configuration.Instance.BanChatColor.ToLower())
            {
                case "red":
                    return Color.red;
                case "white":
                    return Color.white;
                case "yellow":
                    return Color.yellow;
                case "cyan":
                    return Color.cyan;
                case "magenta":
                    return Color.magenta;
                default:
                    return Color.green;
            }
        }

        //private void IsBadIP(CSteamID steamID)
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        SteamGameServerNetworking.GetP2PSessionState(steamID, out P2PSessionState_t pConnectionState);
        //        string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
        //        //Console.WriteLine($"ip: {ip}");
        //        //Console.WriteLine($"url: http://v2.api.iphub.info/ip/{ip}");
        //        Console.WriteLine("point 1");
        //        using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"http://v2.api.iphub.info/ip/{ip}"))
        //        {
        //            Console.WriteLine("point 2");
        //            request.Headers.TryAddWithoutValidation("X-Key", $"{Configuration.Instance.API_Key}");
        //            Console.WriteLine("point 3");
        //            //HttpResponseMessage response = httpClient.SendAsync(request).Result;
        //            Task<HttpResponseMessage> response = Task.Run(() => httpClient.SendAsync(request));
        //            response.Wait();
        //            Console.WriteLine("point 4");
        //            string json = response.Result.Content.ReadAsStringAsync().Result;
        //            Console.WriteLine("point 5");
        //            ConnectedIP client = JsonConvert.DeserializeObject<ConnectedIP>(json);
        //            Console.WriteLine("point 6");
        //            string[] str = client.ip.Split('.');
        //            byte.TryParse(str[0], out byte num1);
        //            byte.TryParse(str[1], out byte num2);
        //            Console.WriteLine($"block: {client.block}");
        //            Console.WriteLine($"countryCode: {client.countryCode}");
        //            Console.WriteLine($"IsPrivateIP: {IsPrivateIP(num1, num2)}");

        //            //res = client.block != 0 || client.countryCode == "ZZ" || IsPrivateIP(num1, num2);
        //        }
        //    }
        //    //using (var httpClient = new HttpClient())
        //    //{
        //    //    using (var request = new HttpRequestMessage(new HttpMethod("GET"), $"http://v2.api.iphub.info/ip/{}"))
        //    //    {
        //    //        request.Headers.TryAddWithoutValidation("X-Key", "NjYwNTpuVWZxQW1aNWZsSlhWNlhzTjJKUlMzOWdqejBQVEJSVg==");

        //    //        var response = httpClient.SendAsync(request).Result;
        //    //        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        //    //    }
        //    //}
        //}

        //private async void GetResponse(CSteamID steamID)
        //{
        //    SteamGameServerNetworking.GetP2PSessionState(steamID, out P2PSessionState_t pConnectionState);
        //    string ip = Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), $"http://v2.api.iphub.info/ip/{ip}"))
        //        {
        //            Console.WriteLine("point 1");
        //            request.Headers.TryAddWithoutValidation("X-Key", "NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ==");
        //            request.Headers.TryAddWithoutValidation("X-Key", $"{Instance.Configuration.Instance.API_Key}");
        //            HttpResponseMessage response = await httpClient.SendAsync(request);
        //            Console.WriteLine("point 2");
        //            string json = await response.Content.ReadAsStringAsync();
        //            Console.WriteLine("point 3");
        //            ConnectedIP client = JsonConvert.DeserializeObject<ConnectedIP>(json);
        //            Console.WriteLine(json);
        //            string[] str = json.Split(':');
        //            string countryCode = str[2].Substring(1, 2);
        //            string block = str[str.Length - 2].Substring(0, 1);
        //            Console.WriteLine();
        //            Console.WriteLine($"client.block: {client.block}, client.ip: {client.ip}");
        //            string[] str2 = client.ip.Split('.');
        //            byte.TryParse(str2[0], out byte num1);
        //            byte.TryParse(str2[1], out byte num2);
        //             || IsPrivateIP(num1, num2) || client.countryCode == "ZZ"
        //            if (client.block != 0)
        //            {
        //                BanDisconnect(player, steamID, ip, hwid, false, $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}", $"VPN/Proxy)", 0);
        //                GlobalBan.Instance.Database.BanPlayer(player.Player.channel.owner.playerID.characterName, player.CSteamID.ToString(), ip, hwid, "Server", "", 0);//0=forever
        //                UnturnedChat.Say(GlobalBan.Instance.Translate("command_ban_private", player.Player.channel.owner.playerID.characterName));
        //                Provider.kick(player.CSteamID, GlobalBan.Instance.Translate("command_ban_private_default_reason"));
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
                //case 192:
                //    return number2 == 168;
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

        //internal void BanDisconnect(string player, CSteamID steamID, string ip, string hwid, bool publicsay, string admin, string reason, uint duration)
        //{
        //    Instance.Database.BanPlayer(player.ToLower(), steamID.ToString(), ip, hwid, admin, reason, duration);//0=forever
        //    if (publicsay)
        //        UnturnedChat.Say(Instance.Translate("command_ban_public_reason", player, reason));
        //    Provider.kick(steamID, reason == "" ? "Permanent ban" : reason);
        //    SendInDiscord(player, steamID.ToString(), reason == "" ? "N/A" : reason, duration, admin, Provider.map);
        //}


//Provider.kick(player.CSteamID, $"{(date == DateTime.MaxValue ? "You are permanently banned on all servers!" : "You are banned on all servers till " + date.AddHours(-UTCoffset))} UTC " + $"For: {reason}");
//Provider.kick(player.CSteamID, $"{(date2 == DateTime.MaxValue ? "You are permanently banned on this server" : "You are banned on this server till " + date2.AddHours(-UTCoffset))} UTC " + $"For: {reason2}");
                

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

        //private void Events_OnJoinRequested(CSteamID player, ref ESteamRejection? rejection)
        //{
        //    try
        //    {
        //        if (IsBadIP(player) || Database.IsBanned(player))
        //        {
        //            rejection = ESteamRejection.AUTH_PUB_BAN;
        //            //Logger.Log($"VPN Connection rejected");
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("Exception in Events_OnJoinRequested");
        //        Console.WriteLine(e.Message);
        //    }
        //}


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
