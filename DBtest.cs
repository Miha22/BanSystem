﻿//using MySql.Data.MySqlClient;
//using System;
//using System.Text.RegularExpressions;
////using System.Text.RegularExpressions;

//namespace practise
//{
//    //https://rutracker.org/forum/viewtopic.php?t=5542725
//    public class DatabaseManager
//    {
//        public DatabaseManager()
//        {
//            _ = new I18N.West.CP1250();
//            CheckSchema();
//        }

//        internal MySqlConnection CreateConnection()
//        {
//            MySqlConnection connection = null;
//            try
//            {
//                //if (GlobalBan.Instance.Configuration.Instance.DatabasePort == 0)
//                //    GlobalBan.Instance.Configuration.Instance.DatabasePort = 3306;
//                connection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", "127.0.0.1", "unturned", "root", "123", "3306"));
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            return connection;
//        }

//        //public bool IsBanned(UnturnedPlayer player, out DateTime date)
//        //{
//        //    try
//        //    {
//        //        //MySqlConnection connection = CreateConnection();
//        //        SteamGameServerNetworking.GetP2PSessionState(player.CSteamID, out P2PSessionState_t pConnectionState);
//        //        string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
//        //        string hwid = "123.123.123.123.123.123";
//        //        using (MySqlConnection connection = CreateConnection())
//        //        {
//        //            MySqlCommand command = connection.CreateCommand();
//        //            command.CommandText = "select `banDuration`,`banTime` from `bansystem.banlist` WHERE (`steamId` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.ToLower() + "') AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
//        //            connection.Open();
//        //            //Console.WriteLine("point 1");
//        //            MySqlDataReader res = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
//        //            //Console.WriteLine($"res != null: {res != null}, res.HasRows: {res.HasRows}");
//        //            bool flag = res != null && res.HasRows;
//        //            //Console.WriteLine($"res != null: {res != null}, res.Read(): {res.Read()}, res.HasRows: {res.HasRows}");
//        //            //Console.WriteLine($"flag: {flag}");
//        //            if (flag)
//        //            {
//        //                res.Read();
//        //                //Console.WriteLine("point 1.5");
//        //                //date = ((DateTime)res["banTime"]).AddSeconds(res.GetInt32("banDuration")).AddHours(-GlobalBan.Instance.UTCoffset);
//        //                date = res["banDuration"] == DBNull.Value ? DateTime.MaxValue : ((DateTime)res["banTime"]).AddSeconds(res.GetInt32("banDuration"));
//        //                connection.Close();
//        //                connection.Dispose();
//        //                return true;
//        //            }
//        //            connection.Close();
//        //        }

//        //        using (MySqlConnection connection = CreateConnection())
//        //        {
//        //            MySqlCommand command = connection.CreateCommand();
//        //            command.CommandText = "select `id`,`steamId`,`ip`,`hwid`,`charactername`,`banDuration`,`banTime` from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `ip` = '" + ip + "' OR `charactername` = '" + player.CharacterName.ToLower() + "');";
//        //            connection.Open();
//        //            MySqlDataReader reader = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
//        //            if (reader == null || !reader.HasRows)
//        //                InsertInToTable(player.CharacterName.ToLower(), player.CSteamID.ToString(), ip, hwid, "", "");
//        //            else
//        //            {
//        //                reader.Read();
//        //                if ((string)reader["charactername"] != player.CharacterName.ToLower())
//        //                    UpdateRow((int)reader["id"], "charactername", player.CharacterName.ToLower());
//        //                if ((string)reader["steamId"] != player.CSteamID.ToString())
//        //                    UpdateRow((int)reader["id"], "steamId", player.CSteamID.ToString());
//        //                if ((string)reader["ip"] != ip)
//        //                    UpdateRow((int)reader["id"], "ip", ip);
//        //                if ((string)reader["hwid"] != hwid)
//        //                    UpdateRow((int)reader["id"], "hwid", hwid);
//        //                if (reader["banDuration"] != (object)0)
//        //                    UpdateRow((int)reader["id"], "banDuration", 0);
//        //                if (reader["banTime"] != DBNull.Value)
//        //                    UpdateRow((int)reader["id"], "banTime", DBNull.Value);
//        //                //Console.WriteLine($"ban dur: " + reader["banDuration"]);
//        //                //Console.WriteLine(reader["banTime"] == DBNull.Value);
//        //            }
//        //            reader.Close();
//        //            reader.Dispose();
//        //            connection.Close();
//        //        }
//        //    }
//        //catch (Exception ex)
//        //{
//        //    Console.WriteLine(ex.Message);
//        //}
//        //date = DateTime.Now;
//        //return false;
//        //}
//        //UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` SET `charactername`= '" + player.CharacterName.ToLower() + "' WHERE  `id`=1;
//        //public bool IsBanned(CSteamID steamID)
//        //{
//        //    try
//        //    {
//        //        MySqlConnection connection = CreateConnection();
//        //        MySqlCommand command = connection.CreateCommand();
//        //        SteamGameServerNetworking.GetP2PSessionState(steamID, out P2PSessionState_t pConnectionState);
//        //        string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
//        //        command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + steamID.ToString() + "' OR `ip` = '" + ip + "') AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
//        //        connection.Open();
//        //        object result = command.ExecuteScalar();
//        //        if (result != null)
//        //        {
//        //            connection.Close();
//        //            return true;
//        //        }
//        //        connection.Close();
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Logger.LogException(ex);
//        //    }
//        //    return false;
//        //}


//        //public bool IsBanned(string hwid)
//        //{
//        //    try
//        //    {
//        //        MySqlConnection connection = CreateConnection();
//        //        MySqlCommand command = connection.CreateCommand();
//        //        command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `hwid` = '" + hwid + "';";
//        //        connection.Open();
//        //        object result = command.ExecuteScalar();
//        //        //Console.WriteLine();
//        //        //Console.WriteLine($"result null?: {result == null}");
//        //        if (result != null) 
//        //            return true;
//        //        connection.Close();
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Logger.LogException(ex);
//        //    }
//        //    return false;
//        //}

//        public class Ban
//        {
//            public string Reason;
//            public string Admin;
//            public string Player;
//            public string SteamID;
//            public DateTime BanDate;
//            public DateTime Duration;
//        }


//        public Ban GetBan(string player)
//        {
//            try
//            {
//                using (MySqlConnection connection = CreateConnection())
//                {
//                    player = Regex.Replace(player, @"\p{Cs}", "");
//                    MySqlCommand command = connection.CreateCommand();
//                    command.Parameters.AddWithValue("@player", "%" + player + "%");
//                    //command.CommandText = "select steamId,charactername from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player OR `charactername` like @player;";

//                    command.CommandText = "select  `steamId`,`banDuration`,`banTime`,`admin`,`reason`,`charactername` from `bansystem.banlist` WHERE `steamId` like @player OR `charactername` like @player;";
//                    connection.Open();
//                    MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
//                    if (result != null && result.HasRows)
//                    {
//                        result.Read();
//                        //connection.Close();
//                        Console.WriteLine("GetBan method");
//                        Console.WriteLine(result["banTime"] == DBNull.Value);
//                        Console.WriteLine((int)result["banDuration"] == 0);
//                        Ban ban = new Ban
//                        {
//                            SteamID = (string)result["steamId"],
//                            Player = (string)result["charactername"],
//                            Reason = (string)result["reason"],
//                            BanDate = result["banTime"] == DBNull.Value ? DateTime.MinValue : (DateTime)result["banTime"],
//                            Duration = result["banDuration"] == DBNull.Value ? DateTime.MaxValue : (int)result["banDuration"] == 0 ? DateTime.MinValue : ((DateTime)result["banTime"]).AddSeconds(result.GetInt32("banDuration")),
//                            Admin = (result["admin"] == DBNull.Value || result["admin"].ToString() == "Rocket.API.ConsolePlayer") ? "Console" : (string)result["admin"]
//                        };
//                        Console.WriteLine("GetBan method");
//                        connection.Close();
//                        connection.Dispose();
//                        return ban;
//                    }
//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            return null;
//        }

//        //public bool PlayerExists(string charactername, string steamID, string ip, string hwid)
//        //{
//        //    object result = null;
//        //    try
//        //    {
//        //        using (MySqlConnection connection = CreateConnection())
//        //        {
//        //            MySqlCommand command = connection.CreateCommand();
//        //            //command.CommandText = "select steamId,charactername from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player OR `charactername` like @player;";

//        //            command.CommandText = "select  1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` = '" + steamID + "' AND `charactername` = '" + charactername + "' AND `ip` = '" + ip + "' AND `hwid` = '" + hwid + "';";
//        //            connection.Open();
//        //            result = command.ExecuteScalar();
//        //            connection.Close();
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Logger.LogException(ex);
//        //    }
//        //    return result == null;
//        //}
//        public bool UpdateRow(int id, string column, object value)
//        {
//            object result = null;
//            try
//            {//UPDATE `unturned`.`bansystem.banlist` SET `charactername`='gay' WHERE  `id`=7;
//                using (MySqlConnection connection = CreateConnection())
//                {
//                    MySqlCommand command = connection.CreateCommand();
//                    command.Parameters.AddWithValue("@value", value);
//                    command.CommandText = "UPDATE `bansystem.banlist` SET `" + column + "` = @value WHERE `id` = '" + id + "';";
//                    connection.Open();
//                    command.ExecuteNonQuery();
//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            return result == null;
//        }

//        public void CheckSchema()
//        {
//            try
//            {
//                MySqlConnection connection = CreateConnection();
//                MySqlCommand command = connection.CreateCommand();
//                command.CommandText = "show tables like 'bansystem.banlist'";
//                connection.Open();
//                object test = command.ExecuteScalar();
//                //SET @@session.time_zone='+00:00';
//                if (test == null)
//                {
//                    //command.CommandText = "SET @@session.time_zone ='+00:00';";
//                    //command.ExecuteNonQuery();
//                    command.CommandText = "CREATE TABLE `bansystem.banlist` (`id` int(11) NOT NULL AUTO_INCREMENT,`steamId` varchar(32) NOT NULL,`ip` varchar(15) DEFAULT NULL,`hwid` varchar(256) DEFAULT NULL,`admin` varchar(32) NOT NULL,`reason` varchar(512) DEFAULT NULL,`charactername` varchar(255) DEFAULT NULL,`banDuration` int NULL,`banTime` timestamp NULL ON UPDATE CURRENT_TIMESTAMP,PRIMARY KEY (`id`));";
//                    command.ExecuteNonQuery();
//                }
//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//        }

//        public void BanPlayer(string characterName, string steamid, string admin, string reason, uint duration)
//        {
//            try
//            {
//                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
//                MySqlConnection connection = CreateConnection();
//                MySqlCommand command = connection.CreateCommand();
//                command.Parameters.AddWithValue("@steamid", "%" + steamid + "%");
//                command.Parameters.AddWithValue("@characterName", "%" + characterName + "%");
//                object res = duration == 0U ? (object)DBNull.Value : duration;
//                //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@banDuration);";
//                command.CommandText = "UPDATE `bansystem.banlist` SET `banDuration` = '" + res + "',`banTime` = now(),`admin` = '" + admin + "',`reason` = '" + reason + "' WHERE `charactername` = @charactername OR `steamId` = @steamid ;";
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//        }

//        private void InsertInToTable(string characterName, string steamid, string ip, string hwid, string admin, string reason)
//        {
//            try
//            {
//                characterName = Regex.Replace(characterName, @"\p{Cs}", "");
//                MySqlConnection connection = CreateConnection();
//                MySqlCommand command = connection.CreateCommand();
//                command.Parameters.AddWithValue("@steamid", steamid);
//                command.Parameters.AddWithValue("@ip", ip);
//                command.Parameters.AddWithValue("@hwid", hwid);
//                command.Parameters.AddWithValue("@admin", admin);
//                command.Parameters.AddWithValue("@charactername", characterName);
//                command.Parameters.AddWithValue("@reason", reason);
//                command.Parameters.AddWithValue("@banTime", DBNull.Value);
//                command.Parameters.AddWithValue("@banDuration", 0);
//                //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@banDuration);";
//                command.CommandText = "insert into `bansystem.banlist` (`steamId`,`ip`,`hwid`,`admin`,`reason`,`charactername`,`banTime`,`banDuration`) values(@steamid,@ip,@hwid,@admin,@reason,@charactername,@banTime,@banDuration);";
//                connection.Open();
//                command.ExecuteNonQuery();
//                connection.Close();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//        }

//        public class UnbanResult
//        {
//            public string Id;
//            public string Name;
//        }

//        //public UnbanResult GetBan(string player)
//        //{
//        //    try
//        //    {
//        //        MySqlConnection connection = CreateConnection();

//        //        MySqlCommand command = connection.CreateCommand();
//        //        command.Parameters.AddWithValue("@player", "%" + player + "%");
//        //        command.CommandText = "select * from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` like @player OR `charactername` like @player;";
//        //        connection.Open();
//        //        MySqlDataReader reader = command.ExecuteReader();
//        //        if (reader.Read())
//        //        {
//        //            //ulong steamId = reader.GetUInt64(0);
//        //            string steamId = reader.GetString(0);
//        //            string charactername = reader.GetString(1);
//        //            connection.Close();
//        //            command = connection.CreateCommand();
//        //            command.Parameters.AddWithValue("@steamId", steamId);
//        //            command.Parameters.AddWithValue("@charactername", charactername);
//        //            command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId OR `charactername` = @charactername;";
//        //            connection.Open();
//        //            command.ExecuteNonQuery();
//        //            connection.Close();
//        //            return new UnbanResult() { Id = steamId, Name = charactername };
//        //        }
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        Logger.LogException(ex);
//        //    }
//        //    return null;
//        //}

//        public UnbanResult UnbanPlayer(string player)
//        {
//            try
//            {
//                using (MySqlConnection connection = CreateConnection())
//                {
//                    MySqlCommand command = connection.CreateCommand();
//                    command.Parameters.AddWithValue("@player", "%" + player + "%");
//                    command.Parameters.AddWithValue("@banTime", DBNull.Value);
//                    //command.CommandText = "insert into `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` (`steamId`,`ip`,`hwid`,`admin`,`banMessage`,`charactername`,`banTime`,`banDuration`) values(@csteamid,@ip,@hwid,@admin,@banMessage,@charactername,now(),@banDuration);";
//                    command.CommandText = "UPDATE `bansystem.banlist` SET `banDuration` = '" + 0 + "',`banTime` = @banTime,`admin` = '" + "" + "',`reason` = '" + "" + "' WHERE `charactername` like @player OR `steamId` like @player;";
//                    connection.Open();
//                    command.ExecuteNonQuery();
//                    connection.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine(ex.Message);
//            }
//            return null;
//        }
//    }
//}
