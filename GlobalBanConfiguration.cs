using Rocket.API;

namespace BanSystem
{
    public class GlobalBanConfiguration : IRocketPluginConfiguration
    {
        //public bool IPcheck_Enabled;
        //public bool WebHook_Enabled;
        //public int Socket_Port;
        public string API_Key;
        // public bool Bot_Enabled;
        //public string Bot_Token;
        //public string Cmd_Prefix;
        public string Webhook;
        public bool ShowConnectInfo;
        //public bool LimbVPNs;
        //public string WebhookName;
        //public Discord.Color Discord_Ban_Color;
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string GlobalDatabaseTableName;
        public string LocalDatabaseTableName;
        public string DatabaseTableNameWhites;
        //public int KickInterval = 10;
        public int DatabasePort;
        //public bool KickInsteadReject;

        public void LoadDefaults()
        {
            Webhook = "https://discordapp.com/api/webhooks/617201018811318277/u2RV4vbtI3_r4k0mHMjfmGx_UaSt65s--m6ExTDEGEEgW0gtcsQyWlACxYLMZXAOmlEt";
            //Bot_Enabled = false;
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            //KickInterval = 10;
            GlobalDatabaseTableName = "bansystem.globalbanlist";
            LocalDatabaseTableName = "bansystem.localbanlist1";
            DatabaseTableNameWhites = "bansystem.whitelist";
            DatabasePort = 3306;
            //WebhookName = "Ban";
            //KickInsteadReject = false;
            ShowConnectInfo = true;
            API_Key = "NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ==";
            //Bot_Token = "";
            //Cmd_Prefix = "$";
            //Discord_Ban_Color = Discord.Color.Default;
            //Discord_Bot_Enabled = true;
            //Socket_Port = 18000;
        }
    }
}
