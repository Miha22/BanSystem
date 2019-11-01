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
            CheckSchema();
        }

        internal MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            try
            {
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

        public bool IsBanned(UnturnedPlayer player, out DateTime date)
        {
            try
            {
                //MySqlConnection connection = CreateConnection();
                SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
                string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                string hwid = GlobalBan.Instance.GetHWidString(player.Player.channel.owner.playerID.hwid);
                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "select `banDuration`,`banTime` from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.ToLower() + "') AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                    connection.Open();
                    //Console.WriteLine("point 1");
                    MySqlDataReader res = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    //Console.WriteLine($"res != null: {res != null}, res.HasRows: {res.HasRows}");
                    bool flag = res != null && res.HasRows;
                    //Console.WriteLine($"res != null: {res != null}, res.Read(): {res.Read()}, res.HasRows: {res.HasRows}");
                    //Console.WriteLine($"flag: {flag}");
                    if (flag)
                    {
                        res.Read();
                        //Console.WriteLine("point 1.5");
                        //date = ((DateTime)res["banTime"]).AddSeconds(res.GetInt32("banDuration")).AddHours(-GlobalBan.Instance.UTCoffset);
                        date = res["banDuration"] == DBNull.Value ? DateTime.MaxValue : ((DateTime)res["banTime"]).AddSeconds(res.GetInt32("banDuration"));
                        connection.Close();
                        connection.Dispose();
                        return true;
                    }
                    connection.Close();
                }

                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.ToLower() + "');";
                    connection.Open();
                    object result = command.ExecuteScalar();
                    if(result == null)
                        InsertInToTable(player.CharacterName.ToLower(), player.CSteamID.ToString(), ip, hwid, "", "", 0, DateTime.Now);
                    //Console.WriteLine("point 2");
                    //MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    //if (result != null && result.Read() && result.HasRows)
                    //{
                    //    int id = (int)result["id"];
                    //    connection.Close();
                    //    using (MySqlConnection connection2 = CreateConnection())
                    //    {
                    //        MySqlCommand command2 = connection2.CreateCommand();
                    //        command2.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `id` = '" + id + "';";
                    //        connection2.Open();
                    //        command2.ExecuteNonQuery();
                    //        //Console.WriteLine("point 3");
                    //        connection2.Close();
                    //    }
                    //}
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.TargetSite.ToString());
                Logger.LogException(ex, ex.Message);
            }
            date = DateTime.Now;
            return false;
        }
        //UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` SET `charactername`= '" + player.CharacterName.ToLower() + "' WHERE  `id`=1;
        //public bool IsBanned(CSteamID steamID)
        //{
        //    try
        //    {
        //        MySqlConnection connection = CreateConnection();
        //        MySqlCommand command = connection.CreateCommand();
        //        SteamGameServerNetworking.GetP2PSessionState(steamID, out P2PSessionState_t pConnectionState);
        //        string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
        //        command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + steamID.ToString() + "' OR `ip` = '" + ip + "') AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        if (result != null)
        //        {
        //            connection.Close();
        //            return true;
        //        }
        //        connection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }
        //    return false;
        //}

        
        //public bool IsBanned(string hwid)
        //{
        //    try
        //    {
        //        MySqlConnection connection = CreateConnection();
        //        MySqlCommand command = connection.CreateCommand();
        //        command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `hwid` = '" + hwid + "';";
        //        connection.Open();
        //        object result = command.ExecuteScalar();
        //        //Console.WriteLine();
        //        //Console.WriteLine($"result null?: {result == null}");
        //        if (result != null) 
        //            return true;
        //        connection.Close();
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }
        //    return false;
        //}

        public class Ban
        {
            public string Reason;
            public string Admin;
            public string Player;
            public DateTime BanDate;
            public DateTime Duration;
        }


        public Ban GetBan(string player)
        {
            try
            {
                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@player", "%" + player + "%");
                    //command.CommandText = "select steamId,charactername from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player OR `charactername` like @player;";

                    command.CommandText = "select  `banDuration`,`banTime`,`admin`,`reason`,`charactername` from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` like @player OR `charactername` like @player;";
                    connection.Open();
                    MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                    if (result != null && result.HasRows)
                    {
                        result.Read();
                        //connection.Close();
                        connection.Close();
                        connection.Dispose();
                        return new Ban
                        {
                            Player = (string)result["charactername"],
                            Reason = (string)result["reason"],
                            BanDate = (DateTime)result["banTime"],
                            Duration = result["banDuration"] == DBNull.Value ? DateTime.MaxValue : ((DateTime)result["banTime"]).AddSeconds(result.GetInt32("banDuration")),
                            Admin = (result["admin"] == DBNull.Value || result["admin"].ToString() == "Rocket.API.ConsolePlayer") ? "Console" : (string)result["admin"]
                        };
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

        public void CheckSchema()
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "show tables like '" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "'";
                connection.Open();
                object test = command.ExecuteScalar();
                //SET @@session.time_zone='+00:00';
                if (test == null)
                {
                    //command.CommandText = "SET @@session.time_zone ='+00:00';";
                    //command.ExecuteNonQuery();
                    command.CommandText = "CREATE TABLE `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`ip` varchar(15) DEFAULT NULL,`hwid` varchar(256) DEFAULT NULL,`admin` varchar(32) NOT NULL,`reason` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
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
                command.Parameters.AddWithValue("@steamid", "%" + steamid + "%");
                command.Parameters.AddWithValue("@characterName", "%" + characterName + "%");
                //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@banDuration);";
                command.CommandText = "UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` SET `banDuration` = `" + duration + "`,`banTime` = `" + DateTime.Now + "`,`admin` = `" + admin + "`,`reason` = `" + reason + "` WHERE `steamId` like @steamid OR `charactername` like @charactername;";
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void InsertInToTable(string characterName, string steamid, string ip, string hwid, string admin, string reason, uint duration, DateTime banTime)
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
                command.Parameters.AddWithValue("@banTime", banTime);
                if (duration == 0U)
                {
                    command.Parameters.AddWithValue("@banDuration", DBNull.Value);
                }
                else
                {
                    command.Parameters.AddWithValue("@banDuration", duration);
                }
                //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@banDuration);";
                command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`reason`,`charactername`,`banTime`,`banDuration`) values(@steamid,@ip,@hwid,@admin,@reason,@charactername,now(),@banDuration);";
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
            public string Id;
            public string Name;
        }

        //public UnbanResult GetBan(string player)
        //{
        //    try
        //    {
        //        MySqlConnection connection = CreateConnection();

        //        MySqlCommand command = connection.CreateCommand();
        //        command.Parameters.AddWithValue("@player", "%" + player + "%");
        //        command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player OR `charactername` like @player;";
        //        connection.Open();
        //        MySqlDataReader reader = command.ExecuteReader();
        //        if (reader.Read())
        //        {
        //            //ulong steamId = reader.GetUInt64(0);
        //            string steamId = reader.GetString(0);
        //            string charactername = reader.GetString(1);
        //            connection.Close();
        //            command = connection.CreateCommand();
        //            command.Parameters.AddWithValue("@steamId", steamId);
        //            command.Parameters.AddWithValue("@charactername", charactername);
        //            command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId OR `charactername` = @charactername;";
        //            connection.Open();
        //            command.ExecuteNonQuery();
        //            connection.Close();
        //            return new UnbanResult() { Id = steamId, Name = charactername };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.LogException(ex);
        //    }
        //    return null;
        //}

        public UnbanResult UnbanPlayer(string player)
        {
            try
            {
                using (MySqlConnection connection = CreateConnection())
                {
                    MySqlCommand command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@player", "%" + player + "%");
                    command.CommandText = "select steamId,charactername from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player OR `charactername` like @player;";
                    connection.Open();
                    MySqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        //ulong steamId = reader.GetUInt64(0);
                        string steamId = reader.GetString(0);
                        string charactername = reader.GetString(1);
                        command = connection.CreateCommand();
                        command.Parameters.AddWithValue("@steamId", steamId);
                        command.Parameters.AddWithValue("@charactername", charactername);
                        command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId OR `charactername` = @charactername;";
                        command.ExecuteNonQuery();
                        connection.Close();
                        connection.Dispose();
                        return new UnbanResult() { Id = steamId, Name = charactername };
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
