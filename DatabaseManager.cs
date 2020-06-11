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

            CheckSchema("CREATE TABLE IF NOT EXISTS `" + GlobalBan.Instance.Configuration.Instance.GlobalBanlist + "` (`id` INT NOT NULL AUTO_INCREMENT,`charactername` VARCHAR(256) NOT NULL,`steamid` VARCHAR(64) NOT NULL,`hwid` VARCHAR(256) NOT NULL,`ip` VARCHAR(32) NOT NULL,`admin` VARCHAR(128) NOT NULL DEFAULT 'Console',`reason` VARCHAR(512) NULL,`duration` INT NULL,`bantime` TIMESTAMP NULL,PRIMARY KEY (`id`));");
            //CheckSchema(GlobalBan.Instance.Configuration.Instance.LocalDatabaseTableName, "CREATE TABLE `" + GlobalBan.Instance.Configuration.Instance.LocalDatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamid` varchar(32) NOT NULL,`ip` varchar(15) DEFAULT NULL,`hwid` varchar(256) DEFAULT NULL,`admin` varchar(32) NOT NULL,`reason` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`duration` int NULL,`bantime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));");
            CheckSchema("CREATE TABLE IF NOT EXISTS `" + GlobalBan.Instance.Configuration.Instance.Whitelist + "` (`id` INT NOT NULL AUTO_INCREMENT,`ip` varchar(64) NOT NULL, PRIMARY KEY (`id`));");
        }

        private MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (GlobalBan.Instance.Configuration.Instance.DatabaseAddress == "localhost")
                    GlobalBan.Instance.Configuration.Instance.DatabaseAddress = "127.0.0.1";
                if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
                    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", GlobalBan.Instance.Configuration.Instance.DatabaseAddress, GlobalBan.Instance.Configuration.Instance.DatabaseName, GlobalBan.Instance.Configuration.Instance.DatabaseUsername, GlobalBan.Instance.Configuration.Instance.DatabasePassword, GlobalBan.Instance.Configuration.Instance.DatabasePort == 0 ? 3306 : GlobalBan.Instance.Configuration.Instance.DatabasePort));
                connection.Open();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public bool IsWhite(string steamid)
        {
            bool flag = false;
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.Whitelist + "` WHERE (`steamid` = '" + steamid + "');";
                flag = command.ExecuteScalar() != null;
                if (flag && GlobalBan.Instance.Configuration.Instance.ShowConnectInfo)
                {
                    ConsoleColor def = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"\n| Connected whitelisted player: \n| SteamID: {steamid}\n");
                    Console.ForegroundColor = def;
                }
                CloseConnection(connection);
            }

            return flag;
        }

        public bool IsBanned(UnturnedPlayer player, out DateTime date, out string reason, out bool global)
        {
            date = DateTime.Now;
            reason = string.Empty;
            global = false;
            bool flag = false;

            SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
            string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
            string name = player.CharacterName.Trim().ToLower();
            //string hwid = GlobalBan.Instance.GetHWidString(player.Player.channel.owner.playerID.hwid);
            string steamid = player.CSteamID.ToString();
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                //looking for if player banned
                command.Parameters.AddWithValue("@global", GlobalBan.Instance.Configuration.Instance.GlobalBanlist);
                command.Parameters.AddWithValue("@local", GlobalBan.Instance.Configuration.Instance.LocalBanlist);
                command.Parameters.AddWithValue("@steamid", steamid);
                command.Parameters.AddWithValue("@ip", ip);
                command.Parameters.AddWithValue("@name", name);
                command.CommandText = "select * FROM `@global`,`@local` WHERE (`steamid` = @steamid OR `ip` = @ip OR `charactername` = @name) AND (duration = 0 OR (duration != -1 AND (duration + UNIX_TIMESTAMP(bantime)) > UNIX_TIMESTAMP()));";

                using (MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        date = (int)reader["duration"] == 0 ? DateTime.MaxValue : ((DateTime)reader["bantime"]).AddSeconds(reader.GetInt32("duration"));
                        reason = (string)reader["reason"];
                        global = reader.GetSchemaTable().TableName.ToLower().Trim().Equals(GlobalBan.Instance.Configuration.Instance.GlobalBanlist.ToLower().Trim());
                        reader.Close();
                        reader.Dispose();
                        flag = true;//banned
                    }
                    CloseReader(reader);
                }
                CloseConnection(connection);
            }

            if (flag)
                return true;

            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@global", GlobalBan.Instance.Configuration.Instance.GlobalBanlist);
                command.Parameters.AddWithValue("@local", GlobalBan.Instance.Configuration.Instance.LocalBanlist);
                command.Parameters.AddWithValue("@steamid", steamid);
                command.Parameters.AddWithValue("@ip", ip);
                command.Parameters.AddWithValue("@name", name);
                command.CommandText = "select * from `@global` WHERE (`steamid` = @steamid OR `ip` = @ip OR `charactername` = @name);";
                MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                if (reader == null || !reader.HasRows)//?legacy
                    InsertInToTable(GlobalBan.Instance.Configuration.Instance.GlobalBanlist, name, player.CSteamID.ToString(), ip, "", "");
                else
                {
                    int id = (int)reader["id"];
                    if (!((string)reader["charactername"]).Equals(name))
                        UpdateRow(id, "charactername", name, true);
                    if (!((string)reader["steamid"]).Equals(steamid))
                        UpdateRow(id, "steamid", steamid, true);
                    if (!((string)reader["ip"]).Equals(ip))
                        UpdateRow(id, "ip", ip, true);
                    //if (!((string)reader["hwid"]).Equals(hwid))
                    //    UpdateRow((int)reader["id"], "hwid", hwid);
                }
                CloseReader(reader);

                command.CommandText = "select * from `@local` WHERE (`steamid` = @steamid OR `ip` = @ip OR `charactername` = @name);";
                reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                if (reader == null || !reader.HasRows)//?legacy
                    InsertInToTable(GlobalBan.Instance.Configuration.Instance.LocalBanlist, name, player.CSteamID.ToString(), ip, "", "");
                else
                {
                    int id = (int)reader["id"];
                    if (!((string)reader["charactername"]).Equals(name))
                        UpdateRow(id, "charactername", name);
                    if (!((string)reader["steamid"]).Equals(steamid))
                        UpdateRow(id, "steamid", steamid);
                    if (!((string)reader["ip"]).Equals(ip))
                        UpdateRow(id, "ip", ip);
                    //if (!((string)reader["hwid"]).Equals(hwid))
                    //    UpdateRow((int)reader["id"], "hwid", hwid);
                }
                CloseConnection(connection);
            }

            return false;
        }

        public class PlayerInfo
        {
            public string Reason;
            public string Admin;
            public string Charactername;
            public string steamid;
            public DateTime BanDate;
            public DateTime Duration;
            public string ip;
        }


        public PlayerInfo GetBan(string player, bool global)
        {
            PlayerInfo ban = null;
            using (MySqlConnection connection = CreateConnection())
            {
                //player = Regex.Replace(player, @"\p{Cs}", "");
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.Parameters.AddWithValue("@table", global ? GlobalBan.Instance.Configuration.Instance.GlobalBanlist : GlobalBan.Instance.Configuration.Instance.LocalBanlist);
                command.CommandText = "select * from `@table` WHERE `steamid` like @player OR `charactername` like @player OR `ip` like @player;";

                using (MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                {
                    if (reader != null && reader.HasRows)
                    {
                        reader.Read();
                        ban = new PlayerInfo
                        {
                            steamid = (string)reader["steamid"],
                            ip = (string)reader["ip"],
                            Charactername = (string)reader["charactername"],
                            Reason = (string)reader["reason"],
                            Duration = (int)reader["duration"] == 0 ? DateTime.MaxValue : (int)reader["duration"] == -1 ? DateTime.MinValue : ((DateTime)reader["bantime"]).AddSeconds(reader.GetInt32("duration")),
                            BanDate = reader["bantime"] == DBNull.Value ? DateTime.MinValue : (DateTime)reader["bantime"],
                            Admin = (reader["admin"] == DBNull.Value || reader["admin"].ToString() == "Console") ? "Console" : (string)reader["admin"]
                        };
                    }
                    CloseReader(reader);
                }
                CloseConnection(connection);
            }

            return ban;
        }

        public void UpdateRow(int id, string column, object value, bool global = false)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@table", global ? GlobalBan.Instance.Configuration.Instance.GlobalBanlist : GlobalBan.Instance.Configuration.Instance.LocalBanlist);
                command.Parameters.AddWithValue("@value", value);
                command.Parameters.AddWithValue("@column", column);
                command.Parameters.AddWithValue("@id", id);
                command.CommandText = "UPDATE `@table` SET `@column` = @value WHERE `id` = @id;";
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }

        public void CheckSchema(string query)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }

        public void BanPlayer(string ip, string steamid, string admin, string reason, uint duration, bool global)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                //characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@table", global ? GlobalBan.Instance.Configuration.Instance.GlobalBanlist : GlobalBan.Instance.Configuration.Instance.LocalBanlist);
                command.Parameters.AddWithValue("@duration", duration);
                command.Parameters.AddWithValue("@ip", ip);
                command.Parameters.AddWithValue("@steamid", steamid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@reason", reason);
                command.CommandText = "UPDATE `@table` SET `duration` = @duration,`bantime` = now(),`admin` = @admin,`reason` = @reason WHERE `ip` = @ip AND `steamid` = @steamid;";
                
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }

        public bool WhiteList(string steamid)
        {
            if (IsWhite(steamid))
                return false;
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@steamid", steamid);
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.Whitelist + "` (`steamid`) values(@steamid);";
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }

            return true;
        }

        private void InsertInToTable(string table, string characterName, string steamid, string ip, string admin, string reason)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@steamid", steamid);
                command.Parameters.AddWithValue("@ip", ip);
                //command.Parameters.AddWithValue("@hwid", hwid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@duration", -1);
                command.Parameters.AddWithValue("@bantime", DBNull.Value);
                command.Parameters.AddWithValue("@table", table);
                command.CommandText = "insert into `@table` (`steamid`,`ip`,`admin`,`reason`,`charactername`,`bantime`,`duration`) values(@steamid,@ip,@admin,@reason,@charactername,@bantime,@duration);";
                
                command.ExecuteNonQuery();
                CloseConnection(connection);
            }
        }

        public class UnbanResult
        {
            public string steamid;
            public string Player;
        }

        public UnbanResult UnbanPlayer(string player, bool global)
        {
            UnbanResult unban = null;
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@table", global ? GlobalBan.Instance.Configuration.Instance.GlobalBanlist : GlobalBan.Instance.Configuration.Instance.LocalBanlist);
                command.Parameters.AddWithValue("@player", "%" + player + "%");
                command.Parameters.AddWithValue("@bantime", DBNull.Value);
                command.CommandText = "UPDATE `@table` SET `duration` = -1,`bantime` = @bantime,`admin` = '',`reason` = '' WHERE `charactername` like @player OR `steamid` like @player;";
                command.ExecuteNonQuery();
                command.CommandText = "select * from `@table` WHERE `charactername` like @player OR `steamid` like @player;";

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
                    CloseReader(reader);
                }
                CloseConnection(connection);
            }

            return unban;
        }

        private void CloseConnection(MySqlConnection connection)
        {
            connection.Close();
            connection.Dispose();
        }

        private void CloseReader(MySqlDataReader reader)
        {
            reader.Close();
            reader.Dispose();
        }
    }
}
