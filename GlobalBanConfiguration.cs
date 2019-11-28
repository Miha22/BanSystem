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
        //public string Webhook;
        //public string WebhookName;
        //public Discord.Color Discord_Ban_Color;
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public string DatabaseTableNameWhites;
        //public int KickInterval = 10;
        public int DatabasePort;
        //public bool KickInsteadReject;

        public void LoadDefaults()
        {
            //Webhook = "https://discordapp.com/api/webhooks/617201018811318277/u2RV4vbtI3_r4k0mHMjfmGx_UaSt65s--m6ExTDEGEEgW0gtcsQyWlACxYLMZXAOmlEt";
            //Bot_Enabled = false;
            DatabaseAddress = "127.0.0.1";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
           //KickInterval = 10;
            DatabaseTableName = "bansystem.banlist";
            DatabaseTableNameWhites = "bansystem.whitelist";
            DatabasePort = 3306;
            //WebhookName = "Ban";
            //KickInsteadReject = false;
            API_Key = "Njg1MzpvaDdFQmdpaG1GelZoS0E1OVJ2SDBuNVBVMUtIOTRjNw==";
            //Bot_Token = "";
            //Cmd_Prefix = "$";
            //Discord_Ban_Color = Discord.Color.Default;
            //Discord_Bot_Enabled = true;
            //Socket_Port = 18000;
        }
    }
}
