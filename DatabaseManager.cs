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
        public DatabaseManager()
        {
            _ = new I18N.West.CP1250();

            CheckSchema(GlobalBan.Instance.Configuration.Instance.DatabaseBanlist, "CREATE TABLE IF NOT EXISTS `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` (`id` INT NOT NULL AUTO_INCREMENT,`charactername` VARCHAR(256) NOT NULL,`steamid` VARCHAR(64) NOT NULL,`hwid` VARCHAR(256) NOT NULL,`ip` VARCHAR(32) NOT NULL,`admin` VARCHAR(128) NOT NULL DEFAULT 'Console',`reason` VARCHAR(512) NULL,`duration` INT NULL,`bantime` TIMESTAMP NULL,PRIMARY KEY (`id`));");
            //CheckSchema(GlobalBan.Instance.Configuration.Instance.LocalDatabaseTableName, "CREATE TABLE `" + GlobalBan.Instance.Configuration.Instance.LocalDatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamid` varchar(32) NOT NULL,`ip` varchar(15) DEFAULT NULL,`hwid` varchar(256) DEFAULT NULL,`admin` varchar(32) NOT NULL,`reason` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`duration` int NULL,`bantime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));");
            CheckSchema(GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist, "CREATE TABLE `" + GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist + "` (`id` INT NOT NULL AUTO_INCREMENT,`ip` varchar(64) NOT NULL, PRIMARY KEY (`id`));");
        }

        internal MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
                if (GlobalBan.Instance.Configuration.Instance.DatabaseAddress == "localhost")
                    GlobalBan.Instance.Configuration.Instance.DatabaseAddress = "127.0.0.1";
                if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
                    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", GlobalBan.Instance.Configuration.Instance.DatabaseAddress, GlobalBan.Instance.Configuration.Instance.DatabaseName, GlobalBan.Instance.Configuration.Instance.DatabaseUsername, GlobalBan.Instance.Configuration.Instance.DatabasePassword, GlobalBan.Instance.Configuration.Instance.DatabasePort == 0 ? 3306 : GlobalBan.Instance.Configuration.Instance.DatabasePort));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return connection;
        }

        public bool IsWhite(string ip)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist + "` WHERE (`ip` = '" + ip + "');";
                connection.Open();
                return command.ExecuteScalar() != null;
            }
        }

        public bool IsBanned(UnturnedPlayer player, out DateTime date, out string reason)
        {
            try
            {
                SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
                string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                string hwid = GlobalBan.Instance.GetHWidString(player.Player.channel.owner.playerID.hwid);
                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "select `duration`,`bantime`,`reason` FROM `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE (`steamid` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.Trim().ToLower() + "') AND (duration = 0 OR ((duration + UNIX_TIMESTAMP(bantime)) > UNIX_TIMESTAMP()));";
                    connection.Open();
                    using (MySqlDataReader res = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (res != null && res.HasRows)
                        {
                            res.Read();
                            date = (int)res["duration"] == 0 ? DateTime.MaxValue : ((DateTime)res["bantime"]).AddSeconds(res.GetInt32("duration"));
                            reason = (string)res["reason"];
                            return true;//banned
                        }
                    }
                    //connection.Close();
                }

                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE (`steamid` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.ToLower() + "');";
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        if (reader == null || !reader.HasRows)
                            InsertInToTable(player.CharacterName.Trim().ToLower(), player.CSteamID.ToString(), ip, hwid, "", "");
                        else
                        {
                            reader.Read();
                            if (!((string)reader["charactername"]).Equals(player.CharacterName))
                                UpdateRow((int)reader["id"], "charactername", player.CharacterName);
                            if (!((string)reader["steamid"]).Equals(player.CSteamID.ToString()))
                                UpdateRow((int)reader["id"], "steamid", player.CSteamID.ToString());
                            if (!((string)reader["ip"]).Equals(ip))
                                UpdateRow((int)reader["id"], "ip", ip);
                            if (!((string)reader["hwid"]).Equals(hwid))
                                UpdateRow((int)reader["id"], "hwid", hwid);
                            //if ((int)reader["duration"] != 0)
                            //    UpdateRow((int)reader["id"], "duration", 0, global);
                            //if (reader["bantime"] != DBNull.Value)
                            //    UpdateRow((int)reader["id"], "bantime", DBNull.Value, global);
                            //Console.WriteLine($"ban dur: " + reader["duration"]);
                            //Console.WriteLine(reader["bantime"] == DBNull.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.TargetSite.ToString());
                Logger.LogException(ex, ex.Message);
            }
            date = DateTime.Now;
            reason = string.Empty;
            return false;
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
            try
            {
                using (MySqlConnection connection = CreateConnection())
                {
                    player = Regex.Replace(player, @"\p{Cs}", "");
                    MySqlCommand command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@player", "%" + player + "%");
                    //command.CommandText = "select steamid,charactername from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamid` like @player OR `charactername` like @player;";

                    command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE `steamid` like @player OR `charactername` like @player OR `ip` like @player OR `hwid` like @player;";
                    connection.Open();
                    MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    if (result != null && result.HasRows)
                    {
                        result.Read();
                        Ban ban = new Ban
                        {
                            steamid = (string)result["steamid"],
                            ip = (string)result["ip"],
                            Player = (string)result["charactername"],
                            Reason = (string)result["reason"],
                            Duration = (int)result["duration"] == 0 ? DateTime.MaxValue : (int)result["duration"] == -1 ? DateTime.MinValue : ((DateTime)result["bantime"]).AddSeconds(result.GetInt32("duration")),
                            BanDate = result["bantime"] == DBNull.Value ? DateTime.MinValue : (DateTime)result["bantime"],
                            Admin = (result["admin"] == DBNull.Value || result["admin"].ToString() == "Console") ? "Console" : (string)result["admin"]
                        };
                        //Console.WriteLine("GetBan method");
                        connection.Close();
                        connection.Dispose();
                        return ban;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return null;
        }

        public bool UpdateRow(int id, string column, object value)
        {
            object result = null;
            try
            {
                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@value", value);
                    command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` SET `" + column + "` = @value WHERE `id` = '" + id + "';";
                    connection.Open();
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return result == null;
        }

        public void CheckSchema(string table, string query)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + table + "'";
                connection.Open();
                object test = command.ExecuteScalar();
                if (test == null)
                {
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void BanPlayer(string characterName, string steamid, string admin, string reason, uint duration)
        {
            try
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@duration", duration);
                command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` SET `duration` = @duration,`bantime` = now(),`admin` = '" + admin + "',`reason` = '" + reason + "' WHERE `charactername` = '" + characterName + "' AND `steamid` = '" + steamid + "';";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public bool WhiteList(string ip)
        {
            if (IsWhite(ip))
                return false;
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@ip", ip);
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseWhitelist + "` (`ip`) values(@ip);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }

            return true;
        }

        private void InsertInToTable(string characterName, string steamid, string ip, string hwid, string admin, string reason)
        {
            try
            {
                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.Parameters.AddWithValue("@steamid", steamid);
                command.Parameters.AddWithValue("@ip", ip);
                command.Parameters.AddWithValue("@hwid", hwid);
                command.Parameters.AddWithValue("@admin", admin);
                command.Parameters.AddWithValue("@charactername", characterName);
                command.Parameters.AddWithValue("@reason", reason);
                command.Parameters.AddWithValue("@duration", -1);
                command.Parameters.AddWithValue("@bantime", DBNull.Value);
                //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamid`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`bantime`,`duration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@duration);";
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` (`steamid`,`ip`,`hwid`,`admin`,`reason`,`charactername`,`bantime`,`duration`) values(@steamid,@ip,@hwid,@admin,@reason,@charactername,@bantime,@duration);";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public class UnbanResult
        {
            public string steamid;
            public string Player;
        }

        public UnbanResult UnbanPlayer(string player, bool global)
        {
            try
            {
                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@player", "%" + player + "%");
                    command.Parameters.AddWithValue("@bantime", DBNull.Value);
                    //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamid`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`bantime`,`duration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@duration);";
                    command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` SET `duration` = '" + -1 + "',`bantime` = @bantime,`admin` = '" + "" + "',`reason` = '" + "" + "' WHERE `charactername` like @player OR `steamid` like @player;";
                    connection.Open();
                    command.ExecuteNonQuery();
                    command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseBanlist + "` WHERE `charactername` like @player OR `steamid` like @player;";
                    MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    if (result != null && result.HasRows)
                    {
                        result.Read();
                        //connection.Close();
                        //Console.WriteLine("GetBan method");

                        Console.WriteLine();
                        UnbanResult ban = new UnbanResult
                        {
                            steamid = (string)result["steamid"],
                            Player = (string)result["charactername"],
                        };
                        connection.Close();
                        connection.Dispose();
                        return ban;
                    }
                    connection.Close();
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
