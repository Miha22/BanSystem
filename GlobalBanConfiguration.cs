using Rocket.API;

namespace BanSystem
{
    public class GlobalBanConfiguration : IRocketPluginConfiguration
    {
        public bool Proxy_Protection;
        public int Socket_Port;
        public string API_Key;
        public bool Bot_Enabled;
        public string Bot_Token;
        public string Cmd_Prefix;
        //public Discord.Color Discord_Ban_Color;
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int KickInterval = 10;
        public int DatabasePort;
        //public bool KickInsteadReject;

        public void LoadDefaults()
        {
            Proxy_Protection = false;
            Bot_Enabled = false;
            DatabaseAddress = "localhost";
            DatabaseUsername = "unturned";
            DatabasePassword = "password";
            DatabaseName = "unturned";
            KickInterval = 10;
            DatabaseTableName = "banlist";
            DatabasePort = 3306;
            //KickInsteadReject = false;
            API_Key = "NjE4NTpHNXZ1Z0ZaVkU3Mmc2SVJLN0dFWjRTWlVUYzJJRGQ2WQ==";
            Bot_Token = "";
            Cmd_Prefix = "$";
            //Discord_Ban_Color = Discord.Color.Default;
            //Discord_Bot_Enabled = true;
            Socket_Port = 18000;
        }
    }
}
