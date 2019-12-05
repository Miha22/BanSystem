//// Decompiled with JetBrains decompiler
//// Type: SDG.Unturned.Provider
//// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: 54B6081C-175E-49DA-B3CC-9C3A7295EED9
//// Assembly location: \\Mac\Home\Documents\GitHub\BarricadeManager\lib\Assembly-CSharp.dll

//using BattlEye;//using SDG.Framework.Debug;//using SDG.Framework.IO.FormattedFiles;//using SDG.Framework.IO.FormattedFiles.KeyValueTables;//using SDG.Framework.Modules;//using SDG.Framework.Translations;//using SDG.Provider;//using SDG.Provider.Services.Community;//using SDG.Provider.Services.Multiplayer;//using SDG.Provider.Services.Multiplayer.Server;//using SDG.SteamworksProvider;//using SDG.SteamworksProvider.Services.Community;//using Steamworks;//using System;//using System.Collections;//using System.Collections.Generic;//using System.Diagnostics;//using System.Globalization;//using System.IO;//using System.Reflection;//using System.Runtime.InteropServices;//using System.Text;//using UnityEngine;//namespace SDG.Unturned//{
//    public class Provider : MonoBehaviour
//    {
//        public static readonly string STEAM_IC = "Steam";
//        public static readonly string STEAM_DC = "<color=#2784c6>Steam</color>";
//        public static readonly AppId_t APP_ID = new AppId_t(304930U);
//        public static readonly AppId_t PRO_ID = new AppId_t(306460U);
//        public static readonly string APP_NAME = "Unturned";
//        public static readonly string APP_AUTHOR = "Nelson Sexton";
//        public static readonly int CLIENT_TIMEOUT = 30;
//        private static readonly float PING_REQUEST_INTERVAL = 1f;
//        private static IntPtr battlEyeClientHandle = IntPtr.Zero;
//        private static BEClient.BECL_GAME_DATA battlEyeClientInitData = (BEClient.BECL_GAME_DATA)null;
//        private static BEClient.BECL_BE_DATA battlEyeClientRunData = (BEClient.BECL_BE_DATA)null;
//        private static bool battlEyeHasRequiredRestart = false;
//        private static IntPtr battlEyeServerHandle = IntPtr.Zero;
//        private static BEServer.BESV_GAME_DATA battlEyeServerInitData = (BEServer.BESV_GAME_DATA)null;
//        private static BEServer.BESV_BE_DATA battlEyeServerRunData = (BEServer.BESV_BE_DATA)null;
//        private static List<ulong> _serverWorkshopFileIDs = new List<ulong>();
//        private static int _channels = 1;
//        private static byte[] buffer = new byte[Block.BUFFER_SIZE];
//        private static List<SDG.Framework.Modules.Module> critMods = new List<SDG.Framework.Modules.Module>();
//        private static StringBuilder modBuilder = new StringBuilder();
//        private static int countShutdownTimer = -1;
//        private static string shutdownMessage = string.Empty;
//        private static List<SDG.Unturned.Provider.WorkshopRequestLog> workshopRequests = new List<SDG.Unturned.Provider.WorkshopRequestLog>();
//        private static List<SDG.Unturned.Provider.CachedWorkshopResponse> cachedWorkshopResponses = new List<SDG.Unturned.Provider.CachedWorkshopResponse>();
//        private static List<CSteamID> netIgnoredSteamIDs = new List<CSteamID>();
//        private static uint STEAM_FAVORITE_FLAG_FAVORITE = 1;
//        private static uint STEAM_FAVORITE_FLAG_HISTORY = 2;
//        private static List<SDG.Unturned.Provider.CachedFavorite> cachedFavorites = new List<SDG.Unturned.Provider.CachedFavorite>();
//        private static HAuthTicket ticketHandle = HAuthTicket.Invalid;
//        public static readonly float EPSILON = 0.01f;
//        public static readonly float UPDATE_TIME = 0.08f;
//        public static readonly float UPDATE_DELAY = 0.1f;
//        public static readonly float UPDATE_DISTANCE = 0.01f;
//        public static readonly uint UPDATES = 1;
//        public static readonly float LERP = 3f;
//        private static DateTime unixEpochDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
//        private static Dictionary<string, Texture2D> downloadedIconCache = new Dictionary<string, Texture2D>();
//        public static uint[] serverListBlacklistedIPs = (uint[])null;
//        private static string _language;
//        private static string _path;
//        public static Local localization;
//        private static uint _bytesSent;
//        private static uint _bytesReceived;
//        private static uint _packetsSent;
//        private static uint _packetsReceived;
//        private static SteamServerInfo _currentServerInfo;
//        private static CSteamID _server;
//        private static CSteamID _client;
//        private static CSteamID _user;
//        private static byte[] _clientHash;
//        private static string _clientName;
//        private static List<SteamPlayer> _clients;
//        public static List<SteamPending> pending;
//        private static bool _isServer;
//        private static bool _isClient;
//        private static bool _isPro;
//        private static bool _isConnected;
//        private static bool isWaitingForWorkshopResponse;
//        public static bool isLoadingUGC;
//        public static bool isLoadingInventory;
//        public static ESteamConnectionFailureInfo _connectionFailureInfo;
//        private static string _connectionFailureReason;
//        private static uint _connectionFailureDuration;
//        private static List<SteamChannel> _receivers;
//        public static SDG.Unturned.Provider.LoginSpawningHandler onLoginSpawning;
//        private static float lastTimerMessage;
//        private static bool isServerConnectedToSteam;
//        private static bool isDedicatedUGCInstalled;
//        public static SDG.Unturned.Provider.ServerWritingPacketHandler onServerWritingPacket;
//        public static SDG.Unturned.Provider.ServerReadingPacketHandler onServerReadingPacket;
//        public static SDG.Unturned.Provider.ServerConnected onServerConnected;
//        public static SDG.Unturned.Provider.ServerDisconnected onServerDisconnected;
//        public static SDG.Unturned.Provider.ServerHosted onServerHosted;
//        public static SDG.Unturned.Provider.ServerShutdown onServerShutdown;
//        private static Callback<P2PSessionConnectFail_t> p2pSessionConnectFail;
//        public static SDG.Unturned.Provider.CheckValid onCheckValid;
//        public static SDG.Unturned.Provider.CheckValidWithExplanation onCheckValidWithExplanation;
//        public static SDG.Unturned.Provider.CheckBanStatusHandler onCheckBanStatus;
//        public static SDG.Unturned.Provider.RequestBanPlayerHandler onBanPlayerRequested;
//        public static SDG.Unturned.Provider.RequestUnbanPlayerHandler onUnbanPlayerRequested;
//        private static Callback<ValidateAuthTicketResponse_t> validateAuthTicketResponse;
//        private static Callback<GSClientGroupStatus_t> clientGroupStatus;
//        private static byte _maxPlayers;
//        public static byte queueSize;
//        private static byte _queuePosition;
//        public static SDG.Unturned.Provider.QueuePositionUpdated onQueuePositionUpdated;
//        private static string _serverName;
//        public static uint ip;
//        public static ushort port;
//        private static byte[] _serverPasswordHash;
//        private static string _serverPassword;
//        public static string map;
//        public static bool isPvP;
//        public static bool isWhitelisted;
//        public static bool hideAdmins;
//        public static bool hasCheats;
//        public static bool filterName;
//        public static EGameMode mode;
//        public static bool isGold;
//        public static GameMode gameMode;
//        public static string selectedGameModeName;
//        public static ECameraMode cameraMode;
//        private static StatusData _statusData;
//        private static PreferenceData _preferenceData;
//        private static ConfigData _configData;
//        private static ModeConfigData _modeConfigData;
//        private static uint favoriteIP;
//        private static ushort favoritePort;
//        public static SDG.Unturned.Provider.ClientConnected onClientConnected;
//        public static SDG.Unturned.Provider.ClientDisconnected onClientDisconnected;
//        public static SDG.Unturned.Provider.EnemyConnected onEnemyConnected;
//        public static SDG.Unturned.Provider.EnemyDisconnected onEnemyDisconnected;
//        private static Callback<PersonaStateChange_t> personaStateChange;
//        private static Callback<GameServerChangeRequested_t> gameServerChangeRequested;
//        private static Callback<GameRichPresenceJoinRequested_t> gameRichPresenceJoinRequested;
//        private static float lastPingRequestTime;
//        private static float timeLastPingRequestWasSentToServer;
//        private static float lastReceivedServersideTime;
//        private static float[] pings;
//        private static float _ping;
//        private static SDG.Unturned.Provider steam;
//        private static bool _isInitialized;
//        private static uint timeOffset;
//        private static uint _time;
//        private static uint initialBackendRealtimeSeconds;
//        private static float initialLocalRealtime;
//        public static SDG.Unturned.Provider.BackendRealtimeAvailableHandler onBackendRealtimeAvailable;
//        private static SteamAPIWarningMessageHook_t apiWarningMessageHook;
//        private static int debugUpdates;
//        public static int debugUPS;
//        private static float debugLastUpdate;
//        private static int debugTicks;
//        public static int debugTPS;
//        private static float debugLastTick;

//        public static string APP_VERSION { get; protected set; }

//        public static void takeScreenshot()
//        {
//            ScreenCapture.CaptureScreenshot(ReadWrite.PATH + "/Screenshot.png", 2);
//            ScreenshotHandle library = SteamScreenshots.AddScreenshotToLibrary(ReadWrite.PATH + "/Screenshot.png", (string)null, Screen.width * 2, Screen.height * 2);
//            Terminal.print("Screenshot handle: " + (object)library, (string)null, SDG.Unturned.Provider.STEAM_IC, SDG.Unturned.Provider.STEAM_DC, true);
//            string pchLocation;
//            if (Level.info != null)
//            {
//                Local local = Localization.tryRead(Level.info.path, false);
//                pchLocation = local == null || !local.has("Name") ? Level.info.name : local.format("Name");
//            }
//            else
//                pchLocation = "Misc";
//            SteamScreenshots.SetLocation(library, pchLocation);
//            foreach (SteamPlayer client in SDG.Unturned.Provider.clients)
//            {
//                if (!((UnityEngine.Object)client.player == (UnityEngine.Object)null) && !client.player.channel.isOwner)
//                {
//                    Vector3 viewportPoint = MainCamera.instance.WorldToViewportPoint(client.player.transform.position + Vector3.up);
//                    if ((double)viewportPoint.x >= 0.0 && (double)viewportPoint.x <= 1.0 && ((double)viewportPoint.y >= 0.0 && (double)viewportPoint.y <= 1.0))
//                        SteamScreenshots.TagUser(library, client.playerID.steamID);
//                }
//            }
//        }

//        public static string language
//        {
//            get
//            {
//                return SDG.Unturned.Provider._language;
//            }
//        }

//        public static string path
//        {
//            get
//            {
//                return SDG.Unturned.Provider._path;
//            }
//        }

//        public static string localizationRoot { get; private set; }

//        public static List<string> streamerNames { get; private set; }

//        protected static void handleLanguageChanged(string oldLanguage, string newLanguage)
//        {
//            if (oldLanguage != "english")
//                Translator.unloadTranslations(oldLanguage);
//            if (!(newLanguage != "english"))
//                return;
//            Translator.loadTranslations(newLanguage);
//        }

//        protected static void handleTranslationRegistered(string language, string ns)
//        {
//            if (!Dedicator.isDedicated && !Translator.isOriginLanguage(language) && !Translator.isCurrentLanguage(language))
//                return;
//            Translator.loadTranslation(language, ns);
//        }

//        private static void battlEyeClientPrintMessage(string message)
//        {
//            Terminal.print(message, message, "BattlEye Client", "<color=yellow>BattlEye Client</color>", true);
//        }

//        private static void battlEyeClientRequestRestart(int reason)
//        {
//            switch (reason)
//            {
//                case 0:
//                    SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.BATTLEYE_BROKEN;
//                    break;
//                case 1:
//                    SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.BATTLEYE_UPDATE;
//                    break;
//                default:
//                    SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.BATTLEYE_UNKNOWN;
//                    break;
//            }
//            SDG.Unturned.Provider.battlEyeHasRequiredRestart = true;
//            UnityEngine.Debug.Log((object)("BattlEye client requested restart with reason: " + (object)reason));
//        }

//        private static void battlEyeClientSendPacket(IntPtr packetHandle, int length)
//        {
//            Block.buffer[0] = (byte)24;
//            Marshal.Copy(packetHandle, Block.buffer, 1, length);
//            SDG.Unturned.Provider.send(SDG.Unturned.Provider.server, ESteamPacket.BATTLEYE, Block.buffer, 1 + length, 0);
//        }

//        private static void battlEyeServerPrintMessage(string message)
//        {
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                SteamPlayer client = SDG.Unturned.Provider.clients[index];
//                if (client != null && !((UnityEngine.Object)client.player == (UnityEngine.Object)null) && client.player.wantsBattlEyeLogs)
//                    client.player.sendTerminalRelay(message, "BattlEye Server", "<color=yellow>BattlEye Server</color>");
//            }
//            if (!CommandWindow.shouldLogAnticheat)
//                return;
//            CommandWindow.Log((object)("BattlEye Server: " + message));
//        }

//        private static void battlEyeServerKickPlayer(int playerID, string reason)
//        {
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.clients[index].channel == playerID)
//                {
//                    if (reason.Length == 18 && reason.StartsWith("Global Ban #"))
//                        ChatManager.say(SDG.Unturned.Provider.clients[index].playerID.playerName + " got banned by BattlEye", Color.yellow, false);
//                    SDG.Unturned.Provider.kick(SDG.Unturned.Provider.clients[index].playerID.steamID, "BattlEye: " + reason);
//                    break;
//                }
//            }
//        }

//        private static void battlEyeServerSendPacket(int playerID, IntPtr packetHandle, int length)
//        {
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.clients[index].channel == playerID)
//                {
//                    Block.buffer[0] = (byte)24;
//                    Marshal.Copy(packetHandle, Block.buffer, 1, length);
//                    SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index].playerID.steamID, ESteamPacket.BATTLEYE, Block.buffer, 1 + length, SDG.Unturned.Provider.clients[index].channel);
//                    break;
//                }
//            }
//        }

//        public static void updateRichPresence()
//        {
//            if (Dedicator.isDedicated)
//                return;
//            SDG.Unturned.Provider.updateSteamRichPresence();
//        }

//        private static void updateSteamRichPresence()
//        {
//            if (Level.info != null)
//            {
//                if (Level.isEditor)
//                    SDG.Unturned.Provider.provider.communityService.setStatus(SDG.Unturned.Provider.localization.format("Rich_Presence_Editing", (object)Level.info.name));
//                else
//                    SDG.Unturned.Provider.provider.communityService.setStatus(SDG.Unturned.Provider.localization.format("Rich_Presence_Playing", (object)Level.info.name));
//            }
//            else if (Lobbies.inLobby)
//                SDG.Unturned.Provider.provider.communityService.setStatus(SDG.Unturned.Provider.localization.format("Rich_Presence_Lobby"));
//            else
//                SDG.Unturned.Provider.provider.communityService.setStatus(SDG.Unturned.Provider.localization.format("Rich_Presence_Menu"));
//        }

//        public static uint bytesSent
//        {
//            get
//            {
//                return SDG.Unturned.Provider._bytesSent;
//            }
//        }

//        public static uint bytesReceived
//        {
//            get
//            {
//                return SDG.Unturned.Provider._bytesReceived;
//            }
//        }

//        public static uint packetsSent
//        {
//            get
//            {
//                return SDG.Unturned.Provider._packetsSent;
//            }
//        }

//        public static uint packetsReceived
//        {
//            get
//            {
//                return SDG.Unturned.Provider._packetsReceived;
//            }
//        }

//        public static SteamServerInfo currentServerInfo
//        {
//            get
//            {
//                return SDG.Unturned.Provider._currentServerInfo;
//            }
//        }

//        public static CSteamID server
//        {
//            get
//            {
//                return SDG.Unturned.Provider._server;
//            }
//        }

//        public static CSteamID client
//        {
//            get
//            {
//                return SDG.Unturned.Provider._client;
//            }
//        }

//        public static CSteamID user
//        {
//            get
//            {
//                return SDG.Unturned.Provider._user;
//            }
//        }

//        public static byte[] clientHash
//        {
//            get
//            {
//                return SDG.Unturned.Provider._clientHash;
//            }
//        }

//        public static string clientName
//        {
//            get
//            {
//                return SDG.Unturned.Provider._clientName;
//            }
//        }

//        public static List<SteamPlayer> clients
//        {
//            get
//            {
//                return SDG.Unturned.Provider._clients;
//            }
//        }

//        [Obsolete]
//        public static List<SteamPlayer> players
//        {
//            get
//            {
//                return SDG.Unturned.Provider.clients;
//            }
//        }

//        public static bool isServer
//        {
//            get
//            {
//                return SDG.Unturned.Provider._isServer;
//            }
//        }

//        public static bool isClient
//        {
//            get
//            {
//                return SDG.Unturned.Provider._isClient;
//            }
//        }

//        public static bool isPro
//        {
//            get
//            {
//                return SDG.Unturned.Provider._isPro;
//            }
//        }

//        public static bool isConnected
//        {
//            get
//            {
//                return SDG.Unturned.Provider._isConnected;
//            }
//        }

//        private static void receiveWorkshopResponse(SDG.Unturned.Provider.CachedWorkshopResponse response)
//        {
//            SDG.Unturned.Provider.isWaitingForWorkshopResponse = false;
//            SDG.Unturned.Provider.provider.workshopService.installing = new List<PublishedFileId_t>();
//            foreach (PublishedFileId_t publishedFileId in response.publishedFileIds)
//            {
//                PublishedFileId_t file = publishedFileId;
//                if (file.m_PublishedFileId != 0UL)
//                {
//                    ulong punSizeOnDisk;
//                    string pchFolder;
//                    uint punTimeStamp;
//                    if (SteamUGC.GetItemInstallInfo(file, out punSizeOnDisk, out pchFolder, 1024U, out punTimeStamp))
//                    {
//                        if (SDG.Unturned.Provider.provider.workshopService.ugc.Find((Predicate<SteamContent>)(x => x.publishedFileID == file)) == null)
//                            SDG.Unturned.Provider.provider.workshopService.installing.Add(file);
//                    }
//                    else
//                        SDG.Unturned.Provider.provider.workshopService.installing.Add(file);
//                }
//            }
//            SDG.Unturned.Provider.provider.workshopService.installed = SDG.Unturned.Provider.provider.workshopService.installing.Count;
//            if (SDG.Unturned.Provider.provider.workshopService.installed == 0)
//                SDG.Unturned.Provider.launch();
//            else
//                SteamUGC.DownloadItem(SDG.Unturned.Provider.provider.workshopService.installing[0], true);
//        }

//        public static List<ulong> getServerWorkshopFileIDs()
//        {
//            return SDG.Unturned.Provider._serverWorkshopFileIDs;
//        }

//        public static void registerServerUsingWorkshopFileId(ulong id)
//        {
//            if (SDG.Unturned.Provider._serverWorkshopFileIDs.Contains(id))
//                return;
//            SDG.Unturned.Provider._serverWorkshopFileIDs.Add(id);
//        }

//        public static bool isLoading
//        {
//            get
//            {
//                return SDG.Unturned.Provider.isLoadingUGC;
//            }
//        }

//        public static int channels
//        {
//            get
//            {
//                return SDG.Unturned.Provider._channels;
//            }
//        }

//        public static ESteamConnectionFailureInfo connectionFailureInfo
//        {
//            get
//            {
//                return SDG.Unturned.Provider._connectionFailureInfo;
//            }
//            set
//            {
//                SDG.Unturned.Provider._connectionFailureInfo = value;
//            }
//        }

//        public static string connectionFailureReason
//        {
//            get
//            {
//                return SDG.Unturned.Provider._connectionFailureReason;
//            }
//            set
//            {
//                SDG.Unturned.Provider._connectionFailureReason = value;
//            }
//        }

//        public static uint connectionFailureDuration
//        {
//            get
//            {
//                return SDG.Unturned.Provider._connectionFailureDuration;
//            }
//        }

//        public static List<SteamChannel> receivers
//        {
//            get
//            {
//                return SDG.Unturned.Provider._receivers;
//            }
//        }

//        public static void resetConnectionFailure()
//        {
//            SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NONE;
//            SDG.Unturned.Provider._connectionFailureReason = string.Empty;
//            SDG.Unturned.Provider._connectionFailureDuration = 0U;
//        }

//        public static void openChannel(SteamChannel receiver)
//        {
//            if (SDG.Unturned.Provider.receivers == null)
//            {
//                SDG.Unturned.Provider.resetChannels();
//            }
//            else
//            {
//                SDG.Unturned.Provider.receivers.Add(receiver);
//                ++SDG.Unturned.Provider._channels;
//            }
//        }

//        public static void closeChannel(SteamChannel receiver)
//        {
//            for (int index = 0; index < SDG.Unturned.Provider.receivers.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.receivers[index].id == receiver.id)
//                {
//                    SDG.Unturned.Provider.receivers.RemoveAt(index);
//                    break;
//                }
//            }
//        }

//        private static SteamPlayer addPlayer(
//          SteamPlayerID playerID,
//          Vector3 point,
//          byte angle,
//          bool isPro,
//          bool isAdmin,
//          int channel,
//          byte face,
//          byte hair,
//          byte beard,
//          Color skin,
//          Color color,
//          Color markerColor,
//          bool hand,
//          int shirtItem,
//          int pantsItem,
//          int hatItem,
//          int backpackItem,
//          int vestItem,
//          int maskItem,
//          int glassesItem,
//          int[] skinItems,
//          string[] skinTags,
//          string[] skinDynamicProps,
//          EPlayerSkillset skillset,
//          string language,
//          CSteamID lobbyID)
//        {
//            if (!Dedicator.isDedicated && playerID.steamID != SDG.Unturned.Provider.client)
//                SteamFriends.SetPlayedWith(playerID.steamID);
//            if (playerID.steamID == SDG.Unturned.Provider.client)
//            {
//                string str = skillset.ToString();
//                int num1 = 0;
//                int num2 = 0;
//                if (shirtItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(shirtItem) != (ushort)0)
//                        ++num2;
//                }
//                if (pantsItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(pantsItem) != (ushort)0)
//                        ++num2;
//                }
//                if (hatItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(hatItem) != (ushort)0)
//                        ++num2;
//                }
//                if (backpackItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(backpackItem) != (ushort)0)
//                        ++num2;
//                }
//                if (vestItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(vestItem) != (ushort)0)
//                        ++num2;
//                }
//                if (maskItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(maskItem) != (ushort)0)
//                        ++num2;
//                }
//                if (glassesItem != 0)
//                {
//                    ++num1;
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(glassesItem) != (ushort)0)
//                        ++num2;
//                }
//                int length = skinItems.Length;
//                for (int index = 0; index < skinItems.Length; ++index)
//                {
//                    if (SDG.Unturned.Provider.provider.economyService.getInventoryMythicID(skinItems[index]) != (ushort)0)
//                        ++num2;
//                }
//                UnityEngine.Analytics.Analytics.CustomEvent("Character", (IDictionary<string, object>)new Dictionary<string, object>()//        {//          {//            "Ability",//            (object) str//          },//          {//            "Cosmetics",//            (object) num1//          },//          {//            "Mythics",//            (object) num2//          },//          {//            "Skins",//            (object) length//          }//        });
//            }
//            Transform newModel = (Transform)null;
//            try
//            {
//                newModel = SDG.Unturned.Provider.gameMode.getPlayerGameObject(playerID).transform;
//                newModel.position = point;
//                newModel.rotation = Quaternion.Euler(0.0f, (float)((int)angle * 2), 0.0f);
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogError((object)"Exception thrown when getting player from game mode:");
//                UnityEngine.Debug.LogException(ex);
//            }
//            SteamPlayer player = (SteamPlayer)null;
//            try
//            {
//                player = new SteamPlayer(playerID, newModel, isPro, isAdmin, channel, face, hair, beard, skin, color, markerColor, hand, shirtItem, pantsItem, hatItem, backpackItem, vestItem, maskItem, glassesItem, skinItems, skinTags, skinDynamicProps, skillset, language, lobbyID);
//                SDG.Unturned.Provider.clients.Add(player);
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogError((object)"Exception thrown when adding player:");
//                UnityEngine.Debug.LogException(ex);
//            }
//            SDG.Unturned.Provider.updateRichPresence();
//            if (SDG.Unturned.Provider.onEnemyConnected != null)
//                SDG.Unturned.Provider.onEnemyConnected(player);
//            return player;
//        }

//        private static void removePlayer(byte index)
//        {
//            if (index < (byte)0 || (int)index >= SDG.Unturned.Provider.clients.Count)
//            {
//                UnityEngine.Debug.LogError((object)("Failed to find player: " + (object)index));
//            }
//            else
//            {
//                if (SDG.Unturned.Provider.battlEyeServerHandle != IntPtr.Zero && SDG.Unturned.Provider.battlEyeServerRunData != null && SDG.Unturned.Provider.battlEyeServerRunData.pfnChangePlayerStatus != null)
//                    SDG.Unturned.Provider.battlEyeServerRunData.pfnChangePlayerStatus(SDG.Unturned.Provider.clients[(int)index].channel, -1);
//                SDG.Unturned.Provider.steam.StartCoroutine("close", (object)SDG.Unturned.Provider.clients[(int)index].playerID.steamID);
//                if (SDG.Unturned.Provider.onEnemyDisconnected != null)
//                    SDG.Unturned.Provider.onEnemyDisconnected(SDG.Unturned.Provider.clients[(int)index]);
//                if ((UnityEngine.Object)SDG.Unturned.Provider.clients[(int)index].model != (UnityEngine.Object)null)
//                    UnityEngine.Object.Destroy((UnityEngine.Object)SDG.Unturned.Provider.clients[(int)index].model.gameObject);
//                SDG.Unturned.Provider.clients.RemoveAt((int)index);
//                SDG.Unturned.Provider.verifyNextPlayerInQueue();
//                SDG.Unturned.Provider.updateRichPresence();
//            }
//        }

//        private static void verifyNextPlayerInQueue()
//        {
//            if (SDG.Unturned.Provider.pending.Count < 1 || SDG.Unturned.Provider.clients.Count >= (int)SDG.Unturned.Provider.maxPlayers)
//                return;
//            SteamPending steamPending = SDG.Unturned.Provider.pending[0];
//            if (steamPending.hasSentVerifyPacket)
//                return;
//            steamPending.sendVerifyPacket();
//        }

//        private static bool isInstant(ESteamPacket packet)
//        {
//            return packet == ESteamPacket.UPDATE_RELIABLE_INSTANT || packet == ESteamPacket.UPDATE_UNRELIABLE_INSTANT || (packet == ESteamPacket.UPDATE_RELIABLE_INSTANT || packet == ESteamPacket.UPDATE_UNRELIABLE_INSTANT) || (packet == ESteamPacket.UPDATE_RELIABLE_CHUNK_INSTANT || packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_INSTANT);
//        }

//        private static bool isUnreliable(ESteamPacket packet)
//        {
//            return packet == ESteamPacket.UPDATE_UNRELIABLE_BUFFER || packet == ESteamPacket.UPDATE_UNRELIABLE_INSTANT || (packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_BUFFER || packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_INSTANT) || (packet == ESteamPacket.UPDATE_VOICE || packet == ESteamPacket.PING_REQUEST || (packet == ESteamPacket.PING_RESPONSE || packet == ESteamPacket.BATTLEYE));
//        }

//        public static bool isChunk(ESteamPacket packet)
//        {
//            return packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_BUFFER || packet == ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER || (packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_INSTANT || packet == ESteamPacket.UPDATE_RELIABLE_CHUNK_INSTANT);
//        }

//        private static bool isUpdate(ESteamPacket packet)
//        {
//            return packet == ESteamPacket.UPDATE_RELIABLE_BUFFER || packet == ESteamPacket.UPDATE_UNRELIABLE_BUFFER || (packet == ESteamPacket.UPDATE_RELIABLE_INSTANT || packet == ESteamPacket.UPDATE_UNRELIABLE_INSTANT) || (packet == ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER || packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_BUFFER || (packet == ESteamPacket.UPDATE_RELIABLE_CHUNK_INSTANT || packet == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_INSTANT)) || packet == ESteamPacket.UPDATE_VOICE;
//        }

//        private static void resetChannels()
//        {
//            SDG.Unturned.Provider._bytesSent = 0U;
//            SDG.Unturned.Provider._bytesReceived = 0U;
//            SDG.Unturned.Provider._packetsSent = 0U;
//            SDG.Unturned.Provider._packetsReceived = 0U;
//            SDG.Unturned.Provider._channels = 1;
//            SDG.Unturned.Provider._receivers = new List<SteamChannel>();
//            foreach (SteamChannel receiver in UnityEngine.Object.FindObjectsOfType<SteamChannel>())
//                SDG.Unturned.Provider.openChannel(receiver);
//            SDG.Unturned.Provider._clients = new List<SteamPlayer>();
//            SDG.Unturned.Provider.pending = new List<SteamPending>();
//        }

//        private static void loadPlayerSpawn(
//          SteamPlayerID playerID,
//          out Vector3 point,
//          out byte angle,
//          out EPlayerStance initialStance)
//        {
//            point = Vector3.zero;
//            angle = (byte)0;
//            initialStance = EPlayerStance.STAND;
//            bool needsNewSpawnpoint = false;
//            if (PlayerSavedata.fileExists(playerID, "/Player/Player.dat") && Level.info.type == ELevelType.SURVIVAL)
//            {
//                Block block = PlayerSavedata.readBlock(playerID, "/Player/Player.dat", (byte)1);
//                point = block.readSingleVector3() + new Vector3(0.0f, 0.1f, 0.0f);
//                angle = block.readByte();
//                if (point.IsFinite())
//                {
//                    if (Physics.OverlapCapsuleNonAlloc(point + new Vector3(0.0f, PlayerStance.RADIUS, 0.0f), point + new Vector3(0.0f, PlayerMovement.HEIGHT_STAND - PlayerStance.RADIUS, 0.0f), PlayerStance.RADIUS, PlayerStance.checkColliders, RayMasks.BLOCK_STANCE, QueryTriggerInteraction.Ignore) == 0)
//                        initialStance = EPlayerStance.STAND;
//                    else if (Physics.OverlapCapsuleNonAlloc(point + new Vector3(0.0f, PlayerStance.RADIUS, 0.0f), point + new Vector3(0.0f, PlayerMovement.HEIGHT_CROUCH - PlayerStance.RADIUS, 0.0f), PlayerStance.RADIUS, PlayerStance.checkColliders, RayMasks.BLOCK_STANCE, QueryTriggerInteraction.Ignore) == 0)
//                        initialStance = EPlayerStance.CROUCH;
//                    else if (Physics.OverlapCapsuleNonAlloc(point + new Vector3(0.0f, PlayerStance.RADIUS, 0.0f), point + new Vector3(0.0f, PlayerMovement.HEIGHT_PRONE - PlayerStance.RADIUS, 0.0f), PlayerStance.RADIUS, PlayerStance.checkColliders, RayMasks.BLOCK_STANCE, QueryTriggerInteraction.Ignore) == 0)
//                        initialStance = EPlayerStance.PRONE;
//                    else
//                        needsNewSpawnpoint = true;
//                }
//                else
//                    needsNewSpawnpoint = true;
//            }
//            else
//                needsNewSpawnpoint = true;
//            try
//            {
//                if (SDG.Unturned.Provider.onLoginSpawning != null)
//                {
//                    float yaw = (float)((int)angle * 2);
//                    SDG.Unturned.Provider.onLoginSpawning(playerID, ref point, ref yaw, ref initialStance, ref needsNewSpawnpoint);
//                    angle = (byte)((double)yaw / 2.0);
//                }
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onLoginSpawning:");
//                UnityEngine.Debug.LogException(ex);
//            }
//            if (!needsNewSpawnpoint)
//                return;
//            PlayerSpawnpoint spawn = LevelPlayers.getSpawn(false);
//            point = spawn.point + new Vector3(0.0f, 0.5f, 0.0f);
//            angle = (byte)((double)spawn.angle / 2.0);
//        }

//        private static void onLevelLoaded(int level)
//        {
//            if (level != 2)
//                return;
//            SDG.Unturned.Provider.isLoadingUGC = false;
//            if (!SDG.Unturned.Provider.isConnected)
//                return;
//            if (SDG.Unturned.Provider.isServer)
//            {
//                if (!SDG.Unturned.Provider.isClient)
//                    return;
//                SteamPlayerID playerID = new SteamPlayerID(SDG.Unturned.Provider.client, Characters.selected, SDG.Unturned.Provider.clientName, Characters.active.name, Characters.active.nick, Characters.active.group);
//                Vector3 point;
//                byte angle;
//                EPlayerStance initialStance;
//                SDG.Unturned.Provider.loadPlayerSpawn(playerID, out point, out angle, out initialStance);
//                int inventoryItem1 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packageShirt);
//                int inventoryItem2 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packagePants);
//                int inventoryItem3 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packageHat);
//                int inventoryItem4 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packageBackpack);
//                int inventoryItem5 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packageVest);
//                int inventoryItem6 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packageMask);
//                int inventoryItem7 = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.active.packageGlasses);
//                int[] skinItems = new int[Characters.packageSkins.Count];
//                for (int index = 0; index < skinItems.Length; ++index)
//                    skinItems[index] = SDG.Unturned.Provider.provider.economyService.getInventoryItem(Characters.packageSkins[index]);
//                string[] skinTags = new string[Characters.packageSkins.Count];
//                for (int index = 0; index < skinTags.Length; ++index)
//                    skinTags[index] = SDG.Unturned.Provider.provider.economyService.getInventoryTags(Characters.packageSkins[index]);
//                string[] skinDynamicProps = new string[Characters.packageSkins.Count];
//                for (int index = 0; index < skinDynamicProps.Length; ++index)
//                    skinDynamicProps[index] = SDG.Unturned.Provider.provider.economyService.getInventoryDynamicProps(Characters.packageSkins[index]);
//                SDG.Unturned.Provider.addPlayer(playerID, point, angle, SDG.Unturned.Provider.isPro, true, SDG.Unturned.Provider.channels, Characters.active.face, Characters.active.hair, Characters.active.beard, Characters.active.skin, Characters.active.color, Characters.active.markerColor, Characters.active.hand, inventoryItem1, inventoryItem2, inventoryItem3, inventoryItem4, inventoryItem5, inventoryItem6, inventoryItem7, skinItems, skinTags, skinDynamicProps, Characters.active.skillset, Translator.language, Lobbies.currentLobby).player.stance.initialStance = initialStance;
//                Lobbies.leaveLobby();
//                SDG.Unturned.Provider.updateRichPresence();
//                try
//                {
//                    if (SDG.Unturned.Provider.onServerConnected == null)
//                        return;
//                    SDG.Unturned.Provider.onServerConnected(playerID.steamID);
//                }
//                catch (Exception ex)
//                {
//                    UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerConnected:");
//                    UnityEngine.Debug.LogException(ex);
//                }
//            }
//            else
//            {
//                byte num = 1;
//                SDG.Unturned.Provider.critMods.Clear();
//                SDG.Unturned.Provider.modBuilder.Length = 0;
//                ModuleHook.getRequiredModules(SDG.Unturned.Provider.critMods);
//                for (int index = 0; index < SDG.Unturned.Provider.critMods.Count; ++index)
//                {
//                    SDG.Unturned.Provider.modBuilder.Append(SDG.Unturned.Provider.critMods[index].config.Name);
//                    SDG.Unturned.Provider.modBuilder.Append(",");
//                    SDG.Unturned.Provider.modBuilder.Append(SDG.Unturned.Provider.critMods[index].config.Version_Internal);
//                    if (index < SDG.Unturned.Provider.critMods.Count - 1)
//                        SDG.Unturned.Provider.modBuilder.Append(";");
//                }
//                int size;
//                SDG.Unturned.Provider.send(SDG.Unturned.Provider.server, ESteamPacket.CONNECT, SteamPacker.getBytes(0, out size, (object)(byte)2, (object)Characters.selected, (object)SDG.Unturned.Provider.clientName, (object)Characters.active.name, (object)SDG.Unturned.Provider._serverPasswordHash, (object)Level.hash, (object)ReadWrite.appOut(), (object)num, (object)SDG.Unturned.Provider.APP_VERSION, (object)SDG.Unturned.Provider.isPro, (object)(float)((double)SDG.Unturned.Provider.currentServerInfo.ping / 1000.0), (object)Characters.active.nick, (object)Characters.active.group, (object)Characters.active.face, (object)Characters.active.hair, (object)Characters.active.beard, (object)Characters.active.skin, (object)Characters.active.color, (object)Characters.active.markerColor, (object)Characters.active.hand, (object)Characters.active.packageShirt, (object)Characters.active.packagePants, (object)Characters.active.packageHat, (object)Characters.active.packageBackpack, (object)Characters.active.packageVest, (object)Characters.active.packageMask, (object)Characters.active.packageGlasses, (object)Characters.packageSkins.ToArray(), (object)(byte)Characters.active.skillset, (object)SDG.Unturned.Provider.modBuilder.ToString(), (object)Translator.language, (object)Lobbies.currentLobby), size, 0);
//            }
//        }

//        public static void connect(SteamServerInfo info, string password)
//        {
//            if (SDG.Unturned.Provider.isConnected)
//                return;
//            SDG.Unturned.Provider._currentServerInfo = info;
//            SDG.Unturned.Provider._isConnected = true;
//            SDG.Unturned.Provider.map = info.map;
//            SDG.Unturned.Provider.isPvP = info.isPvP;
//            SDG.Unturned.Provider.isWhitelisted = false;
//            SDG.Unturned.Provider.mode = info.mode;
//            SDG.Unturned.Provider.cameraMode = info.cameraMode;
//            SDG.Unturned.Provider.maxPlayers = (byte)info.maxPlayers;
//            SDG.Unturned.Provider.selectedGameModeName = info.gameMode;
//            SDG.Unturned.Provider._queuePosition = (byte)0;
//            SDG.Unturned.Provider.resetChannels();
//            Lobbies.linkLobby(info.ip, info.port);
//            SDG.Unturned.Provider._server = info.steamID;
//            SDG.Unturned.Provider._serverPassword = password;
//            SDG.Unturned.Provider._serverPasswordHash = Hash.SHA1(password);
//            SDG.Unturned.Provider._isClient = true;
//            SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.lastReceivedServersideTime = 0.0f;
//            SDG.Unturned.Provider.pings = new float[4];
//            SDG.Unturned.Provider.lag((float)info.ping / 1000f);
//            SDG.Unturned.Provider.isLoadingUGC = true;
//            LoadingUI.updateScene();
//            SDG.Unturned.Provider.isWaitingForWorkshopResponse = true;
//            List<SteamItemInstanceID_t> steamItemInstanceIdTList = new List<SteamItemInstanceID_t>();
//            if (Characters.active.packageShirt != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packageShirt);
//            if (Characters.active.packagePants != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packagePants);
//            if (Characters.active.packageHat != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packageHat);
//            if (Characters.active.packageBackpack != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packageBackpack);
//            if (Characters.active.packageVest != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packageVest);
//            if (Characters.active.packageMask != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packageMask);
//            if (Characters.active.packageGlasses != 0UL)
//                steamItemInstanceIdTList.Add((SteamItemInstanceID_t)Characters.active.packageGlasses);
//            for (int index = 0; index < Characters.packageSkins.Count; ++index)
//            {
//                ulong packageSkin = Characters.packageSkins[index];
//                if (packageSkin != 0UL)
//                    steamItemInstanceIdTList.Add((SteamItemInstanceID_t)packageSkin);
//            }
//            if (steamItemInstanceIdTList.Count > 0)
//                SteamInventory.GetItemsByID(out SDG.Unturned.Provider.provider.economyService.wearingResult, steamItemInstanceIdTList.ToArray(), (uint)steamItemInstanceIdTList.Count);
//            Level.loading();
//            SDG.Unturned.Provider.CachedWorkshopResponse response = (SDG.Unturned.Provider.CachedWorkshopResponse)null;
//            foreach (SDG.Unturned.Provider.CachedWorkshopResponse workshopResponse in SDG.Unturned.Provider.cachedWorkshopResponses)
//            {
//                if (workshopResponse.server == SDG.Unturned.Provider.server && (double)Time.realtimeSinceStartup - (double)workshopResponse.realTime < 60.0)
//                {
//                    response = workshopResponse;
//                    break;
//                }
//            }
//            if (response != null)
//                SDG.Unturned.Provider.receiveWorkshopResponse(response);
//            else
//                SDG.Unturned.Provider.send(SDG.Unturned.Provider.server, ESteamPacket.WORKSHOP, new byte[1]
//                {//          (byte) 1
//                }, 1, 0);
//        }

//        public static void launch()
//        {
//            if (!Level.exists(SDG.Unturned.Provider.map))
//            {
//                SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.MAP;
//                SDG.Unturned.Provider.disconnect();
//            }
//            else
//            {
//                Level.load(Level.getLevel(SDG.Unturned.Provider.map), false);
//                SDG.Unturned.Provider.loadGameMode();
//            }
//        }

//        private static void loadGameMode()
//        {
//            if (Level.info == null || Level.info.configData == null)
//            {
//                SDG.Unturned.Provider.gameMode = (GameMode)new SurvivalGameMode();
//            }
//            else
//            {
//                LevelAsset levelAsset = Assets.find<LevelAsset>(Level.info.configData.Asset);
//                if (levelAsset == null)
//                {
//                    SDG.Unturned.Provider.gameMode = (GameMode)new SurvivalGameMode();
//                }
//                else
//                {
//                    Type type = levelAsset.defaultGameMode.type;
//                    if (!string.IsNullOrEmpty(SDG.Unturned.Provider.selectedGameModeName))
//                    {
//                        foreach (TypeReference<GameMode> supportedGameMode in (List<TypeReference<GameMode>>)levelAsset.supportedGameModes)
//                        {
//                            if (supportedGameMode.assemblyQualifiedName.Contains(SDG.Unturned.Provider.selectedGameModeName))
//                            {
//                                type = supportedGameMode.type;
//                                break;
//                            }
//                        }
//                    }
//                    if (type == (Type)null)
//                    {
//                        SDG.Unturned.Provider.gameMode = (GameMode)new SurvivalGameMode();
//                    }
//                    else
//                    {
//                        SDG.Unturned.Provider.gameMode = Activator.CreateInstance(type) as GameMode;
//                        if (SDG.Unturned.Provider.gameMode != null)
//                            return;
//                        SDG.Unturned.Provider.gameMode = (GameMode)new SurvivalGameMode();
//                    }
//                }
//            }
//        }

//        private static void unloadGameMode()
//        {
//            SDG.Unturned.Provider.gameMode = (GameMode)null;
//            SDG.Unturned.Provider.selectedGameModeName = (string)null;
//        }

//        public static void singleplayer(EGameMode singleplayerMode, bool singleplayerCheats)
//        {
//            SDG.Unturned.Provider._isConnected = true;
//            SDG.Unturned.Provider.resetChannels();
//            Dedicator.serverVisibility = ESteamServerVisibility.LAN;
//            Dedicator.serverID = "Singleplayer_" + (object)Characters.selected;
//            Commander.init();
//            SDG.Unturned.Provider.maxPlayers = (byte)1;
//            SDG.Unturned.Provider.queueSize = (byte)8;
//            SDG.Unturned.Provider.serverName = "Singleplayer #" + (object)((int)Characters.selected + 1);
//            SDG.Unturned.Provider.serverPassword = "Singleplayer";
//            SDG.Unturned.Provider.ip = 0U;
//            SDG.Unturned.Provider.port = (ushort)25000;
//            SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.lastReceivedServersideTime = 0.0f;
//            SDG.Unturned.Provider.pings = new float[4];
//            SDG.Unturned.Provider.isPvP = true;
//            SDG.Unturned.Provider.isWhitelisted = false;
//            SDG.Unturned.Provider.hideAdmins = false;
//            SDG.Unturned.Provider.hasCheats = singleplayerCheats;
//            SDG.Unturned.Provider.filterName = false;
//            SDG.Unturned.Provider.mode = singleplayerMode;
//            SDG.Unturned.Provider.isGold = false;
//            SDG.Unturned.Provider.gameMode = (GameMode)null;
//            SDG.Unturned.Provider.selectedGameModeName = (string)null;
//            SDG.Unturned.Provider.cameraMode = ECameraMode.BOTH;
//            if (singleplayerMode != EGameMode.TUTORIAL)
//                PlayerInventory.skillsets = PlayerInventory.SKILLSETS_CLIENT;
//            SDG.Unturned.Provider.lag(0.0f);
//            SteamWhitelist.load();
//            SteamBlacklist.load();
//            SteamAdminlist.load();
//            SDG.Unturned.Provider._currentServerInfo = new SteamServerInfo(SDG.Unturned.Provider.serverName, SDG.Unturned.Provider.mode, false, false, false);
//            if (ServerSavedata.fileExists("/Config.json"))
//            {
//                try
//                {
//                    SDG.Unturned.Provider._configData = ServerSavedata.deserializeJSON<ConfigData>("/Config.json");
//                }
//                catch
//                {
//                    SDG.Unturned.Provider._configData = (ConfigData)null;
//                }
//                if (SDG.Unturned.Provider.configData == null)
//                    SDG.Unturned.Provider._configData = new ConfigData();
//            }
//            else
//                SDG.Unturned.Provider._configData = new ConfigData();
//            switch (SDG.Unturned.Provider.mode)
//            {
//                case EGameMode.EASY:
//                    SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Easy;
//                    break;
//                case EGameMode.NORMAL:
//                    SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Normal;
//                    break;
//                case EGameMode.HARD:
//                    SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Hard;
//                    break;
//                default:
//                    SDG.Unturned.Provider._modeConfigData = new ModeConfigData(SDG.Unturned.Provider.mode);
//                    break;
//            }
//            SDG.Unturned.Provider._time = SteamUtils.GetServerRealTime();
//            Level.load(Level.getLevel(SDG.Unturned.Provider.map), true);
//            SDG.Unturned.Provider.loadGameMode();
//            SDG.Unturned.Provider.applyLevelModeConfigOverrides();
//            SDG.Unturned.Provider._server = SDG.Unturned.Provider.user;
//            SDG.Unturned.Provider._client = SDG.Unturned.Provider._server;
//            SDG.Unturned.Provider._clientHash = Hash.SHA1(SDG.Unturned.Provider.client);
//            SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.lastReceivedServersideTime = 0.0f;
//            SDG.Unturned.Provider._isServer = true;
//            SDG.Unturned.Provider._isClient = true;
//            SDG.Unturned.Provider.broadcastServerHosted();
//        }

//        public static void host()
//        {
//            SDG.Unturned.Provider._isConnected = true;
//            SDG.Unturned.Provider.resetChannels();
//            SDG.Unturned.Provider.openGameServer();
//            SDG.Unturned.Provider._isServer = true;
//            SDG.Unturned.Provider.broadcastServerHosted();
//        }

//        public static void shutdown()
//        {
//            SDG.Unturned.Provider.shutdown(0);
//        }

//        public static void shutdown(int timer)
//        {
//            SDG.Unturned.Provider.shutdown(timer, string.Empty);
//        }

//        public static void shutdown(int timer, string explanation)
//        {
//            SDG.Unturned.Provider.countShutdownTimer = timer;
//            SDG.Unturned.Provider.lastTimerMessage = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.shutdownMessage = explanation;
//        }

//        public static void disconnect()
//        {
//            if (SDG.Unturned.Provider.isServer)
//            {
//                if (SDG.Unturned.Provider.configData != null && SDG.Unturned.Provider.configData.Server.BattlEye_Secure && SDG.Unturned.Provider.battlEyeServerHandle != IntPtr.Zero)
//                {
//                    if (SDG.Unturned.Provider.battlEyeServerRunData != null && SDG.Unturned.Provider.battlEyeServerRunData.pfnExit != null)
//                    {
//                        int num = SDG.Unturned.Provider.battlEyeServerRunData.pfnExit() ? 1 : 0;
//                    }
//                    BEServer.FreeLibrary(SDG.Unturned.Provider.battlEyeServerHandle);
//                    SDG.Unturned.Provider.battlEyeServerHandle = IntPtr.Zero;
//                }
//                if (Dedicator.isDedicated)
//                    SDG.Unturned.Provider.closeGameServer();
//                else
//                    SDG.Unturned.Provider.broadcastServerShutdown();
//                if (SDG.Unturned.Provider.isClient)
//                {
//                    SDG.Unturned.Provider._client = SDG.Unturned.Provider.user;
//                    SDG.Unturned.Provider._clientHash = Hash.SHA1(SDG.Unturned.Provider.client);
//                }
//                SDG.Unturned.Provider._isServer = false;
//                SDG.Unturned.Provider._isClient = false;
//            }
//            else if (SDG.Unturned.Provider.isClient)
//            {
//                if (SDG.Unturned.Provider.battlEyeClientHandle != IntPtr.Zero)
//                {
//                    if (SDG.Unturned.Provider.battlEyeClientRunData != null && SDG.Unturned.Provider.battlEyeClientRunData.pfnExit != null)
//                    {
//                        int num = SDG.Unturned.Provider.battlEyeClientRunData.pfnExit() ? 1 : 0;
//                    }
//                    BEClient.FreeLibrary(SDG.Unturned.Provider.battlEyeClientHandle);
//                    SDG.Unturned.Provider.battlEyeClientHandle = IntPtr.Zero;
//                }
//                SteamNetworking.CloseP2PSessionWithUser(SDG.Unturned.Provider.server);
//                for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//                    SteamNetworking.CloseP2PSessionWithUser(SDG.Unturned.Provider.clients[index].playerID.steamID);
//                SteamFriends.SetRichPresence("connect", string.Empty);
//                Lobbies.leaveLobby();
//                SDG.Unturned.Provider.closeTicket();
//                SteamUser.AdvertiseGame(CSteamID.Nil, 0U, (ushort)0);
//                SDG.Unturned.Provider._server = new CSteamID();
//                SDG.Unturned.Provider._isServer = false;
//                SDG.Unturned.Provider._isClient = false;
//            }
//            if (SDG.Unturned.Provider.onClientDisconnected != null)
//                SDG.Unturned.Provider.onClientDisconnected();
//            Level.exit();
//            SDG.Unturned.Provider.unloadGameMode();
//            SDG.Unturned.Provider._isConnected = false;
//            SDG.Unturned.Provider.isWaitingForWorkshopResponse = false;
//            SDG.Unturned.Provider.isLoadingUGC = false;
//            SDG.Unturned.Provider.isLoadingInventory = true;
//        }

//        public static void sendGUIDTable(SteamPending player)
//        {
//            SDG.Unturned.Provider.accept(player);
//        }

//        private static void handleServerReady()
//        {
//            if (SDG.Unturned.Provider.isServerConnectedToSteam)
//                return;
//            SDG.Unturned.Provider.isServerConnectedToSteam = true;
//            CommandWindow.Log((object)"Steam servers ready!");
//            List<ulong> instance;
//            if (ServerSavedata.fileExists("/WorkshopDownloadIDs.json"))
//            {
//                try
//                {
//                    instance = ServerSavedata.deserializeJSON<List<ulong>>("/WorkshopDownloadIDs.json");
//                }
//                catch
//                {
//                    instance = (List<ulong>)null;
//                }
//                if (instance == null)
//                    instance = new List<ulong>();
//            }
//            else
//                instance = new List<ulong>();
//            ServerSavedata.serializeJSON<List<ulong>>("/WorkshopDownloadIDs.json", instance);
//            DedicatedUGC.initialize();
//            foreach (ulong id in instance)
//                DedicatedUGC.registerItemInstallation(id);
//            DedicatedUGC.beginInstallingItems();
//        }

//        public static string getModeTagAbbreviation(EGameMode gm)
//        {
//            switch (gm)
//            {
//                case EGameMode.EASY:
//                    return "EZY";
//                case EGameMode.NORMAL:
//                    return "NRM";
//                case EGameMode.HARD:
//                    return "HRD";
//                default:
//                    return (string)null;
//            }
//        }

//        public static string getCameraModeTagAbbreviation(ECameraMode cm)
//        {
//            switch (cm)
//            {
//                case ECameraMode.FIRST:
//                    return "1Pp";
//                case ECameraMode.THIRD:
//                    return "3Pp";
//                case ECameraMode.BOTH:
//                    return "2Pp";
//                case ECameraMode.VEHICLE:
//                    return "4Pp";
//                default:
//                    return (string)null;
//            }
//        }

//        private static void onDedicatedUGCInstalled()
//        {
//            if (SDG.Unturned.Provider.isDedicatedUGCInstalled)
//                return;
//            SDG.Unturned.Provider.isDedicatedUGCInstalled = true;
//            SDG.Unturned.Provider.apiWarningMessageHook = new SteamAPIWarningMessageHook_t(SDG.Unturned.Provider.onAPIWarningMessage);
//            SteamGameServerUtils.SetWarningMessageHook(SDG.Unturned.Provider.apiWarningMessageHook);
//            SDG.Unturned.Provider._time = SteamGameServerUtils.GetServerRealTime();
//            if (!Level.exists(SDG.Unturned.Provider.map))
//            {
//                string map = SDG.Unturned.Provider.map;
//                SDG.Unturned.Provider.map = "PEI";
//                CommandWindow.LogError((object)SDG.Unturned.Provider.localization.format("Map_Missing", (object)map, (object)SDG.Unturned.Provider.map));
//            }
//            Level.load(Level.getLevel(SDG.Unturned.Provider.map), true);
//            SDG.Unturned.Provider.loadGameMode();
//            SDG.Unturned.Provider.applyLevelModeConfigOverrides();
//            SteamGameServer.SetMaxPlayerCount((int)SDG.Unturned.Provider.maxPlayers);
//            SteamGameServer.SetServerName(SDG.Unturned.Provider.serverName);
//            SteamGameServer.SetPasswordProtected(SDG.Unturned.Provider.serverPassword != string.Empty);
//            SteamGameServer.SetMapName(SDG.Unturned.Provider.map);
//            if (Dedicator.isDedicated)
//            {
//                if (!ReadWrite.folderExists("/Bundles/Workshop/Content", true))
//                    ReadWrite.createFolder("/Bundles/Workshop/Content", true);
//                string path1 = "/Bundles/Workshop/Content";
//                foreach (string folder in ReadWrite.getFolders(path1))
//                {
//                    string s = ReadWrite.folderName(folder);
//                    ulong result;
//                    if (ulong.TryParse(s, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result))
//                    {
//                        SDG.Unturned.Provider.registerServerUsingWorkshopFileId(result);
//                        CommandWindow.Log((object)("Recommended to add workshop item " + (object)result + " to WorkshopDownloadIDs.json and remove it from " + path1));
//                    }
//                    else
//                        CommandWindow.LogWarning((object)("Invalid workshop item '" + s + "' in " + path1));
//                }
//                string path2 = ServerSavedata.directory + "/" + SDG.Unturned.Provider.serverID + "/Workshop/Content";
//                if (!ReadWrite.folderExists(path2, true))
//                    ReadWrite.createFolder(path2, true);
//                foreach (string folder in ReadWrite.getFolders(path2))
//                {
//                    string s = ReadWrite.folderName(folder);
//                    ulong result;
//                    if (ulong.TryParse(s, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result))
//                    {
//                        SDG.Unturned.Provider.registerServerUsingWorkshopFileId(result);
//                        CommandWindow.Log((object)("Recommended to add workshop item " + (object)result + " to WorkshopDownloadIDs.json and remove it from " + path2));
//                    }
//                    else
//                        CommandWindow.LogWarning((object)("Invalid workshop item '" + s + "' in " + path2));
//                }
//                ulong result1;
//                if (ulong.TryParse(new DirectoryInfo(Level.info.path).Parent.Name, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result1))
//                    SDG.Unturned.Provider.registerServerUsingWorkshopFileId(result1);
//                SteamGameServer.SetGameData((!(SDG.Unturned.Provider.serverPassword != string.Empty) ? "SSAP" : "PASS") + "," + (!SDG.Unturned.Provider.configData.Server.VAC_Secure ? "VAC_OFF" : "VAC_ON") + "," + SDG.Unturned.Provider.APP_VERSION);
//                int num1 = 128;
//                string pchGameTags = (!SDG.Unturned.Provider.isPvP ? (object)"PVE" : (object)"PVP").ToString() + ",<gm>" + SDG.Unturned.Provider.gameMode.GetType().Name + "</gm>," + (!SDG.Unturned.Provider.hasCheats ? (object)"CHn" : (object)"CHy") + (object)',' + SDG.Unturned.Provider.getModeTagAbbreviation(SDG.Unturned.Provider.mode) + "," + SDG.Unturned.Provider.getCameraModeTagAbbreviation(SDG.Unturned.Provider.cameraMode) + "," + (SDG.Unturned.Provider.getServerWorkshopFileIDs().Count <= 0 ? (object)"WSn" : (object)"WSy") + "," + (!SDG.Unturned.Provider.isGold ? (object)"F2P" : (object)"GLD") + "," + (!SDG.Unturned.Provider.configData.Server.BattlEye_Secure ? "BEn" : "BEy");
//                if (!string.IsNullOrEmpty(SDG.Unturned.Provider.configData.Browser.Thumbnail))
//                    pchGameTags = pchGameTags + ",<tn>" + SDG.Unturned.Provider.configData.Browser.Thumbnail + "</tn>";
//                if (pchGameTags.Length > num1)
//                {
//                    CommandWindow.LogWarning((object)("Server browser thumbnail URL is " + (object)(pchGameTags.Length - num1) + " characters over budget!"));
//                    CommandWindow.LogWarning((object)"Server will not list properly until this URL is adjusted!");
//                }
//                SteamGameServer.SetGameTags(pchGameTags);
//                int num2 = 64;
//                if (SDG.Unturned.Provider.configData.Browser.Desc_Server_List.Length > num2)
//                    CommandWindow.LogWarning((object)("Server browser description is " + (object)(SDG.Unturned.Provider.configData.Browser.Desc_Server_List.Length - num2) + " characters over budget!"));
//                SteamGameServer.SetGameDescription(SDG.Unturned.Provider.configData.Browser.Desc_Server_List);
//                SteamGameServer.SetKeyValue("Browser_Icon", SDG.Unturned.Provider.configData.Browser.Icon);
//                SteamGameServer.SetKeyValue("Browser_Desc_Hint", SDG.Unturned.Provider.configData.Browser.Desc_Hint);
//                int num3 = (SDG.Unturned.Provider.configData.Browser.Desc_Full.Length - 1) / 120 + 1;
//                int num4 = 0;
//                SteamGameServer.SetKeyValue("Browser_Desc_Full_Count", num3.ToString());
//                for (int startIndex = 0; startIndex < SDG.Unturned.Provider.configData.Browser.Desc_Full.Length; startIndex += 120)
//                {
//                    int length = 120;
//                    if (startIndex + length > SDG.Unturned.Provider.configData.Browser.Desc_Full.Length)
//                        length = SDG.Unturned.Provider.configData.Browser.Desc_Full.Length - startIndex;
//                    string pValue = SDG.Unturned.Provider.configData.Browser.Desc_Full.Substring(startIndex, length);
//                    SteamGameServer.SetKeyValue("Browser_Desc_Full_Line_" + (object)num4, pValue);
//                    ++num4;
//                }
//                if (SDG.Unturned.Provider.getServerWorkshopFileIDs().Count > 0)
//                {
//                    string empty = string.Empty;
//                    for (int index = 0; index < SDG.Unturned.Provider.getServerWorkshopFileIDs().Count; ++index)
//                    {
//                        if (empty.Length > 0)
//                            empty += (string)(object)',';
//                        empty += (string)(object)SDG.Unturned.Provider.getServerWorkshopFileIDs()[index];
//                    }
//                    int num5 = (empty.Length - 1) / 120 + 1;
//                    int num6 = 0;
//                    SteamGameServer.SetKeyValue("Browser_Workshop_Count", num5.ToString());
//                    for (int startIndex = 0; startIndex < empty.Length; startIndex += 120)
//                    {
//                        int length = 120;
//                        if (startIndex + length > empty.Length)
//                            length = empty.Length - startIndex;
//                        string pValue = empty.Substring(startIndex, length);
//                        SteamGameServer.SetKeyValue("Browser_Workshop_Line_" + (object)num6, pValue);
//                        ++num6;
//                    }
//                }
//                string str = string.Empty;
//                foreach (FieldInfo field1 in SDG.Unturned.Provider.modeConfigData.GetType().GetFields())
//                {
//                    object obj1 = field1.GetValue((object)SDG.Unturned.Provider.modeConfigData);
//                    foreach (FieldInfo field2 in obj1.GetType().GetFields())
//                    {
//                        object obj2 = field2.GetValue(obj1);
//                        if (str.Length > 0)
//                            str += (string)(object)',';
//                        str = !(obj2 is bool) ? str + obj2 : str + (!(bool)obj2 ? "F" : "T");
//                    }
//                }
//                int num7 = (str.Length - 1) / 120 + 1;
//                int num8 = 0;
//                SteamGameServer.SetKeyValue("Browser_Config_Count", num7.ToString());
//                for (int startIndex = 0; startIndex < str.Length; startIndex += 120)
//                {
//                    int length = 120;
//                    if (startIndex + length > str.Length)
//                        length = str.Length - startIndex;
//                    string pValue = str.Substring(startIndex, length);
//                    SteamGameServer.SetKeyValue("Browser_Config_Line_" + (object)num8, pValue);
//                    ++num8;
//                }
//            }
//            SDG.Unturned.Provider._server = SteamGameServer.GetSteamID();
//            SDG.Unturned.Provider._client = SDG.Unturned.Provider._server;
//            SDG.Unturned.Provider._clientHash = Hash.SHA1(SDG.Unturned.Provider.client);
//            if (Dedicator.isDedicated)
//                SDG.Unturned.Provider._clientName = SDG.Unturned.Provider.localization.format("Console");
//            SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.lastReceivedServersideTime = 0.0f;
//        }

//        public static void send(
//          CSteamID steamID,
//          ESteamPacket type,
//          byte[] packet,
//          int size,
//          int channel)
//        {
//            if (SDG.Unturned.Provider.onServerWritingPacket != null)
//            {
//                try
//                {
//                    SDG.Unturned.Provider.onServerWritingPacket(steamID, type, packet, size, channel);
//                }
//                catch (Exception ex)
//                {
//                    UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerWritingPacket:");
//                    UnityEngine.Debug.LogException(ex);
//                }
//            }
//            if (!SDG.Unturned.Provider.isConnected)
//                return;
//            SDG.Unturned.Provider._bytesSent += (uint)size;
//            ++SDG.Unturned.Provider._packetsSent;
//            if (SDG.Unturned.Provider.isServer)
//            {
//                if (steamID == SDG.Unturned.Provider.server || SDG.Unturned.Provider.isClient && steamID == SDG.Unturned.Provider.client)
//                    SDG.Unturned.Provider.receiveServer(SDG.Unturned.Provider.server, packet, 0, size, channel);
//                else if (steamID.m_SteamID == 0UL)
//                {
//                    UnityEngine.Debug.LogError((object)"Failed to send to invalid steam ID.");
//                }
//                else
//                {
//                    if (SDG.Unturned.Provider.shouldNetIgnoreSteamId(steamID))
//                        return;
//                    if (SDG.Unturned.Provider.isUnreliable(type))
//                    {
//                        if (SteamGameServerNetworking.SendP2PPacket(steamID, packet, (uint)size, !SDG.Unturned.Provider.isInstant(type) ? EP2PSend.k_EP2PSendUnreliable : EP2PSend.k_EP2PSendUnreliableNoDelay, channel))
//                            return;
//                        UnityEngine.Debug.LogError((object)("Failed to send size " + (object)size + " unreliable packet to " + (object)steamID + "!"));
//                    }
//                    else
//                    {
//                        if (SteamGameServerNetworking.SendP2PPacket(steamID, packet, (uint)size, !SDG.Unturned.Provider.isInstant(type) ? EP2PSend.k_EP2PSendReliableWithBuffering : EP2PSend.k_EP2PSendReliable, channel))
//                            return;
//                        UnityEngine.Debug.LogError((object)("Failed to send size " + (object)size + " reliable packet to " + (object)steamID + "!"));
//                    }
//                }
//            }
//            else if (steamID == SDG.Unturned.Provider.client)
//                SDG.Unturned.Provider.receiveClient(SDG.Unturned.Provider.client, packet, 0, size, channel);
//            else if (steamID.m_SteamID == 0UL)
//            {
//                UnityEngine.Debug.LogError((object)"Failed to send to invalid steam ID.");
//            }
//            else
//            {
//                if (SDG.Unturned.Provider.shouldNetIgnoreSteamId(steamID))
//                    return;
//                if (SDG.Unturned.Provider.isUnreliable(type))
//                {
//                    if (SteamNetworking.SendP2PPacket(steamID, packet, (uint)size, !SDG.Unturned.Provider.isInstant(type) ? EP2PSend.k_EP2PSendUnreliable : EP2PSend.k_EP2PSendUnreliableNoDelay, channel))
//                        return;
//                    UnityEngine.Debug.LogError((object)("Failed to send size " + (object)size + " unreliable packet to " + (object)steamID + "!"));
//                }
//                else
//                {
//                    if (SteamNetworking.SendP2PPacket(steamID, packet, (uint)size, !SDG.Unturned.Provider.isInstant(type) ? EP2PSend.k_EP2PSendReliableWithBuffering : EP2PSend.k_EP2PSendReliable, channel))
//                        return;
//                    UnityEngine.Debug.LogError((object)("Failed to send size " + (object)size + " reliable packet to " + (object)steamID + "!"));
//                }
//            }
//        }

//        private static bool isValidPacketIndex(byte index)
//        {
//            return index < (byte)26;
//        }

//        public static bool shouldNetIgnoreSteamId(CSteamID id)
//        {
//            return SDG.Unturned.Provider.netIgnoredSteamIDs.Contains(id);
//        }

//        public static void refuseGarbageConnection(CSteamID remoteId, string reason)
//        {
//            UnityEngine.Debug.Log((object)("Refusing connections from " + (object)remoteId + " (" + reason + ")"));
//            if (SDG.Unturned.Provider.isServer)
//                SteamGameServerNetworking.CloseP2PSessionWithUser(remoteId);
//            else
//                SteamNetworking.CloseP2PSessionWithUser(remoteId);
//            SDG.Unturned.Provider.netIgnoredSteamIDs.Add(remoteId);
//        }

//        private static void receiveServer(
//          CSteamID steamID,
//          byte[] packet,
//          int offset,
//          int size,
//          int channel)
//        {
//            SDG.Unturned.Provider._bytesReceived += (uint)size;
//            ++SDG.Unturned.Provider._packetsReceived;
//            if (!Dedicator.isDedicated)
//                return;
//            byte index1 = packet[offset];
//            if (!SDG.Unturned.Provider.isValidPacketIndex(index1))
//            {
//                UnityEngine.Debug.LogWarning((object)("Received invalid packet index from " + (object)steamID + ", so we're refusing them"));
//                SDG.Unturned.Provider.refuseGarbageConnection(steamID, "sv invalid packet index");
//            }
//            else
//            {
//                if (SDG.Unturned.Provider.onServerReadingPacket != null)
//                {
//                    try
//                    {
//                        SDG.Unturned.Provider.onServerReadingPacket(steamID, packet, offset, size, channel);
//                    }
//                    catch (Exception ex)
//                    {
//                        UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerReadingPacket:");
//                        UnityEngine.Debug.LogException(ex);
//                    }
//                }
//                ESteamPacket packet1 = (ESteamPacket)index1;
//                if (SDG.Unturned.Provider.isUpdate(packet1))
//                {
//                    if (steamID == SDG.Unturned.Provider.server)
//                    {
//                        for (int index2 = 0; index2 < SDG.Unturned.Provider.receivers.Count; ++index2)
//                        {
//                            if (SDG.Unturned.Provider.receivers[index2].id == channel)
//                            {
//                                if (!SDG.Unturned.Provider.receivers[index2].receive(steamID, packet, offset, size))
//                                    break;
//                                break;
//                            }
//                        }
//                    }
//                    else
//                    {
//                        for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                        {
//                            if (SDG.Unturned.Provider.clients[index2].playerID.steamID == steamID)
//                            {
//                                SDG.Unturned.Provider.clients[index2].rpcCredits += 1f / SDG.Unturned.Provider.configData.Server.Max_Packets_Per_Second;
//                                for (int index3 = 0; index3 < SDG.Unturned.Provider.receivers.Count; ++index3)
//                                {
//                                    if (SDG.Unturned.Provider.receivers[index3].id == channel)
//                                    {
//                                        if (SDG.Unturned.Provider.receivers[index3].receive(steamID, packet, offset, size))
//                                            break;
//                                        SDG.Unturned.Provider.refuseGarbageConnection(steamID, "sv channel receive failed");
//                                        break;
//                                    }
//                                }
//                                break;
//                            }
//                        }
//                    }
//                }
//                else
//                {
//                    switch (packet1)
//                    {
//                        case ESteamPacket.WORKSHOP:
//                            bool flag1 = false;
//                            foreach (SDG.Unturned.Provider.WorkshopRequestLog workshopRequest in SDG.Unturned.Provider.workshopRequests)
//                            {
//                                if (workshopRequest.sender == steamID)
//                                {
//                                    bool flag2 = (double)Time.realtimeSinceStartup - (double)workshopRequest.realTime < 30.0;
//                                    workshopRequest.realTime = Time.realtimeSinceStartup;
//                                    if (flag2)
//                                        return;
//                                    flag1 = true;
//                                    break;
//                                }
//                            }
//                            if (!flag1)
//                                SDG.Unturned.Provider.workshopRequests.Add(new SDG.Unturned.Provider.WorkshopRequestLog()
//                                {
//                                    sender = steamID,
//                                    realTime = Time.realtimeSinceStartup
//                                });
//                            byte[] packet2 = new byte[2 + SDG.Unturned.Provider.getServerWorkshopFileIDs().Count * 8];
//                            packet2[0] = (byte)1;
//                            packet2[1] = (byte)SDG.Unturned.Provider.getServerWorkshopFileIDs().Count;
//                            for (byte index2 = 0; (int)index2 < SDG.Unturned.Provider.getServerWorkshopFileIDs().Count; ++index2)
//                                BitConverter.GetBytes(SDG.Unturned.Provider.getServerWorkshopFileIDs()[(int)index2]).CopyTo((Array)packet2, 2 + (int)index2 * 8);
//                            SDG.Unturned.Provider.send(steamID, ESteamPacket.WORKSHOP, packet2, packet2.Length, 0);
//                            break;
//                        case ESteamPacket.CONNECT:
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.pending.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.pending[index2].playerID.steamID == steamID)
//                                {
//                                    SDG.Unturned.Provider.reject(steamID, ESteamRejection.ALREADY_PENDING);
//                                    return;
//                                }
//                            }
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.clients[index2].playerID.steamID == steamID)
//                                {
//                                    SDG.Unturned.Provider.reject(steamID, ESteamRejection.ALREADY_CONNECTED);
//                                    return;
//                                }
//                            }
//                            object[] objects = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE, Types.STRING_TYPE, Types.BYTE_ARRAY_TYPE, Types.BYTE_ARRAY_TYPE, Types.BYTE_ARRAY_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE, Types.BOOLEAN_TYPE, Types.SINGLE_TYPE, Types.STRING_TYPE, Types.STEAM_ID_TYPE, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.COLOR_TYPE, Types.COLOR_TYPE, Types.COLOR_TYPE, Types.BOOLEAN_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_TYPE, Types.UINT64_ARRAY_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE, Types.STRING_TYPE, Types.STEAM_ID_TYPE);
//                            SteamPlayerID newPlayerID = new SteamPlayerID(steamID, (byte)objects[1], (string)objects[2], (string)objects[3], (string)objects[11], (CSteamID)objects[12]);
//                            if ((string)objects[8] != SDG.Unturned.Provider.APP_VERSION)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_VERSION);
//                                break;
//                            }
//                            if (newPlayerID.playerName.Length < 2)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_SHORT);
//                                break;
//                            }
//                            if (newPlayerID.characterName.Length < 2)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_SHORT);
//                                break;
//                            }
//                            if (newPlayerID.playerName.Length > 32)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_LONG);
//                                break;
//                            }
//                            if (newPlayerID.characterName.Length > 32)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_LONG);
//                                break;
//                            }
//                            long result1;
//                            double result2;
//                            if (long.TryParse(newPlayerID.playerName, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result1) || double.TryParse(newPlayerID.playerName, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result2))
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_NUMBER);
//                                break;
//                            }
//                            long result3;
//                            double result4;
//                            if (long.TryParse(newPlayerID.characterName, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result3) || double.TryParse(newPlayerID.characterName, NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result4))
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_NUMBER);
//                                break;
//                            }
//                            if (SDG.Unturned.Provider.filterName)
//                            {
//                                if (!NameTool.isValid(newPlayerID.playerName))
//                                {
//                                    SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_INVALID);
//                                    break;
//                                }
//                                if (!NameTool.isValid(newPlayerID.characterName))
//                                {
//                                    SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_INVALID);
//                                    break;
//                                }
//                            }
//                            if (NameTool.containsRichText(newPlayerID.playerName))
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_PLAYER_INVALID);
//                                break;
//                            }
//                            if (NameTool.containsRichText(newPlayerID.characterName))
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NAME_CHARACTER_INVALID);
//                                break;
//                            }
//                            P2PSessionState_t pConnectionState;
//                            uint remoteIP = !SteamGameServerNetworking.GetP2PSessionState(steamID, out pConnectionState) ? 0U : pConnectionState.m_nRemoteIP;
//                            bool isBanned;
//                            string banReason;
//                            uint banRemainingDuration;
//                            SDG.Unturned.Provider.checkBanStatus(steamID, remoteIP, out isBanned, out banReason, out banRemainingDuration);
//                            if (isBanned)
//                            {
//                                int size1;
//                                byte[] bytes = SteamPacker.getBytes(0, out size1, (object)(byte)9, (object)banReason, (object)banRemainingDuration);
//                                SDG.Unturned.Provider.send(steamID, ESteamPacket.BANNED, bytes, size1, 0);
//                                break;
//                            }
//                            bool flag3 = SteamWhitelist.checkWhitelisted(steamID);
//                            if (SDG.Unturned.Provider.isWhitelisted && !flag3)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.WHITELISTED);
//                                break;
//                            }
//                            if (SDG.Unturned.Provider.clients.Count + 1 > (int)SDG.Unturned.Provider.maxPlayers && SDG.Unturned.Provider.pending.Count + 1 > (int)SDG.Unturned.Provider.queueSize)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.SERVER_FULL);
//                                break;
//                            }
//                            byte[] hash_0_1 = (byte[])objects[4];
//                            if (hash_0_1.Length != 20)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_PASSWORD);
//                                break;
//                            }
//                            byte[] hash_0_2 = (byte[])objects[5];
//                            if (hash_0_2.Length != 20)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_LEVEL);
//                                break;
//                            }
//                            byte[] h = (byte[])objects[6];
//                            if (h.Length != 20)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_ASSEMBLY);
//                                break;
//                            }
//                            string str1 = (string)objects[29];
//                            ModuleDependency[] moduleDependencyArray;
//                            if (string.IsNullOrEmpty(str1))
//                            {
//                                moduleDependencyArray = new ModuleDependency[0];
//                            }
//                            else
//                            {
//                                string[] strArray1 = str1.Split(';');
//                                moduleDependencyArray = new ModuleDependency[strArray1.Length];
//                                for (int index2 = 0; index2 < moduleDependencyArray.Length; ++index2)
//                                {
//                                    string[] strArray2 = strArray1[index2].Split(',');
//                                    if (strArray2.Length == 2)
//                                    {
//                                        moduleDependencyArray[index2] = new ModuleDependency();
//                                        moduleDependencyArray[index2].Name = strArray2[0];
//                                        uint.TryParse(strArray2[1], NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out moduleDependencyArray[index2].Version_Internal);
//                                    }
//                                }
//                            }
//                            SDG.Unturned.Provider.critMods.Clear();
//                            ModuleHook.getRequiredModules(SDG.Unturned.Provider.critMods);
//                            bool flag4 = true;
//                            for (int index2 = 0; index2 < moduleDependencyArray.Length; ++index2)
//                            {
//                                bool flag2 = false;
//                                if (moduleDependencyArray[index2] != null)
//                                {
//                                    for (int index3 = 0; index3 < SDG.Unturned.Provider.critMods.Count; ++index3)
//                                    {
//                                        if (SDG.Unturned.Provider.critMods[index3] != null && SDG.Unturned.Provider.critMods[index3].config != null && (SDG.Unturned.Provider.critMods[index3].config.Name == moduleDependencyArray[index2].Name && SDG.Unturned.Provider.critMods[index3].config.Version_Internal >= moduleDependencyArray[index2].Version_Internal))
//                                        {
//                                            flag2 = true;
//                                            break;
//                                        }
//                                    }
//                                }
//                                if (!flag2)
//                                {
//                                    flag4 = false;
//                                    break;
//                                }
//                            }
//                            if (!flag4)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.CLIENT_MODULE_DESYNC);
//                                break;
//                            }
//                            bool flag5 = true;
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.critMods.Count; ++index2)
//                            {
//                                bool flag2 = false;
//                                if (SDG.Unturned.Provider.critMods[index2] != null && SDG.Unturned.Provider.critMods[index2].config != null)
//                                {
//                                    for (int index3 = 0; index3 < moduleDependencyArray.Length; ++index3)
//                                    {
//                                        if (moduleDependencyArray[index3] != null && moduleDependencyArray[index3].Name == SDG.Unturned.Provider.critMods[index2].config.Name && moduleDependencyArray[index3].Version_Internal >= SDG.Unturned.Provider.critMods[index2].config.Version_Internal)
//                                        {
//                                            flag2 = true;
//                                            break;
//                                        }
//                                    }
//                                }
//                                if (!flag2)
//                                {
//                                    flag5 = false;
//                                    break;
//                                }
//                            }
//                            if (!flag5)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.SERVER_MODULE_DESYNC);
//                                break;
//                            }
//                            if (SDG.Unturned.Provider.serverPassword == string.Empty || Hash.verifyHash(hash_0_1, SDG.Unturned.Provider._serverPasswordHash))
//                            {
//                                if (Hash.verifyHash(hash_0_2, Level.hash))
//                                {
//                                    if (ReadWrite.appIn(h, (byte)objects[7]))
//                                    {
//                                        if ((double)(float)objects[10] < (double)SDG.Unturned.Provider.configData.Server.Max_Ping_Milliseconds / 1000.0)
//                                        {
//                                            if (!SDG.Unturned.Provider.isWhitelisted && flag3)
//                                            {
//                                                if (SDG.Unturned.Provider.pending.Count == 0)
//                                                {
//                                                    SDG.Unturned.Provider.pending.Add(new SteamPending(newPlayerID, (bool)objects[9], (byte)objects[13], (byte)objects[14], (byte)objects[15], (Color)objects[16], (Color)objects[17], (Color)objects[18], (bool)objects[19], (ulong)objects[20], (ulong)objects[21], (ulong)objects[22], (ulong)objects[23], (ulong)objects[24], (ulong)objects[25], (ulong)objects[26], (ulong[])objects[27], (EPlayerSkillset)(byte)objects[28], (string)objects[30], (CSteamID)objects[31]));
//                                                    SDG.Unturned.Provider.verifyNextPlayerInQueue();
//                                                    break;
//                                                }
//                                                SDG.Unturned.Provider.pending.Insert(1, new SteamPending(newPlayerID, (bool)objects[9], (byte)objects[13], (byte)objects[14], (byte)objects[15], (Color)objects[16], (Color)objects[17], (Color)objects[18], (bool)objects[19], (ulong)objects[20], (ulong)objects[21], (ulong)objects[22], (ulong)objects[23], (ulong)objects[24], (ulong)objects[25], (ulong)objects[26], (ulong[])objects[27], (EPlayerSkillset)(byte)objects[28], (string)objects[30], (CSteamID)objects[31]));
//                                                break;
//                                            }
//                                            SDG.Unturned.Provider.pending.Add(new SteamPending(newPlayerID, (bool)objects[9], (byte)objects[13], (byte)objects[14], (byte)objects[15], (Color)objects[16], (Color)objects[17], (Color)objects[18], (bool)objects[19], (ulong)objects[20], (ulong)objects[21], (ulong)objects[22], (ulong)objects[23], (ulong)objects[24], (ulong)objects[25], (ulong)objects[26], (ulong[])objects[27], (EPlayerSkillset)(byte)objects[28], (string)objects[30], (CSteamID)objects[31]));
//                                            if (SDG.Unturned.Provider.pending.Count != 1)
//                                                break;
//                                            SDG.Unturned.Provider.verifyNextPlayerInQueue();
//                                            break;
//                                        }
//                                        SDG.Unturned.Provider.reject(steamID, ESteamRejection.PING);
//                                        break;
//                                    }
//                                    SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_ASSEMBLY);
//                                    break;
//                                }
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_HASH_LEVEL);
//                                break;
//                            }
//                            SDG.Unturned.Provider.reject(steamID, ESteamRejection.WRONG_PASSWORD);
//                            break;
//                        case ESteamPacket.AUTHENTICATE:
//                            SteamPending steamPending = (SteamPending)null;
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.pending.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.pending[index2].playerID.steamID == steamID)
//                                {
//                                    steamPending = SDG.Unturned.Provider.pending[index2];
//                                    break;
//                                }
//                            }
//                            if (steamPending == null)
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.NOT_PENDING);
//                                break;
//                            }
//                            ushort uint16_1 = BitConverter.ToUInt16(packet, 1);
//                            byte[] ticket = new byte[(int)uint16_1];
//                            Buffer.BlockCopy((Array)packet, 3, (Array)ticket, 0, (int)uint16_1);
//                            ushort uint16_2 = BitConverter.ToUInt16(packet, 3 + (int)uint16_1);
//                            byte[] bytes1 = new byte[(int)uint16_2];
//                            Buffer.BlockCopy((Array)packet, 5 + (int)uint16_1, (Array)bytes1, 0, (int)uint16_2);
//                            if (!SDG.Unturned.Provider.verifyTicket(steamID, ticket))
//                            {
//                                SDG.Unturned.Provider.reject(steamID, ESteamRejection.AUTH_VERIFICATION);
//                                break;
//                            }
//                            if (steamPending.playerID.group == CSteamID.Nil)
//                                steamPending.hasGroup = true;
//                            else if (!SteamGameServer.RequestUserGroupStatus(steamPending.playerID.steamID, steamPending.playerID.group))
//                            {
//                                steamPending.playerID.group = CSteamID.Nil;
//                                steamPending.hasGroup = true;
//                            }
//                            List<SteamItemDetails_t> steamItemDetailsTList = new List<SteamItemDetails_t>();
//                            Dictionary<ulong, DynamicEconDetails> dictionary = new Dictionary<ulong, DynamicEconDetails>();
//                            int startIndex1 = 0;
//                            while (startIndex1 + 4 <= (int)uint16_2)
//                            {
//                                int int32 = BitConverter.ToInt32(bytes1, startIndex1);
//                                int startIndex2 = startIndex1 + 4;
//                                if (startIndex2 + 8 <= (int)uint16_2)
//                                {
//                                    ulong uint64 = BitConverter.ToUInt64(bytes1, startIndex2);
//                                    int startIndex3 = startIndex2 + 8;
//                                    if (startIndex3 + 2 <= (int)uint16_2)
//                                    {
//                                        ushort uint16_3 = BitConverter.ToUInt16(bytes1, startIndex3);
//                                        int index2 = startIndex3 + 2;
//                                        if (index2 + (int)uint16_3 <= (int)uint16_2)
//                                        {
//                                            string str2 = Encoding.UTF8.GetString(bytes1, index2, (int)uint16_3);
//                                            int startIndex4 = index2 + (int)uint16_3;
//                                            if (startIndex4 + 2 <= (int)uint16_2)
//                                            {
//                                                ushort uint16_4 = BitConverter.ToUInt16(bytes1, startIndex4);
//                                                int index3 = startIndex4 + 2;
//                                                if (index3 + (int)uint16_4 <= (int)uint16_2)
//                                                {
//                                                    string str3 = Encoding.UTF8.GetString(bytes1, index3, (int)uint16_4);
//                                                    startIndex1 = index3 + (int)uint16_4;
//                                                    if (!dictionary.ContainsKey(uint64))
//                                                    {
//                                                        SteamItemDetails_t steamItemDetailsT = new SteamItemDetails_t();
//                                                        steamItemDetailsT.m_iDefinition.m_SteamItemDef = int32;
//                                                        steamItemDetailsT.m_itemId.m_SteamItemInstanceID = uint64;
//                                                        DynamicEconDetails dynamicEconDetails = new DynamicEconDetails();
//                                                        dynamicEconDetails.tags = str2;
//                                                        dynamicEconDetails.dynamic_props = str3;
//                                                        steamItemDetailsTList.Add(steamItemDetailsT);
//                                                        dictionary.Add(uint64, dynamicEconDetails);
//                                                    }
//                                                    else
//                                                        break;
//                                                }
//                                                else
//                                                    break;
//                                            }
//                                            else
//                                                break;
//                                        }
//                                        else
//                                            break;
//                                    }
//                                    else
//                                        break;
//                                }
//                                else
//                                    break;
//                            }
//                            steamPending.inventoryResult = SteamInventoryResult_t.Invalid;
//                            steamPending.inventoryDetails = steamItemDetailsTList.ToArray();
//                            steamPending.dynamicInventoryDetails = dictionary;
//                            steamPending.inventoryDetailsReady();
//                            break;
//                        case ESteamPacket.PING_REQUEST:
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.pending.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.pending[index2].playerID.steamID == steamID)
//                                {
//                                    if ((double)SDG.Unturned.Provider.pending[index2].averagePingRequestsReceivedPerSecond > (double)SDG.Unturned.Provider.PING_REQUEST_INTERVAL * 2.0)
//                                        return;
//                                    SDG.Unturned.Provider.pending[index2].lastReceivedPingRequestRealtime = Time.realtimeSinceStartup;
//                                    SDG.Unturned.Provider.pending[index2].incrementNumPingRequestsReceived();
//                                    int size1;
//                                    byte[] bytes2 = SteamPacker.getBytes(0, out size1, (object)(byte)14, (object)SDG.Unturned.Provider.clientPredictedServersideTime, (object)(byte)index2);
//                                    SDG.Unturned.Provider.send(steamID, ESteamPacket.PING_RESPONSE, bytes2, size1, 0);
//                                    return;
//                                }
//                            }
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.clients[index2].playerID.steamID == steamID)
//                                {
//                                    if ((double)SDG.Unturned.Provider.clients[index2].averagePingRequestsReceivedPerSecond > (double)SDG.Unturned.Provider.PING_REQUEST_INTERVAL * 2.0)
//                                        break;
//                                    SDG.Unturned.Provider.clients[index2].lastReceivedPingRequestRealtime = Time.realtimeSinceStartup;
//                                    SDG.Unturned.Provider.clients[index2].incrementNumPingRequestsReceived();
//                                    int size1;
//                                    byte[] bytes2 = SteamPacker.getBytes(0, out size1, (object)(byte)14, (object)SDG.Unturned.Provider.clientPredictedServersideTime);
//                                    SDG.Unturned.Provider.send(steamID, ESteamPacket.PING_RESPONSE, bytes2, size1, 0);
//                                    break;
//                                }
//                            }
//                            break;
//                        case ESteamPacket.PING_RESPONSE:
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.clients[index2].playerID.steamID == steamID)
//                                {
//                                    SDG.Unturned.Provider.clients[index2].rpcCredits += 1f / SDG.Unturned.Provider.configData.Server.Max_Packets_Per_Second;
//                                    if ((double)SDG.Unturned.Provider.clients[index2].timeLastPingRequestWasSentToClient <= 0.0)
//                                        break;
//                                    float deltaTime = Time.deltaTime;
//                                    SDG.Unturned.Provider.clients[index2].timeLastPacketWasReceivedFromClient = Time.realtimeSinceStartup;
//                                    SDG.Unturned.Provider.clients[index2].lag(Time.realtimeSinceStartup - SDG.Unturned.Provider.clients[index2].timeLastPingRequestWasSentToClient - deltaTime);
//                                    SDG.Unturned.Provider.clients[index2].timeLastPingRequestWasSentToClient = -1f;
//                                    break;
//                                }
//                            }
//                            break;
//                        case ESteamPacket.BATTLEYE:
//                            if (!(SDG.Unturned.Provider.battlEyeServerHandle != IntPtr.Zero) || SDG.Unturned.Provider.battlEyeServerRunData == null || SDG.Unturned.Provider.battlEyeServerRunData.pfnReceivedPacket == null)
//                                break;
//                            for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                            {
//                                if (SDG.Unturned.Provider.clients[index2].playerID.steamID == steamID)
//                                {
//                                    int nLength = size - offset - 1;
//                                    if (nLength > 0)
//                                    {
//                                        GCHandle gcHandle = GCHandle.Alloc((object)packet, GCHandleType.Pinned);
//                                        IntPtr pvPacket = gcHandle.AddrOfPinnedObject();
//                                        pvPacket = IntPtr.Size != 4 ? new IntPtr(pvPacket.ToInt64() + (long)offset + 1L) : new IntPtr(pvPacket.ToInt32() + offset + 1);
//                                        SDG.Unturned.Provider.battlEyeServerRunData.pfnReceivedPacket(SDG.Unturned.Provider.clients[index2].channel, pvPacket, nLength);
//                                        gcHandle.Free();
//                                        break;
//                                    }
//                                    UnityEngine.Debug.LogWarning((object)("Received empty BattlEye payload from " + (object)steamID + ", so we're refusing them"));
//                                    SDG.Unturned.Provider.refuseGarbageConnection(steamID, "sv empty BE payload");
//                                    break;
//                                }
//                            }
//                            break;
//                        default:
//                            UnityEngine.Debug.LogWarning((object)("Received server unhandled message " + (object)packet1 + " from " + (object)steamID + ", so we're refusing them"));
//                            SDG.Unturned.Provider.refuseGarbageConnection(steamID, "sv unhandled packet");
//                            break;
//                    }
//                }
//            }
//        }

//        private static void receiveClient(
//          CSteamID steamID,
//          byte[] packet,
//          int offset,
//          int size,
//          int channel)
//        {
//            SDG.Unturned.Provider._bytesReceived += (uint)size;
//            ++SDG.Unturned.Provider._packetsReceived;
//            byte index1 = packet[offset];
//            if (!SDG.Unturned.Provider.isValidPacketIndex(index1))
//            {
//                UnityEngine.Debug.LogWarning((object)("Received invalid packet index from " + (object)steamID + ", so we're refusing them"));
//                SDG.Unturned.Provider.refuseGarbageConnection(steamID, "cl invalid packet index");
//            }
//            else
//            {
//                ESteamPacket packet1 = (ESteamPacket)index1;
//                if (SDG.Unturned.Provider.isUpdate(packet1))
//                {
//                    for (int index2 = 0; index2 < SDG.Unturned.Provider.receivers.Count; ++index2)
//                    {
//                        if (SDG.Unturned.Provider.receivers[index2].id == channel)
//                        {
//                            if (SDG.Unturned.Provider.receivers[index2].receive(steamID, packet, offset, size) || !(steamID != SDG.Unturned.Provider.server))
//                                break;
//                            SDG.Unturned.Provider.refuseGarbageConnection(steamID, "cl channel receive failed");
//                            break;
//                        }
//                    }
//                }
//                else
//                {
//                    if (steamID == SDG.Unturned.Provider.server)
//                    {
//                        switch (packet1)
//                        {
//                            case ESteamPacket.SHUTDOWN:
//                                object[] objects1 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.STRING_TYPE);
//                                SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.SHUTDOWN;
//                                SDG.Unturned.Provider._connectionFailureReason = objects1[1] as string;
//                                SDG.Unturned.Provider.disconnect();
//                                return;
//                            case ESteamPacket.WORKSHOP:
//                                SDG.Unturned.Provider.isWaitingForWorkshopResponse = false;
//                                List<PublishedFileId_t> publishedFileIdTList = new List<PublishedFileId_t>();
//                                byte num1 = packet[offset + 1];
//                                for (byte index2 = 0; (int)index2 < (int)num1; ++index2)
//                                {
//                                    PublishedFileId_t publishedFileIdT = new PublishedFileId_t(BitConverter.ToUInt64(packet, offset + 2 + (int)index2 * 8));
//                                    publishedFileIdTList.Add(publishedFileIdT);
//                                }
//                                SDG.Unturned.Provider.CachedWorkshopResponse response = (SDG.Unturned.Provider.CachedWorkshopResponse)null;
//                                foreach (SDG.Unturned.Provider.CachedWorkshopResponse workshopResponse in SDG.Unturned.Provider.cachedWorkshopResponses)
//                                {
//                                    if (workshopResponse.server == SDG.Unturned.Provider.server)
//                                    {
//                                        workshopResponse.publishedFileIds = publishedFileIdTList;
//                                        response = workshopResponse;
//                                        break;
//                                    }
//                                }
//                                if (response == null)
//                                {
//                                    response = new SDG.Unturned.Provider.CachedWorkshopResponse();
//                                    response.server = SDG.Unturned.Provider.server;
//                                    response.publishedFileIds = publishedFileIdTList;
//                                    SDG.Unturned.Provider.cachedWorkshopResponses.Add(response);
//                                }
//                                response.realTime = Time.realtimeSinceStartup;
//                                SDG.Unturned.Provider.receiveWorkshopResponse(response);
//                                return;
//                            case ESteamPacket.VERIFY:
//                                byte[] numArray1 = SDG.Unturned.Provider.openTicket();
//                                if (numArray1 == null)
//                                {
//                                    SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_EMPTY;
//                                    SDG.Unturned.Provider.disconnect();
//                                    return;
//                                }
//                                byte[] numArray2;
//                                uint num2;
//                                if (SDG.Unturned.Provider.provider.economyService.wearingResult == SteamInventoryResult_t.Invalid)
//                                {
//                                    numArray2 = new byte[0];
//                                    num2 = 0U;
//                                }
//                                else
//                                {
//                                    uint punOutItemsArraySize = 0;
//                                    SteamItemDetails_t[] pOutItemsArray;
//                                    if (SteamInventory.GetResultItems(SDG.Unturned.Provider.provider.economyService.wearingResult, (SteamItemDetails_t[])null, ref punOutItemsArraySize) && punOutItemsArraySize > 0U)
//                                    {
//                                        pOutItemsArray = new SteamItemDetails_t[(IntPtr)punOutItemsArraySize];
//                                        SteamInventory.GetResultItems(SDG.Unturned.Provider.provider.economyService.wearingResult, pOutItemsArray, ref punOutItemsArraySize);
//                                    }
//                                    else
//                                        pOutItemsArray = new SteamItemDetails_t[(IntPtr)punOutItemsArraySize];
//                                    List<byte> byteList = new List<byte>();
//                                    for (uint unItemIndex = 0; (long)unItemIndex < (long)pOutItemsArray.Length; ++unItemIndex)
//                                    {
//                                        SteamItemDetails_t steamItemDetailsT = pOutItemsArray[(IntPtr)unItemIndex];
//                                        int steamItemDef = steamItemDetailsT.m_iDefinition.m_SteamItemDef;
//                                        ulong steamItemInstanceId = steamItemDetailsT.m_itemId.m_SteamItemInstanceID;
//                                        uint punValueBufferSizeOut1 = 1024;
//                                        string pchValueBuffer1;
//                                        if (!SteamInventory.GetResultItemProperty(SDG.Unturned.Provider.provider.economyService.wearingResult, unItemIndex, "tags", out pchValueBuffer1, ref punValueBufferSizeOut1) || punValueBufferSizeOut1 == 0U)
//                                            pchValueBuffer1 = string.Empty;
//                                        uint punValueBufferSizeOut2 = 1024;
//                                        string pchValueBuffer2;
//                                        if (!SteamInventory.GetResultItemProperty(SDG.Unturned.Provider.provider.economyService.wearingResult, unItemIndex, "dynamic_props", out pchValueBuffer2, ref punValueBufferSizeOut2) || punValueBufferSizeOut2 == 0U)
//                                            pchValueBuffer2 = string.Empty;
//                                        byte[] bytes1 = BitConverter.GetBytes(steamItemDef);
//                                        byte[] bytes2 = BitConverter.GetBytes(steamItemInstanceId);
//                                        byte[] bytes3 = Encoding.UTF8.GetBytes(pchValueBuffer1);
//                                        byte[] bytes4 = BitConverter.GetBytes((ushort)bytes3.Length);
//                                        byte[] bytes5 = Encoding.UTF8.GetBytes(pchValueBuffer2);
//                                        byte[] bytes6 = BitConverter.GetBytes((ushort)bytes5.Length);
//                                        byteList.AddRange((IEnumerable<byte>)bytes1);
//                                        byteList.AddRange((IEnumerable<byte>)bytes2);
//                                        byteList.AddRange((IEnumerable<byte>)bytes4);
//                                        byteList.AddRange((IEnumerable<byte>)bytes3);
//                                        byteList.AddRange((IEnumerable<byte>)bytes6);
//                                        byteList.AddRange((IEnumerable<byte>)bytes5);
//                                    }
//                                    SteamInventory.DestroyResult(SDG.Unturned.Provider.provider.economyService.wearingResult);
//                                    SDG.Unturned.Provider.provider.economyService.wearingResult = SteamInventoryResult_t.Invalid;
//                                    numArray2 = byteList.ToArray();
//                                    num2 = (uint)numArray2.Length;
//                                }
//                                UnityEngine.Debug.LogFormat("Sending Auth --- TicketLength: {0} ProofLength: {1}", (object)numArray1.Length, (object)num2);
//                                byte[] packet2 = new byte[(long)(5 + numArray1.Length) + (long)num2];
//                                packet2[0] = (byte)4;
//                                Buffer.BlockCopy((Array)BitConverter.GetBytes((ushort)numArray1.Length), 0, (Array)packet2, 1, 2);
//                                Buffer.BlockCopy((Array)numArray1, 0, (Array)packet2, 3, numArray1.Length);
//                                Buffer.BlockCopy((Array)BitConverter.GetBytes((ushort)num2), 0, (Array)packet2, 3 + numArray1.Length, 2);
//                                Buffer.BlockCopy((Array)numArray2, 0, (Array)packet2, 5 + numArray1.Length, (int)num2);
//                                SDG.Unturned.Provider.send(SDG.Unturned.Provider.server, ESteamPacket.AUTHENTICATE, packet2, packet2.Length, 0);
//                                return;
//                            case ESteamPacket.REJECTED:
//                                object[] objects2 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE);
//                                ESteamRejection esteamRejection = (ESteamRejection)(byte)objects2[1];
//                                string str = (string)objects2[2];
//                                SDG.Unturned.Provider._connectionFailureReason = string.Empty;
//                                switch (esteamRejection)
//                                {
//                                    case ESteamRejection.SERVER_FULL:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.FULL;
//                                        break;
//                                    case ESteamRejection.WRONG_HASH_LEVEL:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.HASH_LEVEL;
//                                        break;
//                                    case ESteamRejection.WRONG_HASH_ASSEMBLY:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.HASH_ASSEMBLY;
//                                        break;
//                                    case ESteamRejection.WRONG_VERSION:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.VERSION;
//                                        break;
//                                    case ESteamRejection.WRONG_PASSWORD:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PASSWORD;
//                                        break;
//                                    case ESteamRejection.NAME_PLAYER_SHORT:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_PLAYER_SHORT;
//                                        break;
//                                    case ESteamRejection.NAME_PLAYER_LONG:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_PLAYER_LONG;
//                                        break;
//                                    case ESteamRejection.NAME_PLAYER_INVALID:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_PLAYER_INVALID;
//                                        break;
//                                    case ESteamRejection.NAME_PLAYER_NUMBER:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_PLAYER_NUMBER;
//                                        break;
//                                    case ESteamRejection.NAME_CHARACTER_SHORT:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_CHARACTER_SHORT;
//                                        break;
//                                    case ESteamRejection.NAME_CHARACTER_LONG:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_CHARACTER_LONG;
//                                        break;
//                                    case ESteamRejection.NAME_CHARACTER_INVALID:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_CHARACTER_INVALID;
//                                        break;
//                                    case ESteamRejection.NAME_CHARACTER_NUMBER:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NAME_CHARACTER_NUMBER;
//                                        break;
//                                    case ESteamRejection.PRO_SERVER:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PRO_SERVER;
//                                        break;
//                                    case ESteamRejection.PRO_CHARACTER:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PRO_CHARACTER;
//                                        break;
//                                    case ESteamRejection.PRO_DESYNC:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PRO_DESYNC;
//                                        break;
//                                    case ESteamRejection.PRO_APPEARANCE:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PRO_APPEARANCE;
//                                        break;
//                                    case ESteamRejection.ALREADY_PENDING:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.ALREADY_PENDING;
//                                        break;
//                                    case ESteamRejection.ALREADY_CONNECTED:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.ALREADY_CONNECTED;
//                                        break;
//                                    case ESteamRejection.NOT_PENDING:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.NOT_PENDING;
//                                        break;
//                                    case ESteamRejection.LATE_PENDING:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.LATE_PENDING;
//                                        break;
//                                    case ESteamRejection.WHITELISTED:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.WHITELISTED;
//                                        break;
//                                    case ESteamRejection.AUTH_VERIFICATION:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_VERIFICATION;
//                                        break;
//                                    case ESteamRejection.AUTH_NO_STEAM:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_NO_STEAM;
//                                        break;
//                                    case ESteamRejection.AUTH_LICENSE_EXPIRED:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_LICENSE_EXPIRED;
//                                        break;
//                                    case ESteamRejection.AUTH_VAC_BAN:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_VAC_BAN;
//                                        break;
//                                    case ESteamRejection.AUTH_ELSEWHERE:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_ELSEWHERE;
//                                        break;
//                                    case ESteamRejection.AUTH_TIMED_OUT:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_TIMED_OUT;
//                                        break;
//                                    case ESteamRejection.AUTH_USED:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_USED;
//                                        break;
//                                    case ESteamRejection.AUTH_NO_USER:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_NO_USER;
//                                        break;
//                                    case ESteamRejection.AUTH_PUB_BAN:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_PUB_BAN;
//                                        break;
//                                    case ESteamRejection.AUTH_ECON_DESERIALIZE:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_ECON_DESERIALIZE;
//                                        break;
//                                    case ESteamRejection.AUTH_ECON_VERIFY:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.AUTH_ECON_VERIFY;
//                                        break;
//                                    case ESteamRejection.PING:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PING;
//                                        break;
//                                    case ESteamRejection.PLUGIN:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.PLUGIN;
//                                        SDG.Unturned.Provider._connectionFailureReason = str;
//                                        break;
//                                    case ESteamRejection.CLIENT_MODULE_DESYNC:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.CLIENT_MODULE_DESYNC;
//                                        break;
//                                    case ESteamRejection.SERVER_MODULE_DESYNC:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.SERVER_MODULE_DESYNC;
//                                        break;
//                                    default:
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.REJECT_UNKNOWN;
//                                        SDG.Unturned.Provider._connectionFailureReason = esteamRejection.ToString();
//                                        break;
//                                }
//                                SDG.Unturned.Provider.disconnect();
//                                return;
//                            case ESteamPacket.ACCEPTED:
//                                object[] objects3 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.UINT32_TYPE, Types.UINT16_TYPE, Types.BYTE_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.UINT16_TYPE, Types.UINT16_TYPE, Types.UINT16_TYPE, Types.UINT16_TYPE);
//                                uint num3 = (uint)objects3[1];
//                                ushort num4 = (ushort)objects3[2];
//                                if (SDG.Unturned.Provider.currentServerInfo != null && SDG.Unturned.Provider.currentServerInfo.IsBattlEyeSecure)
//                                {
//                                    string path = ReadWrite.PATH + "/BattlEye/BEClient_x64.dll";
//                                    if (!File.Exists(path))
//                                        path = ReadWrite.PATH + "/BattlEye/BEClient.dll";
//                                    if (!File.Exists(path))
//                                    {
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.KICKED;
//                                        SDG.Unturned.Provider._connectionFailureReason = "Missing BattlEye client library!";
//                                        UnityEngine.Debug.LogError((object)SDG.Unturned.Provider.connectionFailureReason);
//                                        SDG.Unturned.Provider.disconnect();
//                                        return;
//                                    }
//                                    try
//                                    {
//                                        SDG.Unturned.Provider.battlEyeClientHandle = BEClient.LoadLibraryW(path);
//                                        if (SDG.Unturned.Provider.battlEyeClientHandle != IntPtr.Zero)
//                                        {
//                                            BEClient.BEClientInitFn forFunctionPointer = Marshal.GetDelegateForFunctionPointer(BEClient.GetProcAddress(SDG.Unturned.Provider.battlEyeClientHandle, "Init"), typeof(BEClient.BEClientInitFn)) as BEClient.BEClientInitFn;
//                                            if (forFunctionPointer != null)
//                                            {
//                                                uint num5 = (uint)(((int)num3 & (int)byte.MaxValue) << 24 | ((int)num3 & 65280) << 8) | (num3 & 16711680U) >> 8 | (num3 & 4278190080U) >> 24;
//                                                ushort num6 = (ushort)((uint)(((int)num4 & (int)byte.MaxValue) << 8) | ((uint)num4 & 65280U) >> 8);
//                                                SDG.Unturned.Provider.battlEyeClientInitData = new BEClient.BECL_GAME_DATA();
//                                                SDG.Unturned.Provider.battlEyeClientInitData.pstrGameVersion = SDG.Unturned.Provider.APP_NAME + " " + SDG.Unturned.Provider.APP_VERSION;
//                                                SDG.Unturned.Provider.battlEyeClientInitData.ulAddress = num5;
//                                                SDG.Unturned.Provider.battlEyeClientInitData.usPort = num6;
//                                                BEClient.BECL_GAME_DATA eyeClientInitData1 = SDG.Unturned.Provider.battlEyeClientInitData;
//                                                // ISSUE: reference to a compiler-generated field
//                                                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache0 == null)//                        {
//                                                    // ISSUE: reference to a compiler-generated field
//                                                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache0 = new BEClient.BECL_GAME_DATA.PrintMessageFn(SDG.Unturned.Provider.battlEyeClientPrintMessage);
//                                                }
//                                                // ISSUE: reference to a compiler-generated field
//                                                BEClient.BECL_GAME_DATA.PrintMessageFn fMgCache0 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache0;
//                                                eyeClientInitData1.pfnPrintMessage = fMgCache0;
//                                                BEClient.BECL_GAME_DATA eyeClientInitData2 = SDG.Unturned.Provider.battlEyeClientInitData;
//                                                // ISSUE: reference to a compiler-generated field
//                                                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache1 == null)//                        {
//                                                    // ISSUE: reference to a compiler-generated field
//                                                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache1 = new BEClient.BECL_GAME_DATA.RequestRestartFn(SDG.Unturned.Provider.battlEyeClientRequestRestart);
//                                                }
//                                                // ISSUE: reference to a compiler-generated field
//                                                BEClient.BECL_GAME_DATA.RequestRestartFn fMgCache1 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache1;
//                                                eyeClientInitData2.pfnRequestRestart = fMgCache1;
//                                                BEClient.BECL_GAME_DATA eyeClientInitData3 = SDG.Unturned.Provider.battlEyeClientInitData;
//                                                // ISSUE: reference to a compiler-generated field
//                                                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache2 == null)//                        {
//                                                    // ISSUE: reference to a compiler-generated field
//                                                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache2 = new BEClient.BECL_GAME_DATA.SendPacketFn(SDG.Unturned.Provider.battlEyeClientSendPacket);
//                                                }
//                                                // ISSUE: reference to a compiler-generated field
//                                                BEClient.BECL_GAME_DATA.SendPacketFn fMgCache2 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache2;
//                                                eyeClientInitData3.pfnSendPacket = fMgCache2;
//                                                SDG.Unturned.Provider.battlEyeClientRunData = new BEClient.BECL_BE_DATA();
//                                                if (!forFunctionPointer(2, SDG.Unturned.Provider.battlEyeClientInitData, SDG.Unturned.Provider.battlEyeClientRunData))
//                                                {
//                                                    BEClient.FreeLibrary(SDG.Unturned.Provider.battlEyeClientHandle);
//                                                    SDG.Unturned.Provider.battlEyeClientHandle = IntPtr.Zero;
//                                                    SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.KICKED;
//                                                    SDG.Unturned.Provider._connectionFailureReason = "Failed to call BattlEye client init!";
//                                                    UnityEngine.Debug.LogError((object)SDG.Unturned.Provider.connectionFailureReason);
//                                                    SDG.Unturned.Provider.disconnect();
//                                                    return;
//                                                }
//                                            }
//                                            else
//                                            {
//                                                BEClient.FreeLibrary(SDG.Unturned.Provider.battlEyeClientHandle);
//                                                SDG.Unturned.Provider.battlEyeClientHandle = IntPtr.Zero;
//                                                SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.KICKED;
//                                                SDG.Unturned.Provider._connectionFailureReason = "Failed to get BattlEye client init delegate!";
//                                                UnityEngine.Debug.LogError((object)SDG.Unturned.Provider.connectionFailureReason);
//                                                SDG.Unturned.Provider.disconnect();
//                                                return;
//                                            }
//                                        }
//                                        else
//                                        {
//                                            SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.KICKED;
//                                            SDG.Unturned.Provider._connectionFailureReason = "Failed to load BattlEye client library!";
//                                            UnityEngine.Debug.LogError((object)SDG.Unturned.Provider.connectionFailureReason);
//                                            SDG.Unturned.Provider.disconnect();
//                                            return;
//                                        }
//                                    }
//                                    catch (Exception ex)
//                                    {
//                                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.KICKED;
//                                        SDG.Unturned.Provider._connectionFailureReason = "Unhandled exception when loading BattlEye client library!";
//                                        UnityEngine.Debug.LogError((object)SDG.Unturned.Provider.connectionFailureReason);
//                                        UnityEngine.Debug.LogException(ex);
//                                        SDG.Unturned.Provider.disconnect();
//                                        return;
//                                    }
//                                }
//                                SDG.Unturned.Provider._modeConfigData = new ModeConfigData(SDG.Unturned.Provider.mode);
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Repair_Level_Max = (uint)(byte)objects3[3];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Hitmarkers = (bool)objects3[4];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Crosshair = (bool)objects3[5];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Ballistics = (bool)objects3[6];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Chart = (bool)objects3[7];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Satellite = (bool)objects3[8];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Compass = (bool)objects3[9];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Group_Map = (bool)objects3[10];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Group_HUD = (bool)objects3[11];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Allow_Static_Groups = (bool)objects3[12];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Allow_Dynamic_Groups = (bool)objects3[13];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Allow_Shoulder_Camera = (bool)objects3[14];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Can_Suicide = (bool)objects3[15];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Timer_Exit = (uint)(ushort)objects3[16];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Timer_Respawn = (uint)(ushort)objects3[17];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Timer_Home = (uint)(ushort)objects3[18];
//                                SDG.Unturned.Provider.modeConfigData.Gameplay.Max_Group_Members = (uint)(ushort)objects3[19];
//                                if (OptionsSettings.streamer)
//                                {
//                                    SteamFriends.SetRichPresence("connect", string.Empty);
//                                }
//                                else
//                                {
//                                    SteamUser.AdvertiseGame(SDG.Unturned.Provider.server, num3, num4);
//                                    SteamFriends.SetRichPresence("connect", "+connect " + (object)num3 + ":" + (object)num4);
//                                }
//                                Lobbies.leaveLobby();
//                                SDG.Unturned.Provider.favoriteIP = num3;
//                                SDG.Unturned.Provider.favoritePort = num4;
//                                SteamMatchmaking.AddFavoriteGame(SDG.Unturned.Provider.APP_ID, num3, num4, (ushort)((uint)num4 + 1U), SDG.Unturned.Provider.STEAM_FAVORITE_FLAG_HISTORY, SteamUtils.GetServerRealTime());
//                                SDG.Unturned.Provider.updateRichPresence();
//                                if (SDG.Unturned.Provider.onClientConnected == null)
//                                    return;
//                                SDG.Unturned.Provider.onClientConnected();
//                                return;
//                            case ESteamPacket.ADMINED:
//                                int index3 = (int)packet[offset + 1];
//                                if (index3 < 0 || index3 >= SDG.Unturned.Provider.clients.Count)
//                                {
//                                    UnityEngine.Debug.LogError((object)("Failed to find player at index " + (object)index3 + "."));
//                                    return;
//                                }
//                                SDG.Unturned.Provider.clients[index3].isAdmin = true;
//                                return;
//                            case ESteamPacket.UNADMINED:
//                                int index4 = (int)packet[offset + 1];
//                                if (index4 < 0 || index4 >= SDG.Unturned.Provider.clients.Count)
//                                {
//                                    UnityEngine.Debug.LogError((object)("Failed to find player at index " + (object)index4 + "."));
//                                    return;
//                                }
//                                SDG.Unturned.Provider.clients[index4].isAdmin = false;
//                                return;
//                            case ESteamPacket.BANNED:
//                                object[] objects4 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.STRING_TYPE, Types.UINT32_TYPE);
//                                SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.BANNED;
//                                SDG.Unturned.Provider._connectionFailureReason = (string)objects4[1];
//                                SDG.Unturned.Provider._connectionFailureDuration = (uint)objects4[2];
//                                SDG.Unturned.Provider.disconnect();
//                                return;
//                            case ESteamPacket.KICKED:
//                                object[] objects5 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.STRING_TYPE);
//                                SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.KICKED;
//                                SDG.Unturned.Provider._connectionFailureReason = (string)objects5[1];
//                                SDG.Unturned.Provider.disconnect();
//                                return;
//                            case ESteamPacket.CONNECTED:
//                                object[] objects6 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.STEAM_ID_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE, Types.STRING_TYPE, Types.VECTOR3_TYPE, Types.BYTE_TYPE, Types.BOOLEAN_TYPE, Types.BOOLEAN_TYPE, Types.INT32_TYPE, Types.STEAM_ID_TYPE, Types.STRING_TYPE, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.BYTE_TYPE, Types.COLOR_TYPE, Types.COLOR_TYPE, Types.COLOR_TYPE, Types.BOOLEAN_TYPE, Types.INT32_TYPE, Types.INT32_TYPE, Types.INT32_TYPE, Types.INT32_TYPE, Types.INT32_TYPE, Types.INT32_TYPE, Types.INT32_TYPE, Types.INT32_ARRAY_TYPE, Types.STRING_ARRAY_TYPE, Types.STRING_ARRAY_TYPE, Types.BYTE_TYPE, Types.STRING_TYPE);
//                                SDG.Unturned.Provider.addPlayer(new SteamPlayerID((CSteamID)objects6[1], (byte)objects6[2], (string)objects6[3], (string)objects6[4], (string)objects6[11], (CSteamID)objects6[10]), (Vector3)objects6[5], (byte)objects6[6], (bool)objects6[7], (bool)objects6[8], (int)objects6[9], (byte)objects6[12], (byte)objects6[13], (byte)objects6[14], (Color)objects6[15], (Color)objects6[16], (Color)objects6[17], (bool)objects6[18], (int)objects6[19], (int)objects6[20], (int)objects6[21], (int)objects6[22], (int)objects6[23], (int)objects6[24], (int)objects6[25], (int[])objects6[26], (string[])objects6[27], (string[])objects6[28], (EPlayerSkillset)(byte)objects6[29], (string)objects6[30], CSteamID.Nil);
//                                return;
//                            case ESteamPacket.DISCONNECTED:
//                                SDG.Unturned.Provider.removePlayer(packet[offset + 1]);
//                                return;
//                            case ESteamPacket.PING_REQUEST:
//                                SDG.Unturned.Provider.send(SDG.Unturned.Provider.server, ESteamPacket.PING_RESPONSE, new byte[1]
//                                {//                  (byte) 14
//                                }, 1, 0);
//                                return;
//                            case ESteamPacket.PING_RESPONSE:
//                                if ((double)SDG.Unturned.Provider.timeLastPingRequestWasSentToServer <= 0.0)
//                                    return;
//                                object[] objects7 = SteamPacker.getObjects(steamID, offset, 0, packet, Types.BYTE_TYPE, Types.SINGLE_TYPE, Types.BYTE_TYPE);
//                                float deltaTime = Time.deltaTime;
//                                SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//                                SDG.Unturned.Provider.lastReceivedServersideTime = (float)objects7[1] + (float)(((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.timeLastPingRequestWasSentToServer - (double)deltaTime) / 2.0);
//                                if ((UnityEngine.Object)Player.player == (UnityEngine.Object)null)
//                                {
//                                    SDG.Unturned.Provider._queuePosition = (byte)objects7[2];
//                                    if (SDG.Unturned.Provider.onQueuePositionUpdated != null)
//                                        SDG.Unturned.Provider.onQueuePositionUpdated();
//                                }
//                                SDG.Unturned.Provider.lag(Time.realtimeSinceStartup - SDG.Unturned.Provider.timeLastPingRequestWasSentToServer - deltaTime);
//                                SDG.Unturned.Provider.timeLastPingRequestWasSentToServer = -1f;
//                                return;
//                            case ESteamPacket.BATTLEYE:
//                                if (!(SDG.Unturned.Provider.battlEyeClientHandle != IntPtr.Zero) || SDG.Unturned.Provider.battlEyeClientRunData == null || SDG.Unturned.Provider.battlEyeClientRunData.pfnReceivedPacket == null)
//                                    return;
//                                GCHandle gcHandle = GCHandle.Alloc((object)packet, GCHandleType.Pinned);
//                                IntPtr pvPacket = gcHandle.AddrOfPinnedObject();
//                                pvPacket = IntPtr.Size != 4 ? new IntPtr(pvPacket.ToInt64() + (long)offset + 1L) : new IntPtr(pvPacket.ToInt32() + offset + 1);
//                                SDG.Unturned.Provider.battlEyeClientRunData.pfnReceivedPacket(pvPacket, size - offset - 1);
//                                gcHandle.Free();
//                                return;
//                        }
//                    }
//                    UnityEngine.Debug.LogWarning((object)("Received client unhandled message " + (object)packet1 + " from " + (object)steamID + ", so we're refusing them"));
//                    SDG.Unturned.Provider.refuseGarbageConnection(steamID, "cl unhandled packet");
//                }
//            }
//        }

//        private static void listenServer(int channel)
//        {
//            ICommunityEntity entity;
//            ulong length;
//            while (SDG.Unturned.Provider.provider.multiplayerService.serverMultiplayerService.read(out entity, SDG.Unturned.Provider.buffer, out length, channel))
//                SDG.Unturned.Provider.receiveServer(((SteamworksCommunityEntity)entity).steamID, SDG.Unturned.Provider.buffer, 0, (int)length, channel);
//        }

//        private static void listenClient(int channel)
//        {
//            ICommunityEntity entity;
//            ulong length;
//            while (SDG.Unturned.Provider.provider.multiplayerService.clientMultiplayerService.read(out entity, SDG.Unturned.Provider.buffer, out length, channel))
//                SDG.Unturned.Provider.receiveClient(((SteamworksCommunityEntity)entity).steamID, SDG.Unturned.Provider.buffer, 0, (int)length, channel);
//        }

//        private static void listen()
//        {
//            if (!SDG.Unturned.Provider.isConnected)
//                return;
//            if (SDG.Unturned.Provider.isServer)
//            {
//                if (!Dedicator.isDedicated || !Level.isLoaded)
//                    return;
//                SDG.Unturned.Provider.listenServer(0);
//                for (int index = 0; index < SDG.Unturned.Provider.receivers.Count; ++index)
//                    SDG.Unturned.Provider.listenServer(SDG.Unturned.Provider.receivers[index].id);
//                if (!Dedicator.isDedicated)
//                    return;
//                if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.lastPingRequestTime > (double)SDG.Unturned.Provider.PING_REQUEST_INTERVAL)
//                {
//                    SDG.Unturned.Provider.lastPingRequestTime = Time.realtimeSinceStartup;
//                    for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//                    {
//                        if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.clients[index].timeLastPingRequestWasSentToClient > 1.0 || (double)SDG.Unturned.Provider.clients[index].timeLastPingRequestWasSentToClient < 0.0)
//                        {
//                            SDG.Unturned.Provider.clients[index].timeLastPingRequestWasSentToClient = Time.realtimeSinceStartup;
//                            SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index].playerID.steamID, ESteamPacket.PING_REQUEST, new byte[1]
//                            {//                (byte) 13
//                            }, 1, 0);
//                        }
//                    }
//                }
//                for (int index = SDG.Unturned.Provider.clients.Count - 1; index >= 0; --index)
//                {
//                    if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.clients[index].timeLastPacketWasReceivedFromClient > (double)SDG.Unturned.Provider.configData.Server.Timeout_Game_Seconds || (double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.clients[index].joined > (double)SDG.Unturned.Provider.configData.Server.Timeout_Game_Seconds && (double)SDG.Unturned.Provider.clients[index].ping > (double)SDG.Unturned.Provider.configData.Server.Max_Ping_Milliseconds / 1000.0)
//                    {
//                        if (CommandWindow.shouldLogJoinLeave)
//                        {
//                            SteamPlayerID playerId = SDG.Unturned.Provider.clients[index].playerID;
//                            CommandWindow.Log((object)SDG.Unturned.Provider.localization.format("Dismiss_Timeout", (object)playerId.steamID, (object)playerId.playerName, (object)playerId.characterName));
//                        }
//                        SDG.Unturned.Provider.dismiss(SDG.Unturned.Provider.clients[index].playerID.steamID);
//                        break;
//                    }
//                    SDG.Unturned.Provider.clients[index].rpcCredits -= Time.deltaTime;
//                    if ((double)SDG.Unturned.Provider.clients[index].rpcCredits < 0.0)
//                        SDG.Unturned.Provider.clients[index].rpcCredits = 0.0f;
//                }
//                if (SDG.Unturned.Provider.pending.Count > 0 && SDG.Unturned.Provider.pending[0].hasSentVerifyPacket && (double)SDG.Unturned.Provider.pending[0].realtimeSinceSentVerifyPacket > (double)SDG.Unturned.Provider.configData.Server.Timeout_Queue_Seconds)
//                    SDG.Unturned.Provider.reject(SDG.Unturned.Provider.pending[0].playerID.steamID, ESteamRejection.LATE_PENDING);
//                if (SDG.Unturned.Provider.pending.Count <= 1)
//                    return;
//                for (int index = SDG.Unturned.Provider.pending.Count - 1; index > 0; --index)
//                {
//                    if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.pending[index].lastReceivedPingRequestRealtime > (double)SDG.Unturned.Provider.configData.Server.Timeout_Queue_Seconds)
//                    {
//                        SDG.Unturned.Provider.reject(SDG.Unturned.Provider.pending[index].playerID.steamID, ESteamRejection.LATE_PENDING);
//                        break;
//                    }
//                }
//            }
//            else
//            {
//                SDG.Unturned.Provider.listenClient(0);
//                for (int index = 0; index < SDG.Unturned.Provider.receivers.Count; ++index)
//                    SDG.Unturned.Provider.listenClient(SDG.Unturned.Provider.receivers[index].id);
//                if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.lastPingRequestTime > (double)SDG.Unturned.Provider.PING_REQUEST_INTERVAL && ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.timeLastPingRequestWasSentToServer > 1.0 || (double)SDG.Unturned.Provider.timeLastPingRequestWasSentToServer < 0.0))
//                {
//                    SDG.Unturned.Provider.lastPingRequestTime = Time.realtimeSinceStartup;
//                    SDG.Unturned.Provider.timeLastPingRequestWasSentToServer = Time.realtimeSinceStartup;
//                    SDG.Unturned.Provider.send(SDG.Unturned.Provider.server, ESteamPacket.PING_REQUEST, new byte[1]
//                    {//            (byte) 13
//                    }, 1, 0);
//                }
//                if (SDG.Unturned.Provider.isLoadingUGC)
//                {
//                    if (SDG.Unturned.Provider.isWaitingForWorkshopResponse)
//                    {
//                        if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer <= (double)SDG.Unturned.Provider.CLIENT_TIMEOUT)
//                            return;
//                        SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.TIMED_OUT;
//                        SDG.Unturned.Provider.disconnect();
//                    }
//                    else
//                        SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//                }
//                else if (Level.isLoading)
//                    SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer = Time.realtimeSinceStartup;
//                else if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer > (double)SDG.Unturned.Provider.CLIENT_TIMEOUT)
//                {
//                    SDG.Unturned.Provider._connectionFailureInfo = ESteamConnectionFailureInfo.TIMED_OUT;
//                    SDG.Unturned.Provider.disconnect();
//                }
//                else
//                {
//                    if (!SDG.Unturned.Provider.battlEyeHasRequiredRestart)
//                        return;
//                    SDG.Unturned.Provider.battlEyeHasRequiredRestart = false;
//                    SDG.Unturned.Provider.disconnect();
//                }
//            }
//        }

//        private static void broadcastServerDisconnected(CSteamID steamID)
//        {
//            try
//            {
//                if (SDG.Unturned.Provider.onServerDisconnected == null)
//                    return;
//                SDG.Unturned.Provider.onServerDisconnected(steamID);
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerDisconnected:");
//                UnityEngine.Debug.LogException(ex);
//            }
//        }

//        private static void broadcastServerHosted()
//        {
//            try
//            {
//                if (SDG.Unturned.Provider.onServerHosted == null)
//                    return;
//                SDG.Unturned.Provider.onServerHosted();
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerHosted:");
//                UnityEngine.Debug.LogException(ex);
//            }
//        }

//        private static void broadcastServerShutdown()
//        {
//            try
//            {
//                if (SDG.Unturned.Provider.onServerShutdown == null)
//                    return;
//                SDG.Unturned.Provider.onServerShutdown();
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerShutdown:");
//                UnityEngine.Debug.LogException(ex);
//            }
//        }

//        private static void onP2PSessionConnectFail(P2PSessionConnectFail_t callback)
//        {
//            SDG.Unturned.Provider.dismiss(callback.m_steamIDRemote);
//        }

//        private static void checkBanStatus(
//          CSteamID steamID,
//          uint remoteIP,
//          out bool isBanned,
//          out string banReason,
//          out uint banRemainingDuration)
//        {
//            isBanned = false;
//            banReason = string.Empty;
//            banRemainingDuration = 0U;
//            SteamBlacklistID blacklistID;
//            if (SteamBlacklist.checkBanned(steamID, remoteIP, out blacklistID))
//            {
//                isBanned = true;
//                banReason = blacklistID.reason;
//                banRemainingDuration = blacklistID.getTime();
//            }
//            if (SDG.Unturned.Provider.onCheckBanStatus == null)
//                return;
//            SDG.Unturned.Provider.onCheckBanStatus(steamID, remoteIP, ref isBanned, ref banReason, ref banRemainingDuration);
//        }

//        public static bool requestBanPlayer(
//          CSteamID instigator,
//          CSteamID playerToBan,
//          uint ipToBan,
//          string reason,
//          uint duration)
//        {
//            bool shouldVanillaBan = true;
//            if (SDG.Unturned.Provider.onBanPlayerRequested != null)
//                SDG.Unturned.Provider.onBanPlayerRequested(instigator, playerToBan, ipToBan, ref reason, ref duration, ref shouldVanillaBan);
//            if (shouldVanillaBan)
//                SteamBlacklist.ban(playerToBan, ipToBan, instigator, reason, duration);
//            return true;
//        }

//        public static bool requestUnbanPlayer(CSteamID instigator, CSteamID playerToUnban)
//        {
//            bool shouldVanillaUnban = true;
//            if (SDG.Unturned.Provider.onUnbanPlayerRequested != null)
//                SDG.Unturned.Provider.onUnbanPlayerRequested(instigator, playerToUnban, ref shouldVanillaUnban);
//            if (shouldVanillaUnban)
//                return SteamBlacklist.unban(playerToUnban);
//            return true;
//        }

//        private static void handleValidateAuthTicketResponse(ValidateAuthTicketResponse_t callback)
//        {
//            if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseOK)
//            {
//                SteamPending player = (SteamPending)null;
//                for (int index = 0; index < SDG.Unturned.Provider.pending.Count; ++index)
//                {
//                    if (SDG.Unturned.Provider.pending[index].playerID.steamID == callback.m_SteamID)
//                    {
//                        player = SDG.Unturned.Provider.pending[index];
//                        break;
//                    }
//                }
//                if (player == null)
//                {
//                    for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//                    {
//                        if (SDG.Unturned.Provider.clients[index].playerID.steamID == callback.m_SteamID)
//                            return;
//                    }
//                    SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.NOT_PENDING);
//                }
//                else
//                {
//                    bool isValid = true;
//                    string empty = string.Empty;
//                    try
//                    {
//                        if (SDG.Unturned.Provider.onCheckValidWithExplanation != null)
//                            SDG.Unturned.Provider.onCheckValidWithExplanation(callback, ref isValid, ref empty);
//                        else if (SDG.Unturned.Provider.onCheckValid != null)
//                            SDG.Unturned.Provider.onCheckValid(callback, ref isValid);
//                    }
//                    catch (Exception ex)
//                    {
//                        UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onCheckValidWithExplanation or onCheckValid:");
//                        UnityEngine.Debug.LogException(ex);
//                    }
//                    if (!isValid)
//                    {
//                        SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.PLUGIN, empty);
//                    }
//                    else
//                    {
//                        bool flag = SteamGameServer.UserHasLicenseForApp(player.playerID.steamID, SDG.Unturned.Provider.PRO_ID) != EUserHasLicenseForAppResult.k_EUserHasLicenseResultDoesNotHaveLicense;
//                        if (SDG.Unturned.Provider.isGold && !flag)
//                            SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_SERVER);
//                        else if ((int)player.playerID.characterID >= (int)Customization.FREE_CHARACTERS && !flag || (int)player.playerID.characterID >= (int)Customization.FREE_CHARACTERS + (int)Customization.PRO_CHARACTERS)
//                            SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_CHARACTER);
//                        else if (!flag && player.isPro)
//                            SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_DESYNC);
//                        else if ((int)player.face >= (int)Customization.FACES_FREE + (int)Customization.FACES_PRO || !flag && (int)player.face >= (int)Customization.FACES_FREE)
//                            SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_APPEARANCE);
//                        else if ((int)player.hair >= (int)Customization.HAIRS_FREE + (int)Customization.HAIRS_PRO || !flag && (int)player.hair >= (int)Customization.HAIRS_FREE)
//                            SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_APPEARANCE);
//                        else if ((int)player.beard >= (int)Customization.BEARDS_FREE + (int)Customization.BEARDS_PRO || !flag && (int)player.beard >= (int)Customization.BEARDS_FREE)
//                        {
//                            SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_APPEARANCE);
//                        }
//                        else
//                        {
//                            if (!flag)
//                            {
//                                if (!Customization.checkSkin(player.skin))
//                                {
//                                    SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_APPEARANCE);
//                                    return;
//                                }
//                                if (!Customization.checkColor(player.color))
//                                {
//                                    SDG.Unturned.Provider.reject(player.playerID.steamID, ESteamRejection.PRO_APPEARANCE);
//                                    return;
//                                }
//                            }
//                            player.assignedPro = flag;
//                            player.assignedAdmin = SteamAdminlist.checkAdmin(player.playerID.steamID);
//                            player.hasAuthentication = true;
//                            if (!player.canAcceptYet)
//                                return;
//                            SDG.Unturned.Provider.sendGUIDTable(player);
//                        }
//                    }
//                }
//            }
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseUserNotConnectedToSteam)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_NO_STEAM);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseNoLicenseOrExpired)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_LICENSE_EXPIRED);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseVACBanned)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_VAC_BAN);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseLoggedInElseWhere)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_ELSEWHERE);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseVACCheckTimedOut)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_TIMED_OUT);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseAuthTicketCanceled)
//            {
//                if (CommandWindow.shouldLogJoinLeave)
//                    CommandWindow.Log((object)("Player finished session: " + (object)callback.m_SteamID));
//                SDG.Unturned.Provider.dismiss(callback.m_SteamID);
//            }
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseAuthTicketInvalidAlreadyUsed)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_USED);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponseAuthTicketInvalid)
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_NO_USER);
//            else if (callback.m_eAuthSessionResponse == EAuthSessionResponse.k_EAuthSessionResponsePublisherIssuedBan)
//            {
//                SDG.Unturned.Provider.reject(callback.m_SteamID, ESteamRejection.AUTH_PUB_BAN);
//            }
//            else
//            {
//                if (CommandWindow.shouldLogJoinLeave)
//                    CommandWindow.Log((object)("Kicking player " + (object)callback.m_SteamID + " for unknown session response " + (object)callback.m_eAuthSessionResponse));
//                SDG.Unturned.Provider.dismiss(callback.m_SteamID);
//            }
//        }

//        private static void onValidateAuthTicketResponse(ValidateAuthTicketResponse_t callback)
//        {
//            SDG.Unturned.Provider.handleValidateAuthTicketResponse(callback);
//        }

//        private static void handleClientGroupStatus(GSClientGroupStatus_t callback)
//        {
//            SteamPending player = (SteamPending)null;
//            for (int index = 0; index < SDG.Unturned.Provider.pending.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.pending[index].playerID.steamID == callback.m_SteamIDUser)
//                {
//                    player = SDG.Unturned.Provider.pending[index];
//                    break;
//                }
//            }
//            if (player == null)
//            {
//                SDG.Unturned.Provider.reject(callback.m_SteamIDUser, ESteamRejection.NOT_PENDING);
//            }
//            else
//            {
//                if (!callback.m_bMember && !callback.m_bOfficer)
//                    player.playerID.group = CSteamID.Nil;
//                player.hasGroup = true;
//                if (!player.canAcceptYet)
//                    return;
//                SDG.Unturned.Provider.sendGUIDTable(player);
//            }
//        }

//        private static void onClientGroupStatus(GSClientGroupStatus_t callback)
//        {
//            SDG.Unturned.Provider.handleClientGroupStatus(callback);
//        }

//        public static byte maxPlayers
//        {
//            get
//            {
//                return SDG.Unturned.Provider._maxPlayers;
//            }
//            set
//            {
//                SDG.Unturned.Provider._maxPlayers = value;
//                if (!SDG.Unturned.Provider.isServer)
//                    return;
//                SteamGameServer.SetMaxPlayerCount((int)SDG.Unturned.Provider.maxPlayers);
//            }
//        }

//        public static byte queuePosition
//        {
//            get
//            {
//                return SDG.Unturned.Provider._queuePosition;
//            }
//        }

//        public static string serverName
//        {
//            get
//            {
//                return SDG.Unturned.Provider._serverName;
//            }
//            set
//            {
//                SDG.Unturned.Provider._serverName = value;
//                if (Dedicator.commandWindow != null)
//                    Dedicator.commandWindow.title = SDG.Unturned.Provider.serverName;
//                if (!SDG.Unturned.Provider.isServer)
//                    return;
//                SteamGameServer.SetServerName(SDG.Unturned.Provider.serverName);
//            }
//        }

//        public static string serverID
//        {
//            get
//            {
//                return Dedicator.serverID;
//            }
//            set
//            {
//                Dedicator.serverID = value;
//            }
//        }

//        public static byte[] serverPasswordHash
//        {
//            get
//            {
//                return SDG.Unturned.Provider._serverPasswordHash;
//            }
//        }

//        public static string serverPassword
//        {
//            get
//            {
//                return SDG.Unturned.Provider._serverPassword;
//            }
//            set
//            {
//                SDG.Unturned.Provider._serverPassword = value;
//                SDG.Unturned.Provider._serverPasswordHash = Hash.SHA1(SDG.Unturned.Provider.serverPassword);
//                if (!SDG.Unturned.Provider.isServer)
//                    return;
//                SteamGameServer.SetPasswordProtected(SDG.Unturned.Provider.serverPassword != string.Empty);
//            }
//        }

//        public static StatusData statusData
//        {
//            get
//            {
//                return SDG.Unturned.Provider._statusData;
//            }
//        }

//        public static PreferenceData preferenceData
//        {
//            get
//            {
//                return SDG.Unturned.Provider._preferenceData;
//            }
//        }

//        public static ConfigData configData
//        {
//            get
//            {
//                return SDG.Unturned.Provider._configData;
//            }
//        }

//        public static ModeConfigData modeConfigData
//        {
//            get
//            {
//                return SDG.Unturned.Provider._modeConfigData;
//            }
//        }

//        public static void resetConfig()
//        {
//            SDG.Unturned.Provider._modeConfigData = new ModeConfigData(SDG.Unturned.Provider.mode);
//            switch (SDG.Unturned.Provider.mode)
//            {
//                case EGameMode.EASY:
//                    SDG.Unturned.Provider.configData.Easy = SDG.Unturned.Provider.modeConfigData;
//                    break;
//                case EGameMode.NORMAL:
//                    SDG.Unturned.Provider.configData.Normal = SDG.Unturned.Provider.modeConfigData;
//                    break;
//                case EGameMode.HARD:
//                    SDG.Unturned.Provider.configData.Hard = SDG.Unturned.Provider.modeConfigData;
//                    break;
//            }
//            ServerSavedata.serializeJSON<ConfigData>("/Config.json", SDG.Unturned.Provider.configData);
//        }

//        public static void applyLevelModeConfigOverrides()
//        {
//            if (Level.info == null || Level.info.configData == null)
//                return;
//            foreach (KeyValuePair<string, object> modeConfigOverride in Level.info.configData.Mode_Config_Overrides)
//            {
//                if (string.IsNullOrEmpty(modeConfigOverride.Key))
//                {
//                    CommandWindow.LogError((object)"Level mode config overrides contains an empty key");
//                    break;
//                }
//                if (modeConfigOverride.Value == null)
//                {
//                    CommandWindow.LogError((object)"Level mode config overrides contains a null value");
//                    break;
//                }
//                Type type = typeof(ModeConfigData);
//                object modeConfigData = (object)SDG.Unturned.Provider.modeConfigData;
//                string[] strArray = modeConfigOverride.Key.Split('.');
//                for (int index = 0; index < strArray.Length; ++index)
//                {
//                    string name = strArray[index];
//                    FieldInfo field = type.GetField(name);
//                    if (field == (FieldInfo)null)
//                    {
//                        CommandWindow.LogError((object)("Failed to find mode config for level override: " + name));
//                        break;
//                    }
//                    if (index == strArray.Length - 1)
//                    {
//                        object obj = modeConfigOverride.Value;
//                        if (obj is bool)
//                            field.SetValue(modeConfigData, obj);
//                        else if (obj is double)
//                            field.SetValue(modeConfigData, (object)(float)(double)obj);
//                        else if (obj is long)
//                        {
//                            field.SetValue(modeConfigData, (object)(uint)(long)obj);
//                        }
//                        else
//                        {
//                            CommandWindow.LogError((object)("Failed to handle level mode config override type: " + modeConfigOverride.Key + " -> " + (object)obj.GetType()));
//                            break;
//                        }
//                        CommandWindow.Log((object)("Level Overrides Config: " + modeConfigOverride.Key + " -> " + modeConfigOverride.Value));
//                    }
//                    else
//                    {
//                        type = field.FieldType;
//                        modeConfigData = field.GetValue(modeConfigData);
//                    }
//                }
//            }
//        }

//        public static void accept(SteamPending player)
//        {
//            SDG.Unturned.Provider.accept(player.playerID, player.assignedPro, player.assignedAdmin, player.face, player.hair, player.beard, player.skin, player.color, player.markerColor, player.hand, player.shirtItem, player.pantsItem, player.hatItem, player.backpackItem, player.vestItem, player.maskItem, player.glassesItem, player.skinItems, player.skinTags, player.skinDynamicProps, player.skillset, player.language, player.lobbyID);
//        }

//        private static byte[] buildConnectionPacket(
//          SteamPlayer aboutPlayer,
//          SteamPlayer forPlayer,
//          out int size)
//        {
//            CSteamID steamId = aboutPlayer.playerID.steamID;
//            byte characterId = aboutPlayer.playerID.characterID;
//            string playerName = aboutPlayer.playerID.playerName;
//            string characterName = aboutPlayer.playerID.characterName;
//            Vector3 position = aboutPlayer.model.transform.position;
//            byte num = (byte)((double)aboutPlayer.model.transform.rotation.eulerAngles.y / 2.0);
//            bool isPro = aboutPlayer.isPro;
//            bool flag = aboutPlayer.isAdmin;
//            if (forPlayer != aboutPlayer && SDG.Unturned.Provider.hideAdmins)
//                flag = false;
//            int channel = aboutPlayer.channel;
//            CSteamID group = aboutPlayer.playerID.group;
//            string nickName = aboutPlayer.playerID.nickName;
//            byte face = aboutPlayer.face;
//            byte hair = aboutPlayer.hair;
//            byte beard = aboutPlayer.beard;
//            Color skin = aboutPlayer.skin;
//            Color color = aboutPlayer.color;
//            Color markerColor = aboutPlayer.markerColor;
//            bool hand = aboutPlayer.hand;
//            int shirtItem = aboutPlayer.shirtItem;
//            int pantsItem = aboutPlayer.pantsItem;
//            int hatItem = aboutPlayer.hatItem;
//            int backpackItem = aboutPlayer.backpackItem;
//            int vestItem = aboutPlayer.vestItem;
//            int maskItem = aboutPlayer.maskItem;
//            int glassesItem = aboutPlayer.glassesItem;
//            int[] skinItems = aboutPlayer.skinItems;
//            string[] skinTags = aboutPlayer.skinTags;
//            string[] skinDynamicProps = aboutPlayer.skinDynamicProps;
//            byte skillset = (byte)aboutPlayer.skillset;
//            string language = aboutPlayer.language;
//            return SteamPacker.getBytes(0, out size, (object)(byte)11, (object)steamId, (object)characterId, (object)playerName, (object)characterName, (object)position, (object)num, (object)isPro, (object)flag, (object)channel, (object)group, (object)nickName, (object)face, (object)hair, (object)beard, (object)skin, (object)color, (object)markerColor, (object)hand, (object)shirtItem, (object)pantsItem, (object)hatItem, (object)backpackItem, (object)vestItem, (object)maskItem, (object)glassesItem, (object)skinItems, (object)skinTags, (object)skinDynamicProps, (object)skillset, (object)language);
//        }

//        public static void accept(
//          SteamPlayerID playerID,
//          bool isPro,
//          bool isAdmin,
//          byte face,
//          byte hair,
//          byte beard,
//          Color skin,
//          Color color,
//          Color markerColor,
//          bool hand,
//          int shirtItem,
//          int pantsItem,
//          int hatItem,
//          int backpackItem,
//          int vestItem,
//          int maskItem,
//          int glassesItem,
//          int[] skinItems,
//          string[] skinTags,
//          string[] skinDynamicProps,
//          EPlayerSkillset skillset,
//          string language,
//          CSteamID lobbyID)
//        {
//            bool flag = false;
//            int num1 = 0;
//            for (int index = 0; index < SDG.Unturned.Provider.pending.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.pending[index].playerID == playerID)
//                {
//                    if (SDG.Unturned.Provider.pending[index].inventoryResult != SteamInventoryResult_t.Invalid)
//                    {
//                        SteamGameServerInventory.DestroyResult(SDG.Unturned.Provider.pending[index].inventoryResult);
//                        SDG.Unturned.Provider.pending[index].inventoryResult = SteamInventoryResult_t.Invalid;
//                    }
//                    flag = true;
//                    num1 = index;
//                    SDG.Unturned.Provider.pending.RemoveAt(index);
//                    break;
//                }
//            }
//            if (!flag)
//                return;
//            if (isPro)
//                SteamGameServer.BUpdateUserData(playerID.steamID, playerID.playerName, 1U);
//            else
//                SteamGameServer.BUpdateUserData(playerID.steamID, playerID.playerName, 0U);
//            Vector3 point;
//            byte angle;
//            EPlayerStance initialStance;
//            SDG.Unturned.Provider.loadPlayerSpawn(playerID, out point, out angle, out initialStance);
//            int channels = SDG.Unturned.Provider.channels;
//            SteamPlayer steamPlayer = SDG.Unturned.Provider.addPlayer(playerID, point, angle, isPro, isAdmin, channels, face, hair, beard, skin, color, markerColor, hand, shirtItem, pantsItem, hatItem, backpackItem, vestItem, maskItem, glassesItem, skinItems, skinTags, skinDynamicProps, skillset, language, lobbyID);
//            PlayerStance component = steamPlayer.player.GetComponent<PlayerStance>();
//            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
//                component.initialStance = initialStance;
//            else
//                UnityEngine.Debug.LogWarning((object)"Was unable to get PlayerStance for new connection!");
//            int size;
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                byte[] packet = SDG.Unturned.Provider.buildConnectionPacket(SDG.Unturned.Provider.clients[index], steamPlayer, out size);
//                SDG.Unturned.Provider.send(playerID.steamID, ESteamPacket.CONNECTED, packet, size, 0);
//            }
//            byte[] bytes = SteamPacker.getBytes(0, out size, (object)(byte)6, (object)SteamGameServer.GetPublicIP(), (object)SDG.Unturned.Provider.port, (object)(byte)SDG.Unturned.Provider.modeConfigData.Gameplay.Repair_Level_Max, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Hitmarkers, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Crosshair, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Ballistics, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Chart, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Satellite, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Compass, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Group_Map, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Group_HUD, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Allow_Static_Groups, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Allow_Dynamic_Groups, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Allow_Shoulder_Camera, (object)SDG.Unturned.Provider.modeConfigData.Gameplay.Can_Suicide, (object)(ushort)SDG.Unturned.Provider.modeConfigData.Gameplay.Timer_Exit, (object)(ushort)SDG.Unturned.Provider.modeConfigData.Gameplay.Timer_Respawn, (object)(ushort)SDG.Unturned.Provider.modeConfigData.Gameplay.Timer_Home, (object)(ushort)SDG.Unturned.Provider.modeConfigData.Gameplay.Max_Group_Members);
//            SDG.Unturned.Provider.send(playerID.steamID, ESteamPacket.ACCEPTED, bytes, size, 0);
//            if (SDG.Unturned.Provider.battlEyeServerHandle != IntPtr.Zero && SDG.Unturned.Provider.battlEyeServerRunData != null && (SDG.Unturned.Provider.battlEyeServerRunData.pfnAddPlayer != null && SDG.Unturned.Provider.battlEyeServerRunData.pfnReceivedPlayerGUID != null))
//            {
//                P2PSessionState_t pConnectionState;
//                uint num2;
//                ushort num3;
//                if (SteamGameServerNetworking.GetP2PSessionState(playerID.steamID, out pConnectionState))
//                {
//                    num2 = pConnectionState.m_nRemoteIP;
//                    num3 = pConnectionState.m_nRemotePort;
//                }
//                else
//                {
//                    num2 = 0U;
//                    num3 = (ushort)0;
//                }
//                uint ulAddress = (uint)(((int)num2 & (int)byte.MaxValue) << 24 | ((int)num2 & 65280) << 8) | (num2 & 16711680U) >> 8 | (num2 & 4278190080U) >> 24;
//                ushort usPort = (ushort)((uint)(((int)num3 & (int)byte.MaxValue) << 8) | ((uint)num3 & 65280U) >> 8);
//                SDG.Unturned.Provider.battlEyeServerRunData.pfnAddPlayer(channels, ulAddress, usPort, playerID.playerName);
//                GCHandle gcHandle = GCHandle.Alloc((object)playerID.steamID.m_SteamID, GCHandleType.Pinned);
//                IntPtr pvGUID = gcHandle.AddrOfPinnedObject();
//                SDG.Unturned.Provider.battlEyeServerRunData.pfnReceivedPlayerGUID(channels, pvGUID, 8);
//                gcHandle.Free();
//            }
//            byte[] packet1 = SDG.Unturned.Provider.buildConnectionPacket(steamPlayer, (SteamPlayer)null, out size);
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.clients[index] != steamPlayer)
//                    SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index].playerID.steamID, ESteamPacket.CONNECTED, packet1, size, 0);
//            }
//            try
//            {
//                if (SDG.Unturned.Provider.onServerConnected != null)
//                    SDG.Unturned.Provider.onServerConnected(playerID.steamID);
//            }
//            catch (Exception ex)
//            {
//                UnityEngine.Debug.LogWarning((object)"Plugin raised an exception from onServerConnected:");
//                UnityEngine.Debug.LogException(ex);
//            }
//            if (CommandWindow.shouldLogJoinLeave)
//                CommandWindow.Log((object)SDG.Unturned.Provider.localization.format("PlayerConnectedText", (object)playerID.steamID, (object)playerID.playerName, (object)playerID.characterName));
//            if (num1 != 0)
//                return;
//            SDG.Unturned.Provider.verifyNextPlayerInQueue();
//        }

//        public static void reject(CSteamID steamID, ESteamRejection rejection)
//        {
//            SDG.Unturned.Provider.reject(steamID, rejection, string.Empty);
//        }

//        public static void reject(CSteamID steamID, ESteamRejection rejection, string explanation)
//        {
//            for (int index = 0; index < SDG.Unturned.Provider.pending.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.pending[index].playerID.steamID == steamID)
//                {
//                    switch (rejection)
//                    {
//                        case ESteamRejection.AUTH_VAC_BAN:
//                            ChatManager.say(SDG.Unturned.Provider.pending[index].playerID.playerName + " was banned by VAC", Color.yellow, false);
//                            break;
//                        case ESteamRejection.AUTH_PUB_BAN:
//                            ChatManager.say(SDG.Unturned.Provider.pending[index].playerID.playerName + " was banned by BattlEye", Color.yellow, false);
//                            break;
//                    }
//                    if (SDG.Unturned.Provider.pending[index].inventoryResult != SteamInventoryResult_t.Invalid)
//                    {
//                        SteamGameServerInventory.DestroyResult(SDG.Unturned.Provider.pending[index].inventoryResult);
//                        SDG.Unturned.Provider.pending[index].inventoryResult = SteamInventoryResult_t.Invalid;
//                    }
//                    SDG.Unturned.Provider.pending.RemoveAt(index);
//                    if (index == 0)
//                    {
//                        SDG.Unturned.Provider.verifyNextPlayerInQueue();
//                        break;
//                    }
//                    break;
//                }
//            }
//            SteamGameServer.EndAuthSession(steamID);
//            int size;
//            byte[] bytes = SteamPacker.getBytes(0, out size, (object)(byte)5, (object)(byte)rejection, (object)explanation);
//            SDG.Unturned.Provider.send(steamID, ESteamPacket.REJECTED, bytes, size, 0);
//        }

//        public static void kick(CSteamID steamID, string reason)
//        {
//            bool flag = false;
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.clients[index].playerID.steamID == steamID)
//                {
//                    flag = true;
//                    break;
//                }
//            }
//            if (!flag)
//                return;
//            int size;
//            byte[] bytes = SteamPacker.getBytes(0, out size, (object)(byte)10, (object)reason);
//            SDG.Unturned.Provider.send(steamID, ESteamPacket.KICKED, bytes, size, 0);
//            SDG.Unturned.Provider.broadcastServerDisconnected(steamID);
//            SteamGameServer.EndAuthSession(steamID);
//            for (int index1 = 0; index1 < SDG.Unturned.Provider.clients.Count; ++index1)
//            {
//                if (SDG.Unturned.Provider.clients[index1].playerID.steamID == steamID)
//                {
//                    SDG.Unturned.Provider.removePlayer((byte)index1);
//                    for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                    {
//                        if (SDG.Unturned.Provider.clients[index2].playerID.steamID != steamID)
//                            SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index2].playerID.steamID, ESteamPacket.DISCONNECTED, new byte[2]
//                            {//                (byte) 12,//                (byte) index1
//                            }, 2, 0);
//                    }
//                    break;
//                }
//            }
//        }

//        public static void ban(CSteamID steamID, string reason, uint duration)
//        {
//            bool flag = false;
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.clients[index].playerID.steamID == steamID)
//                {
//                    flag = true;
//                    break;
//                }
//            }
//            if (!flag)
//                return;
//            int size;
//            byte[] bytes = SteamPacker.getBytes(0, out size, (object)(byte)9, (object)reason, (object)duration);
//            SDG.Unturned.Provider.send(steamID, ESteamPacket.BANNED, bytes, size, 0);
//            SteamGameServer.EndAuthSession(steamID);
//            for (int index1 = 0; index1 < SDG.Unturned.Provider.clients.Count; ++index1)
//            {
//                if (SDG.Unturned.Provider.clients[index1].playerID.steamID == steamID)
//                {
//                    SDG.Unturned.Provider.broadcastServerDisconnected(steamID);
//                    SDG.Unturned.Provider.removePlayer((byte)index1);
//                    for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                    {
//                        if (SDG.Unturned.Provider.clients[index2].playerID.steamID != steamID)
//                            SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index2].playerID.steamID, ESteamPacket.DISCONNECTED, new byte[2]
//                            {//                (byte) 12,//                (byte) index1
//                            }, 2, 0);
//                    }
//                    break;
//                }
//            }
//        }

//        public static void dismiss(CSteamID steamID)
//        {
//            bool flag = false;
//            for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//            {
//                if (SDG.Unturned.Provider.clients[index].playerID.steamID == steamID)
//                {
//                    flag = true;
//                    break;
//                }
//            }
//            if (!flag)
//                return;
//            SDG.Unturned.Provider.broadcastServerDisconnected(steamID);
//            SteamGameServer.EndAuthSession(steamID);
//            for (int index1 = 0; index1 < SDG.Unturned.Provider.clients.Count; ++index1)
//            {
//                if (SDG.Unturned.Provider.clients[index1].playerID.steamID == steamID)
//                {
//                    if (CommandWindow.shouldLogJoinLeave)
//                        CommandWindow.Log((object)SDG.Unturned.Provider.localization.format("PlayerDisconnectedText", (object)SDG.Unturned.Provider.clients[index1].playerID.steamID, (object)SDG.Unturned.Provider.clients[index1].playerID.playerName, (object)SDG.Unturned.Provider.clients[index1].playerID.characterName));
//                    SDG.Unturned.Provider.removePlayer((byte)index1);
//                    for (int index2 = 0; index2 < SDG.Unturned.Provider.clients.Count; ++index2)
//                    {
//                        if (SDG.Unturned.Provider.clients[index2].playerID.steamID != steamID)
//                            SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index2].playerID.steamID, ESteamPacket.DISCONNECTED, new byte[2]
//                            {//                (byte) 12,//                (byte) index1
//                            }, 2, 0);
//                    }
//                    break;
//                }
//            }
//        }

//        private static bool verifyTicket(CSteamID steamID, byte[] ticket)
//        {
//            return SteamGameServer.BeginAuthSession(ticket, ticket.Length, steamID) == EBeginAuthSessionResult.k_EBeginAuthSessionResultOK;
//        }

//        private static void openGameServer()
//        {
//            if (SDG.Unturned.Provider.isServer || SDG.Unturned.Provider.isClient)
//            {
//                UnityEngine.Debug.LogError((object)"Failed to open game server: session already in progress.");
//            }
//            else
//            {
//                ESecurityMode security = ESecurityMode.LAN;
//                switch (Dedicator.serverVisibility)
//                {
//                    case ESteamServerVisibility.Internet:
//                        security = !SDG.Unturned.Provider.configData.Server.VAC_Secure ? ESecurityMode.INSECURE : ESecurityMode.SECURE;
//                        break;
//                    case ESteamServerVisibility.LAN:
//                        security = ESecurityMode.LAN;
//                        break;
//                }
//                if (security == ESecurityMode.INSECURE)
//                    CommandWindow.LogWarning((object)SDG.Unturned.Provider.localization.format("InsecureWarningText"));
//                try
//                {
//                    SDG.Unturned.Provider.provider.multiplayerService.serverMultiplayerService.open(SDG.Unturned.Provider.ip, SDG.Unturned.Provider.port, security);
//                }
//                catch (Exception ex)
//                {
//                    UnityEngine.Debug.Log((object)("Quit due to provider exception: " + ex.Message));
//                    Application.Quit();
//                    return;
//                }
//                SDG.Unturned.Provider.backendRealtimeSeconds = SteamGameServerUtils.GetServerRealTime();
//                if (SDG.Unturned.Provider.configData != null && SDG.Unturned.Provider.configData.Server.BattlEye_Secure && security == ESecurityMode.SECURE)
//                {
//                    string path = ReadWrite.PATH + "/BattlEye/BEServer_x64.dll";
//                    if (!File.Exists(path))
//                        path = ReadWrite.PATH + "/BattlEye/BEServer.dll";
//                    if (!File.Exists(path))
//                    {
//                        UnityEngine.Debug.LogError((object)"Missing BattlEye server library!");
//                        Application.Quit();
//                        return;
//                    }
//                    try
//                    {
//                        SDG.Unturned.Provider.battlEyeServerHandle = BEServer.LoadLibraryW(path);
//                        if (SDG.Unturned.Provider.battlEyeServerHandle != IntPtr.Zero)
//                        {
//                            BEServer.BEServerInitFn forFunctionPointer = Marshal.GetDelegateForFunctionPointer(BEServer.GetProcAddress(SDG.Unturned.Provider.battlEyeServerHandle, "Init"), typeof(BEServer.BEServerInitFn)) as BEServer.BEServerInitFn;
//                            if (forFunctionPointer != null)
//                            {
//                                SDG.Unturned.Provider.battlEyeServerInitData = new BEServer.BESV_GAME_DATA();
//                                SDG.Unturned.Provider.battlEyeServerInitData.pstrGameVersion = SDG.Unturned.Provider.APP_NAME + " " + SDG.Unturned.Provider.APP_VERSION;
//                                BEServer.BESV_GAME_DATA eyeServerInitData1 = SDG.Unturned.Provider.battlEyeServerInitData;
//                                // ISSUE: reference to a compiler-generated field
//                                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache3 == null)//                {
//                                    // ISSUE: reference to a compiler-generated field
//                                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache3 = new BEServer.BESV_GAME_DATA.PrintMessageFn(SDG.Unturned.Provider.battlEyeServerPrintMessage);
//                                }
//                                // ISSUE: reference to a compiler-generated field
//                                BEServer.BESV_GAME_DATA.PrintMessageFn fMgCache3 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache3;
//                                eyeServerInitData1.pfnPrintMessage = fMgCache3;
//                                BEServer.BESV_GAME_DATA eyeServerInitData2 = SDG.Unturned.Provider.battlEyeServerInitData;
//                                // ISSUE: reference to a compiler-generated field
//                                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache4 == null)//                {
//                                    // ISSUE: reference to a compiler-generated field
//                                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache4 = new BEServer.BESV_GAME_DATA.KickPlayerFn(SDG.Unturned.Provider.battlEyeServerKickPlayer);
//                                }
//                                // ISSUE: reference to a compiler-generated field
//                                BEServer.BESV_GAME_DATA.KickPlayerFn fMgCache4 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache4;
//                                eyeServerInitData2.pfnKickPlayer = fMgCache4;
//                                BEServer.BESV_GAME_DATA eyeServerInitData3 = SDG.Unturned.Provider.battlEyeServerInitData;
//                                // ISSUE: reference to a compiler-generated field
//                                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache5 == null)//                {
//                                    // ISSUE: reference to a compiler-generated field
//                                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache5 = new BEServer.BESV_GAME_DATA.SendPacketFn(SDG.Unturned.Provider.battlEyeServerSendPacket);
//                                }
//                                // ISSUE: reference to a compiler-generated field
//                                BEServer.BESV_GAME_DATA.SendPacketFn fMgCache5 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache5;
//                                eyeServerInitData3.pfnSendPacket = fMgCache5;
//                                SDG.Unturned.Provider.battlEyeServerRunData = new BEServer.BESV_BE_DATA();
//                                if (!forFunctionPointer(0, SDG.Unturned.Provider.battlEyeServerInitData, SDG.Unturned.Provider.battlEyeServerRunData))
//                                {
//                                    BEServer.FreeLibrary(SDG.Unturned.Provider.battlEyeServerHandle);
//                                    SDG.Unturned.Provider.battlEyeServerHandle = IntPtr.Zero;
//                                    UnityEngine.Debug.LogError((object)"Failed to call BattlEye server init!");
//                                    Application.Quit();
//                                    return;
//                                }
//                            }
//                            else
//                            {
//                                BEServer.FreeLibrary(SDG.Unturned.Provider.battlEyeServerHandle);
//                                SDG.Unturned.Provider.battlEyeServerHandle = IntPtr.Zero;
//                                UnityEngine.Debug.LogError((object)"Failed to get BattlEye server init delegate!");
//                                Application.Quit();
//                                return;
//                            }
//                        }
//                        else
//                        {
//                            UnityEngine.Debug.LogError((object)"Failed to load BattlEye server library!");
//                            Application.Quit();
//                            return;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        UnityEngine.Debug.LogError((object)"Unhandled exception when loading BattlEye server library!");
//                        UnityEngine.Debug.LogException(ex);
//                        Application.Quit();
//                        return;
//                    }
//                }
//                CommandWindow.Log((object)"Waiting for Steam servers...");
//            }
//        }

//        private static void closeGameServer()
//        {
//            if (!SDG.Unturned.Provider.isServer)
//            {
//                UnityEngine.Debug.LogError((object)"Failed to close game server: no session in progress.");
//            }
//            else
//            {
//                SDG.Unturned.Provider.broadcastServerShutdown();
//                SDG.Unturned.Provider._isServer = false;
//                SDG.Unturned.Provider.provider.multiplayerService.serverMultiplayerService.close();
//            }
//        }

//        public static bool getServerIsFavorited(uint ip, ushort port)
//        {
//            foreach (SDG.Unturned.Provider.CachedFavorite cachedFavorite in SDG.Unturned.Provider.cachedFavorites)
//            {
//                if (cachedFavorite.matchesServer(ip, port))
//                    return cachedFavorite.isFavorited;
//            }
//            for (int iGame = 0; iGame < SteamMatchmaking.GetFavoriteGameCount(); ++iGame)
//            {
//                AppId_t pnAppID;
//                uint pnIP;
//                ushort pnConnPort;
//                ushort pnQueryPort;
//                uint punFlags;
//                uint pRTime32LastPlayedOnServer;
//                SteamMatchmaking.GetFavoriteGame(iGame, out pnAppID, out pnIP, out pnConnPort, out pnQueryPort, out punFlags, out pRTime32LastPlayedOnServer);
//                if (((int)punFlags | (int)SDG.Unturned.Provider.STEAM_FAVORITE_FLAG_FAVORITE) == (int)punFlags && (int)pnIP == (int)ip && ((int)pnConnPort == (int)port || (int)pnQueryPort - 1 == (int)port))
//                    return true;
//            }
//            return false;
//        }

//        public static void setServerIsFavorited(uint ip, ushort port, bool newFavorited)
//        {
//            bool flag = false;
//            foreach (SDG.Unturned.Provider.CachedFavorite cachedFavorite in SDG.Unturned.Provider.cachedFavorites)
//            {
//                if (cachedFavorite.matchesServer(ip, port))
//                {
//                    cachedFavorite.isFavorited = newFavorited;
//                    flag = true;
//                    break;
//                }
//            }
//            if (!flag)
//                SDG.Unturned.Provider.cachedFavorites.Add(new SDG.Unturned.Provider.CachedFavorite()
//                {
//                    ip = ip,
//                    port = port,
//                    isFavorited = newFavorited
//                });
//            if (newFavorited)
//                SteamMatchmaking.AddFavoriteGame(SDG.Unturned.Provider.APP_ID, ip, port, (ushort)((uint)port + 1U), SDG.Unturned.Provider.STEAM_FAVORITE_FLAG_FAVORITE, SteamUtils.GetServerRealTime());
//            else
//                SteamMatchmaking.RemoveFavoriteGame(SDG.Unturned.Provider.APP_ID, ip, port, (ushort)((uint)port + 1U), SDG.Unturned.Provider.STEAM_FAVORITE_FLAG_FAVORITE);
//        }

//        public static void openURL(string url)
//        {
//            if (SteamUtils.IsOverlayEnabled())
//                SteamFriends.ActivateGameOverlayToWebPage(url);
//            else
//                Process.Start(url);
//        }

//        public static bool isCurrentServerFavorited
//        {
//            get
//            {
//                return SDG.Unturned.Provider.getServerIsFavorited(SDG.Unturned.Provider.favoriteIP, SDG.Unturned.Provider.favoritePort);
//            }
//        }

//        public static void toggleCurrentServerFavorited()
//        {
//            if (SDG.Unturned.Provider.isServer)
//                return;
//            SDG.Unturned.Provider.setServerIsFavorited(SDG.Unturned.Provider.favoriteIP, SDG.Unturned.Provider.favoritePort, !SDG.Unturned.Provider.isCurrentServerFavorited);
//        }

//        private static void onPersonaStateChange(PersonaStateChange_t callback)
//        {
//            if (callback.m_nChangeFlags != EPersonaChange.k_EPersonaChangeName || (long)callback.m_ulSteamID != (long)SDG.Unturned.Provider.client.m_SteamID)
//                return;
//            SDG.Unturned.Provider._clientName = SteamFriends.GetPersonaName();
//        }

//        private static void onGameServerChangeRequested(GameServerChangeRequested_t callback)
//        {
//            if (SDG.Unturned.Provider.isConnected)
//                return;
//            Terminal.print("onGameServerChangeRequested " + callback.m_rgchServer + " " + callback.m_rgchPassword, (string)null, "Steam", "<color=#2784c6>Steam</color>", true);
//            SteamConnectionInfo info = new SteamConnectionInfo(callback.m_rgchServer, callback.m_rgchPassword);
//            Terminal.print(((int)info.ip).ToString() + " " + Parser.getIPFromUInt32(info.ip) + " " + (object)info.port + " " + info.password, (string)null, "Steam", "<color=#2784c6>Steam</color>", true);
//            MenuPlayConnectUI.connect(info);
//        }

//        private static void onGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t callback)
//        {
//            if (SDG.Unturned.Provider.isConnected)
//                return;
//            Terminal.print("onGameRichPresenceJoinRequested " + callback.m_rgchConnect, (string)null, "Steam", "<color=#2784c6>Steam</color>", true);
//            uint ip;
//            ushort port;
//            string pass;
//            if (!CommandLine.tryGetConnect(callback.m_rgchConnect, out ip, out port, out pass))
//                return;
//            SteamConnectionInfo info = new SteamConnectionInfo(ip, port, pass);
//            Terminal.print(((int)info.ip).ToString() + " " + Parser.getIPFromUInt32(info.ip) + " " + (object)info.port + " " + info.password, (string)null, "Steam", "<color=#2784c6>Steam</color>", true);
//            MenuPlayConnectUI.connect(info);
//        }

//        public static float timeLastPacketWasReceivedFromServer { get; private set; }

//        public static float clientPredictedServersideTime
//        {
//            get
//            {
//                return SDG.Unturned.Provider.lastReceivedServersideTime + Time.realtimeSinceStartup - SDG.Unturned.Provider.timeLastPacketWasReceivedFromServer;
//            }
//        }

//        public static float ping
//        {
//            get
//            {
//                return SDG.Unturned.Provider._ping;
//            }
//        }

//        private static void lag(float value)
//        {
//            value = Mathf.Clamp01(value);
//            SDG.Unturned.Provider._ping = value;
//            for (int index = SDG.Unturned.Provider.pings.Length - 1; index > 0; --index)
//            {
//                SDG.Unturned.Provider.pings[index] = SDG.Unturned.Provider.pings[index - 1];
//                if ((double)SDG.Unturned.Provider.pings[index] > 1.0 / 1000.0)
//                    SDG.Unturned.Provider._ping += SDG.Unturned.Provider.pings[index];
//            }
//            SDG.Unturned.Provider._ping /= (float)SDG.Unturned.Provider.pings.Length;
//            SDG.Unturned.Provider.pings[0] = value;
//        }

//        private static byte[] openTicket()
//        {
//            if (SDG.Unturned.Provider.ticketHandle != HAuthTicket.Invalid)
//                return (byte[])null;
//            byte[] pTicket = new byte[1024];
//            uint pcbTicket;
//            SDG.Unturned.Provider.ticketHandle = SteamUser.GetAuthSessionTicket(pTicket, pTicket.Length, out pcbTicket);
//            if (pcbTicket == 0U)
//                return (byte[])null;
//            byte[] numArray = new byte[(IntPtr)pcbTicket];
//            Buffer.BlockCopy((Array)pTicket, 0, (Array)numArray, 0, (int)pcbTicket);
//            return numArray;
//        }

//        private static void closeTicket()
//        {
//            if (SDG.Unturned.Provider.ticketHandle == HAuthTicket.Invalid)
//                return;
//            SteamUser.CancelAuthTicket(SDG.Unturned.Provider.ticketHandle);
//            SDG.Unturned.Provider.ticketHandle = HAuthTicket.Invalid;
//        }

//        public static IProvider provider { get; protected set; }

//        public static bool isInitialized
//        {
//            get
//            {
//                return SDG.Unturned.Provider._isInitialized;
//            }
//        }

//        public static uint time
//        {
//            get
//            {
//                return SDG.Unturned.Provider._time + (uint)((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.timeOffset);
//            }
//            set
//            {
//                SDG.Unturned.Provider._time = value;
//                SDG.Unturned.Provider.timeOffset = (uint)Time.realtimeSinceStartup;
//            }
//        }

//        public static uint backendRealtimeSeconds
//        {
//            get
//            {
//                return SDG.Unturned.Provider.initialBackendRealtimeSeconds + (uint)((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.initialLocalRealtime);
//            }
//            private set
//            {
//                SDG.Unturned.Provider.initialBackendRealtimeSeconds = value;
//                SDG.Unturned.Provider.initialLocalRealtime = Time.realtimeSinceStartup;
//                if (SDG.Unturned.Provider.onBackendRealtimeAvailable == null)
//                    return;
//                SDG.Unturned.Provider.onBackendRealtimeAvailable();
//            }
//        }

//        public static DateTime backendRealtimeDate
//        {
//            get
//            {
//                return SDG.Unturned.Provider.unixEpochDateTime.AddSeconds((double)SDG.Unturned.Provider.backendRealtimeSeconds);
//            }
//        }

//        public static bool isBackendRealtimeAvailable
//        {
//            get
//            {
//                return SDG.Unturned.Provider.initialBackendRealtimeSeconds > 0U;
//            }
//        }

//        [DebuggerHidden]
//        public IEnumerator close(CSteamID steamID)
//        {
//            // ISSUE: object of a compiler-generated type is created
//            return (IEnumerator)new SDG.Unturned.Provider.\u003Cclose\u003Ec__Iterator0()
//          {
//                steamID = steamID//      };
//        }

//        private void exit()
//        {
//            Application.Quit();
//        }

//        private static void onAPIWarningMessage(int severity, StringBuilder warning)
//        {
//            CommandWindow.LogWarning((object)"Steam API Warning Message:");
//            CommandWindow.LogWarning((object)("Severity: " + (object)severity));
//            CommandWindow.LogWarning((object)("Warning: " + (object)warning));
//        }

//        private void updateDebug()
//        {
//            ++SDG.Unturned.Provider.debugUpdates;
//            if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.debugLastUpdate <= 1.0)
//                return;
//            SDG.Unturned.Provider.debugUPS = (int)((double)SDG.Unturned.Provider.debugUpdates / ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.debugLastUpdate));
//            SDG.Unturned.Provider.debugLastUpdate = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.debugUpdates = 0;
//        }

//        private void tickDebug()
//        {
//            ++SDG.Unturned.Provider.debugTicks;
//            if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.debugLastTick <= 1.0)
//                return;
//            SDG.Unturned.Provider.debugTPS = (int)((double)SDG.Unturned.Provider.debugTicks / ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.debugLastTick));
//            SDG.Unturned.Provider.debugLastTick = Time.realtimeSinceStartup;
//            SDG.Unturned.Provider.debugTicks = 0;
//        }

//        [DebuggerHidden]
//        private IEnumerator downloadIcon(SDG.Unturned.Provider.IconQueryParams iconQueryParams)
//        {
//            // ISSUE: object of a compiler-generated type is created
//            return (IEnumerator)new SDG.Unturned.Provider.\u003CdownloadIcon\u003Ec__Iterator1()
//          {
//                iconQueryParams = iconQueryParams//      };
//        }

//        public static void refreshIcon(SDG.Unturned.Provider.IconQueryParams iconQueryParams)
//        {
//            SDG.Unturned.Provider.steam.StartCoroutine("downloadIcon", (object)iconQueryParams);
//        }

//        private void Update()
//        {
//            if (!SDG.Unturned.Provider.isInitialized)
//                return;
//            if (SDG.Unturned.Provider.battlEyeClientHandle != IntPtr.Zero && SDG.Unturned.Provider.battlEyeClientRunData != null && SDG.Unturned.Provider.battlEyeClientRunData.pfnRun != null)
//                SDG.Unturned.Provider.battlEyeClientRunData.pfnRun();
//            if (SDG.Unturned.Provider.configData != null && SDG.Unturned.Provider.configData.Server.BattlEye_Secure && (SDG.Unturned.Provider.battlEyeServerHandle != IntPtr.Zero && SDG.Unturned.Provider.battlEyeServerRunData != null) && SDG.Unturned.Provider.battlEyeServerRunData.pfnRun != null)
//                SDG.Unturned.Provider.battlEyeServerRunData.pfnRun();
//            if (SDG.Unturned.Provider.isConnected)
//                SDG.Unturned.Provider.listen();
//            this.updateDebug();
//            SDG.Unturned.Provider.provider.update();
//            if (SDG.Unturned.Provider.countShutdownTimer > 0)
//            {
//                if ((double)Time.realtimeSinceStartup - (double)SDG.Unturned.Provider.lastTimerMessage > 1.0)
//                {
//                    SDG.Unturned.Provider.lastTimerMessage = Time.realtimeSinceStartup;
//                    --SDG.Unturned.Provider.countShutdownTimer;
//                    if (SDG.Unturned.Provider.countShutdownTimer == 300 || SDG.Unturned.Provider.countShutdownTimer == 60 || (SDG.Unturned.Provider.countShutdownTimer == 30 || SDG.Unturned.Provider.countShutdownTimer == 15) || (SDG.Unturned.Provider.countShutdownTimer == 3 || SDG.Unturned.Provider.countShutdownTimer == 2 || SDG.Unturned.Provider.countShutdownTimer == 1))
//                        ChatManager.say(SDG.Unturned.Provider.localization.format("Shutdown", (object)SDG.Unturned.Provider.countShutdownTimer), ChatManager.welcomeColor, false);
//                }
//            }
//            else if (SDG.Unturned.Provider.countShutdownTimer == 0)
//            {
//                SDG.Unturned.Provider.countShutdownTimer = -1;
//                int size;
//                byte[] bytes = SteamPacker.getBytes(0, out size, (object)(byte)0, (object)SDG.Unturned.Provider.shutdownMessage);
//                for (int index = 0; index < SDG.Unturned.Provider.clients.Count; ++index)
//                {
//                    SteamGameServer.EndAuthSession(SDG.Unturned.Provider.clients[index].playerID.steamID);
//                    SDG.Unturned.Provider.send(SDG.Unturned.Provider.clients[index].playerID.steamID, ESteamPacket.SHUTDOWN, bytes, size, 0);
//                }
//                this.Invoke("exit", 0.5f);
//            }
//            RazerChroma.tick();
//        }

//        private void FixedUpdate()
//        {
//            if (!SDG.Unturned.Provider.isInitialized)
//                return;
//            this.tickDebug();
//        }

//        [DebuggerHidden]
//        private IEnumerator UpdateServerListBlacklist()
//        {
//            // ISSUE: object of a compiler-generated type is created
//            // ISSUE: variable of a compiler-generated type
//            SDG.Unturned.Provider.\u003CUpdateServerListBlacklist\u003Ec__Iterator2 blacklistCIterator2 = new SDG.Unturned.Provider.\u003CUpdateServerListBlacklist\u003Ec__Iterator2();
//            return (IEnumerator)blacklistCIterator2;
//        }

//        public void awake()
//        {
//            if (ReadWrite.fileExists("/Status.json", false, true))
//            {
//                try
//                {
//                    SDG.Unturned.Provider._statusData = ReadWrite.deserializeJSON<StatusData>("/Status.json", false, true);
//                }
//                catch
//                {
//                    SDG.Unturned.Provider._statusData = (StatusData)null;
//                }
//                if (SDG.Unturned.Provider.statusData == null)
//                    SDG.Unturned.Provider._statusData = new StatusData();
//            }
//            else
//                SDG.Unturned.Provider._statusData = new StatusData();
//            if (!Dedicator.isDedicated)
//                this.StartCoroutine(this.UpdateServerListBlacklist());
//            SDG.Unturned.Provider.APP_VERSION = string.Format("3.{0}.{1}.{2}", (object)SDG.Unturned.Provider.statusData.Game.Major_Version, (object)SDG.Unturned.Provider.statusData.Game.Minor_Version, (object)SDG.Unturned.Provider.statusData.Game.Patch_Version);
//            Terminal.print("App: " + SDG.Unturned.Provider.APP_NAME + " " + SDG.Unturned.Provider.APP_VERSION, (string)null, "Steam", "<color=#2784c6>Steam</color>", true);
//            if (SDG.Unturned.Provider.isInitialized)
//            {
//                UnityEngine.Object.Destroy((UnityEngine.Object)this.gameObject);
//            }
//            else
//            {
//                SDG.Unturned.Provider._isInitialized = true;
//                UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)this.gameObject);
//                SDG.Unturned.Provider.steam = this;
//                LevelLoaded onLevelLoaded = Level.onLevelLoaded;
//                // ISSUE: reference to a compiler-generated field
//                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache6 == null)//        {
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache6 = new LevelLoaded(SDG.Unturned.Provider.onLevelLoaded);
//                }
//                // ISSUE: reference to a compiler-generated field
//                LevelLoaded fMgCache6 = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache6;
//                Level.onLevelLoaded = onLevelLoaded + fMgCache6;
//                if (Dedicator.isDedicated)
//                {
//                    try
//                    {
//                        SDG.Unturned.Provider.provider = (IProvider)new SDG.SteamworksProvider.SteamworksProvider(new SteamworksAppInfo(SDG.Unturned.Provider.APP_ID.m_AppId, SDG.Unturned.Provider.APP_NAME, SDG.Unturned.Provider.APP_VERSION, true));
//                        SDG.Unturned.Provider.provider.intialize();
//                    }
//                    catch (Exception ex)
//                    {
//                        UnityEngine.Debug.Log((object)("Quit due to provider exception: " + ex.Message));
//                        Application.Quit();
//                        return;
//                    }
//                    if (!CommandLine.tryGetLanguage(out SDG.Unturned.Provider._language, out SDG.Unturned.Provider._path))
//                    {
//                        SDG.Unturned.Provider._path = ReadWrite.PATH + "/Localization/";
//                        SDG.Unturned.Provider._language = "English";
//                    }
//                    SDG.Unturned.Provider.localizationRoot = SDG.Unturned.Provider.path + SDG.Unturned.Provider.language;
//                    SDG.Unturned.Provider.localization = Localization.read("/Server/ServerConsole.dat");
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache7 == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache7 = new Callback<P2PSessionConnectFail_t>.DispatchDelegate(SDG.Unturned.Provider.onP2PSessionConnectFail);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.p2pSessionConnectFail = Callback<P2PSessionConnectFail_t>.CreateGameServer(SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache7);
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache8 == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache8 = new Callback<ValidateAuthTicketResponse_t>.DispatchDelegate(SDG.Unturned.Provider.onValidateAuthTicketResponse);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.validateAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.CreateGameServer(SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache8);
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache9 == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache9 = new Callback<GSClientGroupStatus_t>.DispatchDelegate(SDG.Unturned.Provider.onClientGroupStatus);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.clientGroupStatus = Callback<GSClientGroupStatus_t>.CreateGameServer(SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache9);
//                    SDG.Unturned.Provider._isPro = true;
//                    CommandWindow.Log((object)SDG.Unturned.Provider.APP_VERSION);
//                    SDG.Unturned.Provider.maxPlayers = (byte)8;
//                    SDG.Unturned.Provider.queueSize = (byte)8;
//                    SDG.Unturned.Provider.serverName = "Unturned";
//                    SDG.Unturned.Provider.serverPassword = string.Empty;
//                    SDG.Unturned.Provider.ip = 0U;
//                    SDG.Unturned.Provider.port = (ushort)27015;
//                    SDG.Unturned.Provider.map = "PEI";
//                    SDG.Unturned.Provider.isPvP = true;
//                    SDG.Unturned.Provider.isWhitelisted = false;
//                    SDG.Unturned.Provider.hideAdmins = false;
//                    SDG.Unturned.Provider.hasCheats = false;
//                    SDG.Unturned.Provider.filterName = false;
//                    SDG.Unturned.Provider.mode = EGameMode.NORMAL;
//                    SDG.Unturned.Provider.isGold = false;
//                    SDG.Unturned.Provider.gameMode = (GameMode)null;
//                    SDG.Unturned.Provider.selectedGameModeName = (string)null;
//                    SDG.Unturned.Provider.cameraMode = ECameraMode.FIRST;
//                    Commander.init();
//                    SteamWhitelist.load();
//                    SteamBlacklist.load();
//                    SteamAdminlist.load();
//                    foreach (string command in CommandLine.getCommands())
//                        Commander.execute(CSteamID.Nil, command);
//                    if (ServerSavedata.fileExists("/Server/Commands.dat"))
//                    {
//                        FileStream fileStream = (FileStream)null;
//                        StreamReader streamReader = (StreamReader)null;
//                        try
//                        {
//                            fileStream = new FileStream(ReadWrite.PATH + "/Servers/" + SDG.Unturned.Provider.serverID + "/Server/Commands.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
//                            streamReader = new StreamReader((Stream)fileStream);
//                            string command;
//                            while ((command = streamReader.ReadLine()) != null)
//                                Commander.execute(CSteamID.Nil, command);
//                        }
//                        finally
//                        {
//                            fileStream?.Close();
//                            streamReader?.Close();
//                        }
//                    }
//                    else
//                        ServerSavedata.writeData("/Server/Commands.dat", new Data());
//                    if (!ServerSavedata.folderExists("/Bundles"))
//                        ServerSavedata.createFolder("/Bundles");
//                    if (!ServerSavedata.folderExists("/Maps"))
//                        ServerSavedata.createFolder("/Maps");
//                    if (!ServerSavedata.folderExists("/Workshop/Content"))
//                        ServerSavedata.createFolder("/Workshop/Content");
//                    if (!ServerSavedata.folderExists("/Workshop/Maps"))
//                        ServerSavedata.createFolder("/Workshop/Maps");
//                    if (ServerSavedata.fileExists("/Config.json"))
//                    {
//                        try
//                        {
//                            SDG.Unturned.Provider._configData = ServerSavedata.deserializeJSON<ConfigData>("/Config.json");
//                        }
//                        catch
//                        {
//                            SDG.Unturned.Provider._configData = (ConfigData)null;
//                        }
//                        if (SDG.Unturned.Provider.configData == null)
//                            SDG.Unturned.Provider._configData = new ConfigData();
//                    }
//                    else
//                        SDG.Unturned.Provider._configData = new ConfigData();
//                    ServerSavedata.serializeJSON<ConfigData>("/Config.json", SDG.Unturned.Provider.configData);
//                    switch (SDG.Unturned.Provider.mode)
//                    {
//                        case EGameMode.EASY:
//                            SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Easy;
//                            break;
//                        case EGameMode.NORMAL:
//                            SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Normal;
//                            break;
//                        case EGameMode.HARD:
//                            SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Hard;
//                            break;
//                        default:
//                            SDG.Unturned.Provider._modeConfigData = new ModeConfigData(SDG.Unturned.Provider.mode);
//                            break;
//                    }
//                    IServerMultiplayerService multiplayerService = SDG.Unturned.Provider.provider.multiplayerService.serverMultiplayerService;
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheA == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheA = new ServerMultiplayerServiceReadyHandler(SDG.Unturned.Provider.handleServerReady);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    ServerMultiplayerServiceReadyHandler fMgCacheA = SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheA;
//                    multiplayerService.ready += fMgCacheA;
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheB == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheB = new DedicatedUGCInstalledHandler(SDG.Unturned.Provider.onDedicatedUGCInstalled);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    DedicatedUGC.installed += SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheB;
//                }
//                else
//                {
//                    try
//                    {
//                        SDG.Unturned.Provider.provider = (IProvider)new SDG.SteamworksProvider.SteamworksProvider(new SteamworksAppInfo(SDG.Unturned.Provider.APP_ID.m_AppId, SDG.Unturned.Provider.APP_NAME, SDG.Unturned.Provider.APP_VERSION, false));
//                        SDG.Unturned.Provider.provider.intialize();
//                    }
//                    catch (Exception ex)
//                    {
//                        UnityEngine.Debug.Log((object)("Quit due to provider exception: " + ex.Message));
//                        Application.Quit();
//                        return;
//                    }
//                    SDG.Unturned.Provider.backendRealtimeSeconds = SteamUtils.GetServerRealTime();
//                    SDG.Unturned.Provider.apiWarningMessageHook = new SteamAPIWarningMessageHook_t(SDG.Unturned.Provider.onAPIWarningMessage);
//                    SteamUtils.SetWarningMessageHook(SDG.Unturned.Provider.apiWarningMessageHook);
//                    SDG.Unturned.Provider._time = SteamUtils.GetServerRealTime();
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheC == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheC = new Callback<PersonaStateChange_t>.DispatchDelegate(SDG.Unturned.Provider.onPersonaStateChange);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.personaStateChange = Callback<PersonaStateChange_t>.Create(SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheC);
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheD == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheD = new Callback<GameServerChangeRequested_t>.DispatchDelegate(SDG.Unturned.Provider.onGameServerChangeRequested);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.gameServerChangeRequested = Callback<GameServerChangeRequested_t>.Create(SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheD);
//                    // ISSUE: reference to a compiler-generated field
//                    if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheE == null)//          {
//                        // ISSUE: reference to a compiler-generated field
//                        SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheE = new Callback<GameRichPresenceJoinRequested_t>.DispatchDelegate(SDG.Unturned.Provider.onGameRichPresenceJoinRequested);
//                    }
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.gameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheE);
//                    SDG.Unturned.Provider._user = SteamUser.GetSteamID();
//                    SDG.Unturned.Provider._client = SDG.Unturned.Provider.user;
//                    SDG.Unturned.Provider._clientHash = Hash.SHA1(SDG.Unturned.Provider.client);
//                    SDG.Unturned.Provider._clientName = SteamFriends.GetPersonaName();
//                    SDG.Unturned.Provider.provider.statisticsService.userStatisticsService.requestStatistics();
//                    SDG.Unturned.Provider.provider.statisticsService.globalStatisticsService.requestStatistics();
//                    SDG.Unturned.Provider.provider.workshopService.refreshUGC();
//                    SDG.Unturned.Provider.provider.workshopService.refreshPublished();
//                    SDG.Unturned.Provider._isPro = SteamApps.BIsSubscribedApp(SDG.Unturned.Provider.PRO_ID);
//                    SDG.Unturned.Provider.isLoadingInventory = true;
//                    SteamInventory.GrantPromoItems(out SDG.Unturned.Provider.provider.economyService.promoResult);
//                    if (CommandLine.tryGetLanguage(out SDG.Unturned.Provider._language, out SDG.Unturned.Provider._path))
//                    {
//                        SDG.Unturned.Provider.localizationRoot = SDG.Unturned.Provider.path + SDG.Unturned.Provider.language;
//                    }
//                    else
//                    {
//                        string steamUiLanguage = SteamUtils.GetSteamUILanguage();
//                        SDG.Unturned.Provider._language = steamUiLanguage.Substring(0, 1).ToUpper() + steamUiLanguage.Substring(1, steamUiLanguage.Length - 1).ToLower();
//                        bool flag1 = false;
//                        foreach (SteamContent steamContent in SDG.Unturned.Provider.provider.workshopService.ugc)
//                        {
//                            if (steamContent.type == ESteamUGCType.LOCALIZATION && ReadWrite.folderExists(steamContent.path + "/" + SDG.Unturned.Provider.language, false))
//                            {
//                                SDG.Unturned.Provider._path = steamContent.path + "/";
//                                SDG.Unturned.Provider.localizationRoot = SDG.Unturned.Provider.path + SDG.Unturned.Provider.language;
//                                flag1 = true;
//                                break;
//                            }
//                        }
//                        if (!flag1)
//                        {
//                            SDG.Unturned.Provider._path = ReadWrite.PATH + "/Localization/";
//                            SDG.Unturned.Provider.localizationRoot = SDG.Unturned.Provider.path + SDG.Unturned.Provider.language;
//                            flag1 = ReadWrite.folderExists("/Localization/" + SDG.Unturned.Provider.language);
//                            if (!ReadWrite.folderExists("/Localization/" + SDG.Unturned.Provider.language))
//                                SDG.Unturned.Provider._language = "English";
//                        }
//                        if (!flag1)
//                        {
//                            foreach (SteamContent steamContent in SDG.Unturned.Provider.provider.workshopService.ugc)
//                            {
//                                if (steamContent.type == ESteamUGCType.LOCALIZATION)
//                                {
//                                    bool flag2 = ReadWrite.folderExists(steamContent.path + "/Editor", false);
//                                    bool flag3 = ReadWrite.folderExists(steamContent.path + "/Menu", false);
//                                    bool flag4 = ReadWrite.folderExists(steamContent.path + "/Player", false);
//                                    bool flag5 = ReadWrite.folderExists(steamContent.path + "/Server", false);
//                                    bool flag6 = ReadWrite.folderExists(steamContent.path + "/Shared", false);
//                                    if (flag2 && flag3 && (flag4 && flag5) && flag6)
//                                    {
//                                        SDG.Unturned.Provider._path = (string)null;
//                                        SDG.Unturned.Provider.localizationRoot = steamContent.path;
//                                        flag1 = true;
//                                    }
//                                }
//                            }
//                        }
//                        if (!flag1)
//                        {
//                            SDG.Unturned.Provider._path = ReadWrite.PATH + "/Localization/";
//                            SDG.Unturned.Provider._language = "English";
//                            SDG.Unturned.Provider.localizationRoot = SDG.Unturned.Provider.path + SDG.Unturned.Provider.language;
//                        }
//                    }
//                    SDG.Unturned.Provider.provider.economyService.loadTranslationEconInfo();
//                    SDG.Unturned.Provider.localization = Localization.read("/Server/ServerConsole.dat");
//                    SDG.Unturned.Provider.updateRichPresence();
//                    SDG.Unturned.Provider._configData = new ConfigData();
//                    SDG.Unturned.Provider._modeConfigData = SDG.Unturned.Provider.configData.Normal;
//                    if (ReadWrite.fileExists("/Preferences.json", false, true))
//                    {
//                        try
//                        {
//                            SDG.Unturned.Provider._preferenceData = ReadWrite.deserializeJSON<PreferenceData>("/Preferences.json", false, true);
//                        }
//                        catch
//                        {
//                            SDG.Unturned.Provider._preferenceData = (PreferenceData)null;
//                        }
//                        if (SDG.Unturned.Provider.preferenceData == null)
//                            SDG.Unturned.Provider._preferenceData = new PreferenceData();
//                    }
//                    else
//                        SDG.Unturned.Provider._preferenceData = new PreferenceData();
//                    ReadWrite.serializeJSON<PreferenceData>("/Preferences.json", false, true, SDG.Unturned.Provider.preferenceData);
//                    if (ReadWrite.fileExists("/StreamerNames.json", false, true))
//                    {
//                        try
//                        {
//                            SDG.Unturned.Provider.streamerNames = ReadWrite.deserializeJSON<List<string>>("/StreamerNames.json", false, true);
//                        }
//                        catch
//                        {
//                            SDG.Unturned.Provider.streamerNames = (List<string>)null;
//                        }
//                        if (SDG.Unturned.Provider.streamerNames == null)
//                            SDG.Unturned.Provider.streamerNames = new List<string>();
//                    }
//                    else
//                        SDG.Unturned.Provider.streamerNames = new List<string>();
//                    RazerChroma.init();
//                }
//            }
//        }

//        public void start()
//        {
//            // ISSUE: reference to a compiler-generated field
//            if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheF == null)//      {
//                // ISSUE: reference to a compiler-generated field
//                SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheF = new TranslationRegisteredHandler(SDG.Unturned.Provider.handleTranslationRegistered);
//            }
//            // ISSUE: reference to a compiler-generated field
//            Translator.translationRegistered += SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cacheF;
//            if (ContinuousIntegration.isRunning)
//                CommandWindow.Log((object)"Running CI");
//            if (Dedicator.isDedicated)
//            {
//                if (Translator.isOriginLanguage(Translator.language) && !Translator.isOriginLanguage(SDG.Unturned.Provider.language))
//                    Translator.language = SDG.Unturned.Provider.language;
//                Translator.registerTranslationDirectory(ReadWrite.PATH + "/Translations");
//            }
//            else
//            {
//                if (Translator.isOriginLanguage(SDG.Unturned.Provider.language))
//                {
//                    if (File.Exists(ReadWrite.PATH + "/Cloud/Translations.config"))
//                    {
//                        using (StreamReader input = new StreamReader(ReadWrite.PATH + "/Cloud/Translations.config"))
//                        {
//                            IFormattedFileReader formattedFileReader = (IFormattedFileReader)new KeyValueTableReader(input);
//                            if (formattedFileReader != null)
//                            {
//                                string str = formattedFileReader.readValue<string>("Language");
//                                if (!string.IsNullOrEmpty(str))
//                                    Translator.language = str;
//                            }
//                        }
//                    }
//                    else if (!Translator.isOriginLanguage(SDG.Unturned.Provider.language))
//                        Translator.language = SDG.Unturned.Provider.language;
//                }
//                Translator.registerTranslationDirectory(ReadWrite.PATH + "/Translations");
//                // ISSUE: reference to a compiler-generated field
//                if (SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache10 == null)//        {
//                    // ISSUE: reference to a compiler-generated field
//                    SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache10 = new LanguageChangedHandler(SDG.Unturned.Provider.handleLanguageChanged);
//                }
//                // ISSUE: reference to a compiler-generated field
//                Translator.languageChanged += SDG.Unturned.Provider.\u003C\u003Ef__mg\u0024cache10;
//                if (SDG.Unturned.Provider.provider.workshopService.ugc == null || SDG.Unturned.Provider.provider.workshopService.ugc == null)
//                    return;
//                for (int index = 0; index < SDG.Unturned.Provider.provider.workshopService.ugc.Count; ++index)
//                {
//                    SteamContent steamContent = SDG.Unturned.Provider.provider.workshopService.ugc[index];
//                    if (Directory.Exists(steamContent.path + "/Translations"))
//                        Translator.registerTranslationDirectory(steamContent.path + "/Translations");
//                    if (Directory.Exists(steamContent.path + "/Content"))
//                        Assets.searchForAndLoadContent(steamContent.path + "/Content");
//                }
//            }
//        }

//        public void unityStart()
//        {
//            if (Dedicator.isDedicated)
//                ;
//        }

//        private void OnApplicationQuit()
//        {
//            if (!Dedicator.isDedicated)
//            {
//                if (!Translator.isOriginLanguage(Translator.language))
//                {
//                    string path = ReadWrite.PATH + "/Cloud/Translations.config";
//                    string directoryName = Path.GetDirectoryName(path);
//                    if (!Directory.Exists(directoryName))
//                        Directory.CreateDirectory(directoryName);
//                    using (StreamWriter writer = new StreamWriter(path))
//                    {
//                        IFormattedFileWriter formattedFileWriter = (IFormattedFileWriter)new KeyValueTableWriter(writer);
//                        formattedFileWriter.writeKey("Language");
//                        formattedFileWriter.writeValue<string>(Translator.language);
//                    }
//                }
//                RazerChroma.shutdown();
//            }
//            if (!SDG.Unturned.Provider.isInitialized)
//                return;
//            if (!SDG.Unturned.Provider.isServer && SDG.Unturned.Provider.isPvP && (SDG.Unturned.Provider.clients.Count > 1 && (UnityEngine.Object)Player.player != (UnityEngine.Object)null) && (!Player.player.movement.isSafe && !Player.player.life.isDead))
//            {
//                Application.CancelQuit();
//            }
//            else
//            {
//                SDG.Unturned.Provider.disconnect();
//                SDG.Unturned.Provider.provider.shutdown();
//            }
//        }

//        public delegate void LoginSpawningHandler(
//          SteamPlayerID playerID,
//          ref Vector3 point,
//          ref float yaw,
//          ref EPlayerStance initialStance,
//          ref bool needsNewSpawnpoint);

//        public delegate void ServerWritingPacketHandler(
//          CSteamID remoteSteamId,
//          ESteamPacket type,
//          byte[] payload,
//          int size,
//          int channel);

//        private class WorkshopRequestLog
//        {
//            public CSteamID sender;
//            public float realTime;
//        }

//        private class CachedWorkshopResponse
//        {
//            public List<PublishedFileId_t> publishedFileIds = new List<PublishedFileId_t>();
//            public CSteamID server;
//            public float realTime;
//        }

//        public delegate void ServerReadingPacketHandler(
//          CSteamID remoteSteamId,
//          byte[] payload,
//          int offset,
//          int size,
//          int channel);

//        public delegate void ServerConnected(CSteamID steamID);

//        public delegate void ServerDisconnected(CSteamID steamID);

//        public delegate void ServerHosted();

//        public delegate void ServerShutdown();

//        public delegate void CheckValid(ValidateAuthTicketResponse_t callback, ref bool isValid);

//        public delegate void CheckValidWithExplanation(
//          ValidateAuthTicketResponse_t callback,
//          ref bool isValid,
//          ref string explanation);

//        public delegate void CheckBanStatusHandler(
//          CSteamID steamID,
//          uint remoteIP,
//          ref bool isBanned,
//          ref string banReason,
//          ref uint banRemainingDuration);

//        public delegate void RequestBanPlayerHandler(
//          CSteamID instigator,
//          CSteamID playerToBan,
//          uint ipToBan,
//          ref string reason,
//          ref uint duration,
//          ref bool shouldVanillaBan);

//        public delegate void RequestUnbanPlayerHandler(
//          CSteamID instigator,
//          CSteamID playerToUnban,
//          ref bool shouldVanillaUnban);

//        public delegate void QueuePositionUpdated();

//        private class CachedFavorite
//        {
//            public uint ip;
//            public ushort port;
//            public bool isFavorited;

//            public bool matchesServer(uint ip, ushort port)
//            {
//                if ((int)this.ip == (int)ip)
//                    return (int)this.port == (int)port;
//                return false;
//            }
//        }

//        public delegate void ClientConnected();

//        public delegate void ClientDisconnected();

//        public delegate void EnemyConnected(SteamPlayer player);

//        public delegate void EnemyDisconnected(SteamPlayer player);

//        public delegate void BackendRealtimeAvailableHandler();

//        public delegate void IconQueryCallback(Texture2D icon);

//        public struct IconQueryParams
//        {
//            public string url;
//            public SDG.Unturned.Provider.IconQueryCallback callback;
//            public bool shouldCache;
//        }
//    }//}