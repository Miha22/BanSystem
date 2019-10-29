﻿using MySql.Data.MySqlClient;
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
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                string hwid = GlobalBan.Instance.GetHWidString(player.Player.channel.owner.playerID.hwid);
                command.CommandText = "select `banDuration`,`banTime` from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `hwid` = '" + hwid + "' OR `charactername` = '" + player.CharacterName.ToLower() + "' AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                MySqlDataReader res = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                if (res != null && res.Read() && res.HasRows)
                {
                    date = ((DateTime)res["banTime"]).AddSeconds(res.GetInt32("banDuration")).AddHours(-GlobalBan.Instance.UTCoffset);
                    connection.Close();
                    return true;
                }
                connection.Close();

                connection = CreateConnection();
                command = connection.CreateCommand();
                command.CommandText = "select `id` from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `steamId` = '" + player.CSteamID.ToString() + "' OR `hwid` = '" + hwid + "' OR `charactername` = '" + player.CharacterName.ToLower() + "';";
                connection.Open();         
                MySqlDataReader result = command.ExecuteReader(System.Data.CommandBehavior.SingleRow);
                if(result != null && result.Read() && result.HasRows)
                {
                    command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE `id` = '" + result.GetInt32("id") + "';";
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            date = DateTime.Now;
            return false;
        }
        //UPDATE `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` SET `charactername`= '" + player.CharacterName.ToLower() + "' WHERE  `id`=1;
        public bool IsBanned(CSteamID steamID)
        {
            try
            {
                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                SteamGameServerNetworking.GetP2PSessionState(steamID, out P2PSessionState_t pConnectionState);
                string ip = SDG.Unturned.Parser.getIPFromUInt32(pConnectionState.m_nRemoteIP);
                command.CommandText = "select 1 from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` WHERE (`steamId` = '" + steamID.ToString() + "' OR `ip` = '" + ip + "') AND (banDuration is null OR ((banDuration + UNIX_TIMESTAMP(banTime)) > UNIX_TIMESTAMP()));";
                connection.Open();
                object result = command.ExecuteScalar();
                if (result != null)
                {
                    connection.Close();
                    return true;
                }
                connection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return false;
        }

        
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
                //SET @@session.time_zone='+00:00';
                if (test == null)
                {
                    //command.CommandText = "SET @@session.time_zone ='+00:00';";
                    //command.ExecuteNonQuery();
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

        public void BanPlayer(string characterName, string steamid, string ip, string hwid, string admin, string banMessage, uint duration, DateTime banTime)
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
            public string Id;
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
                    //ulong steamId = reader.GetUInt64(0);
                    string steamId = reader.GetString(0);
                    string charactername = reader.GetString(1);
                    connection.Close();
                    command = connection.CreateCommand();
                    command.Parameters.AddWithValue("@steamId", steamId);
                    command.Parameters.AddWithValue("@charactername", charactername);
                    command.CommandText = "delete from `" + GlobalBan.Instance.Configuration.Instance.DatabaseTableName + "` where `steamId` = @steamId or `charactername` = @charactername;";
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
