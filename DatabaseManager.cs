using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Steamworks;
using System;
using System.Text.RegularExpressions;
//using System.Text.RegularExpressions;

namespace BanSystem
{
    //https://rutracker.org/forum/viewtopic.php?t=5542725
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            _ = new I18N.West.CP1250();
            CheckSchema();
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
                    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", GlobalBan.Instance.Configuration.Instance.DatabaseAddress, GlobalBan.Instance.Configuration.Instance.DatabaseName, GlobalBan.Instance.Configuration.Instance.DatabaseUsername, GlobalBan.Instance.Configuration.Instance.DatabasePassword, GlobalBan.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public bool IsBanned(CSteamID steamId)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                SteamGameServerNetworking.GetP2PSessionState(steamId, out P2PSessionState_t pConnectionState);
                string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + steamId.ToString() + "' AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()))) OR `ip` = '" + ip + "';";
                string[] str = ip.Split('.');
                byte.TryParse(str[0], out byte num1);
                byte.TryParse(str[1], out byte num2);
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null || GlobalBan.Instance.IsPrivateIP(num1, num2))
                    return true;
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }

        public bool IsBanned(string hwid)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `hwid` = '" + hwid + "';";
                connection.Open();
                object result = command.ExecuteScalar();
                //Console.WriteLine();
                //Console.WriteLine($"result null?: {result == null}");
                if (result != null) 
                    return true;
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }

        public class Ban{
            public int Duration;
            public DateTime Time;
            public string Admin;
        }


        public Ban GetBan(string steamId)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select  `banDuration`,`banTime`,`admin` from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = '" + steamId + "' and (banDuration is null or ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                if (result != null && result.Read() && result.HasRows) return new Ban() {
                    Duration = result["banDuration"] == DBNull.Value ? -1 : result.GetInt32("banDuration"),
                    Time = (DateTime)result["banTime"],
                    Admin = (result["admin"] == DBNull.Value || result["admin"].ToString() == "Rocket.API.ConsolePlayer") ? "Console" : (string)result["admin"]
                };
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }

        public void CheckSchema()
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();

                if (test == null)
                {
                    command.CommandText = "CREATE TABLE `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`ip` varchar(15) DEFAULT NULL,`hwid` varchar(256) DEFAULT NULL,`admin` varchar(32) NOT NULL,`banMessage` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void BanPlayer(string characterName, string steamid, string ip, string hwid, string admin, string banMessage, uint duration)
        {
            try
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                if (banMessage == null) banMessage = "";
                command.Parameters.AddWithValue("@csteamid", steamid);
                command.Parameters.AddWithValue("@ip", ip);
                command.Parameters.AddWithValue("@hwid", hwid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@banMessage", banMessage);
                if (duration == 0U)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@banDuration);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public class UnbanResult {
            public ulong Id;
            public string Name;
        }

        public UnbanResult UnbanPlayer(string player)
        {
            try
            {
                MySqlConnection connection = CreateConnection();

                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select steamId,charactername from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player or `charactername` like @player limit 1;";
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    ulong steamId = reader.GetUInt64(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                    return new UnbanResult() { Id = steamId, Name = charactername };
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }
    }
}
