using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Text.RegularExpressions;
//using System.Text.RegularExpressions;

namespace BanSystem
{
    //https://rutracker.org/forum/viewtopic.php?t=5542725
    public class DatabaseManager
    {

        static DatabaseManager()
        {
            try
            {
                if (GlobalBan.Instance.Configuration.Instance.DatabaseAddress == "localhost")
                    GlobalBan.Instance.Configuration.Instance.DatabaseAddress = "127.0.0.1";
                if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
                    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public DatabaseManager()
        {
            _ = new I18N.West.CP1250();

            CheckSchema("CREATE TABLE IF NOT EXISTS `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` (`id` INT NOT NULL AUTO_INCREMENT,`charactername` VARCHAR(256) NOT NULL,`steamid` VARCHAR(64) NOT NULL,`hwid` VARCHAR(256) NOT NULL,`ip` VARCHAR(32) NOT NULL,`admin` VARCHAR(128) NOT NULL DEFAULT 'Console',`reason` VARCHAR(512) NULL,`duration` INT NULL,`bantime` TIMESTAMP NULL,PRIMARY KEY (`id`));");
            //CheckSchema(GlobalBan.Instance.Configuration.Instance.LocalDatabaseTableName, "CREATE TABLE `" + GlobalBan.Instance.Configuration.Instance.LocalDatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamid` varchar(32) NOT NULL,`ip` varchar(15) DEFAULT NULL,`hwid` varchar(256) DEFAULT NULL,`admin` varchar(32) NOT NULL,`reason` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`duration` int NULL,`bantime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));");
            CheckSchema("CREATE TABLE IF NOT EXISTS `" + GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist + "` (`id` INT NOT NULL AUTO_INCREMENT,`ip` varchar(64) NOT NULL, PRIMARY KEY (`id`));");
        }

        internal MySqlConnection CreateConnection()
        {
            MySqlConnection Connection = null;
            try
            {
                if (GlobalBan.Instance.Configuration.Instance.DatabaseAddress == "localhost")
                    GlobalBan.Instance.Configuration.Instance.DatabaseAddress = "127.0.0.1";
                if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
                    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
                Connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", GlobalBan.Instance.Configuration.Instance.DatabaseAddress, GlobalBan.Instance.Configuration.Instance.DatabaseName, GlobalBan.Instance.Configuration.Instance.DatabaseUsername, GlobalBan.Instance.Configuration.Instance.DatabasePassword, GlobalBan.Instance.Configuration.Instance.DatabasePort == 0 ? 3306 : GlobalBan.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return Connection;
        }

        public bool IsWhite(string ip)
        {
            bool flag = false;
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist + "` WHERE (`ip` = '" + ip + "');";
                flag = command.ExecuteScalar() != null;
                if (flag && GlobalBan.Instance.Configuration.Instance.ShowConnectInfo)
                {
                    ConsoleColor def = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n| Connected whitelisted player: \n| IP: {ip}\n");
                    Console.ForegroundColor = def;
                }
            }

            return flag;
        }

        public bool IsBanned(UnturnedPlayer player, out DateTime date, out string reason)
        {
            date = DateTime.Now;
            reason = string.Empty;
            bool flag = false;
            using (MySqlConnection connection = CreateConnection())
            {
                SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
                string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                string hwid = GlobalBan.Instance.GetHWidString(player.Player.channel.owner.playerID.hwid);

                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select * FROM `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE (`steamid` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.Trim().ToLower() + "') AND (duration = 0 OR ((duration + UNIX_TIMESTAMP(bantime)) > UNIX_TIMESTAMP()));";

                using (MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        date = (int)reader["duration"] == 0 ? DateTime.MaxValue : ((DateTime)reader["bantime"]).AddSeconds(reader.GetInt32("duration"));
                        reason = (string)reader["reason"];
                        reader.Close(); reader.Dispose();

                        flag = true;//banned
                    }
                    else if (reader == null || !reader.HasRows)//?legacy
                        InsertInToTable(player.CharacterName.Trim().ToLower(), player.CSteamID.ToString(), ip, hwid, "", "");
                    else
                    {
                        if (!((string)reader["charactername"]).Equals(player.CharacterName))
                            UpdateRow((int)reader["id"], "charactername", player.CharacterName);
                        if (!((string)reader["steamid"]).Equals(player.CSteamID.ToString()))
                            UpdateRow((int)reader["id"], "steamid", player.CSteamID.ToString());
                        if (!((string)reader["ip"]).Equals(ip))
                            UpdateRow((int)reader["id"], "ip", ip);
                        if (!((string)reader["hwid"]).Equals(hwid))
                            UpdateRow((int)reader["id"], "hwid", hwid);
                    }
                }
                
            }

            return flag;
        }

        public class Ban
        {
            public string Reason;
            public string Admin;
            public string Player;
            public string steamid;
            public DateTime BanDate;
            public DateTime Duration;
            public string ip;
        }


        public Ban GetBan(string player)
        {
            Ban ban = null;
            using (MySqlConnection connection = CreateConnection())
            {
                player = Regex.Replace(player, @"\p{Cs}", "");
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE `steamid` like @player OR `charactername` like @player OR `ip` like @player OR `hwid` like @player;";

                using (MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        ban = new Ban
                        {
                            steamid = (string)reader["steamid"],
                            ip = (string)reader["ip"],
                            Player = (string)reader["charactername"],
                            Reason = (string)reader["reason"],
                            Duration = (int)reader["duration"] == 0 ? DateTime.MaxValue : (int)reader["duration"] == -1 ? DateTime.MinValue : ((DateTime)reader["bantime"]).AddSeconds(reader.GetInt32("duration")),
                            BanDate = reader["bantime"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["bantime"],
                            Admin = (reader["admin"] == DBNull.Value || reader["admin"].ToString() == "Console") ? "Console" : (string)reader["admin"]
                        };
                    }
                }
            }

            return ban;
        }

        public void UpdateRow(int id, string column, object value)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@value", value);
                command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` SET `" + column + "` = @value WHERE `id` = '" + id + "';";
                command.ExecuteNonQuery();
            }
        }

        public void CheckSchema(string query)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
            }
        }

        public void BanPlayer(string characterName, string steamid, string admin, string reason, uint duration)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@duration", duration);
                command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` SET `duration` = @duration,`bantime` = now(),`admin` = '" + admin + "',`reason` = '" + reason + "' WHERE `charactername` = '" + characterName + "' AND `steamid` = '" + steamid + "';";
                
                command.ExecuteNonQuery();
            }
        }

        public bool WhiteList(string ip)
        {
            if (IsWhite(ip))
                return false;
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@ip", ip);
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist + "` (`ip`) values(@ip);";
                command.ExecuteNonQuery();   
            }

            return true;
        }

        private void InsertInToTable(string characterName, string steamid, string ip, string hwid, string admin, string reason)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@steamid", steamid);
                command.Parameters.AddWithValue("@ip", ip);
                command.Parameters.AddWithValue("@hwid", hwid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@duration", -1);
                command.Parameters.AddWithValue("@bantime", DBNull.Value);
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` (`steamid`,`ip`,`hwid`,`admin`,`reason`,`charactername`,`bantime`,`duration`) values(@steamid,@ip,@hwid,@admin,@reason,@charactername,@bantime,@duration);";
                
                command.ExecuteNonQuery();
            }
        }

        public class UnbanResult
        {
            public string steamid;
            public string Player;
        }

        public UnbanResult UnbanPlayer(string player)
        {
            UnbanResult unban = null;
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.Parameters.AddWithValue("@bantime", DBNull.Value);
                command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` SET `duration` = '" + -1 + "',`bantime` = @bantime,`admin` = '" + "" + "',`reason` = '" + "" + "' WHERE `charactername` like @player OR `steamid` like @player;";
                
                command.ExecuteNonQuery();
                command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE `charactername` like @player OR `steamid` like @player;";

                using (MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        unban = new UnbanResult
                        {
                            steamid = (string)reader["steamid"],
                            Player = (string)reader["charactername"],
                        };
                    }
                }
            }

            return unban;
        }
    }
}
