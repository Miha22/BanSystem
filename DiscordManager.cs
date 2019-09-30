//using DiscordTutorialBot.Core;
using SDG.Unturned;
using Discord.Commands;
using Steamworks;
using Discord;

namespace fr34kyn01535.GlobalBan
{
    class DiscordManager : ModuleBase<SocketCommandContext>
    {
        private readonly Color[] colors = { Color.Default, Color.Blue, Color.DarkBlue, Color.DarkerGrey, Color.DarkGreen, Color.DarkGrey, Color.DarkMagenta, Color.DarkOrange, Color.DarkPurple, Color.DarkRed, Color.DarkTeal, Color.Gold, Color.Green, Color.LighterGrey, Color.LightGrey, Color.LightOrange, Color.Magenta, Color.Orange, Color.Purple, Color.Red, Color.Teal};

        internal void SendEmbedError(string message)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Error");
            embed.WithDescription(message);
            embed.WithColor(Color.Red);
            Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        internal void SendChannelBanMessage(string name, string admin, string duration, string reason = "")
        {

            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("Ban Report");
            embed.WithDescription($"Player: {name} was banned\r\nReason: {(reason == "" ? "undefined" : reason)}\r\nDuration: {duration}\r\nAdmin: {admin}");
            embed.WithColor(Color.Blue);
            Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        internal void SendChannelUnbanMessage(string message)
        {
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("UnBan Report");
            embed.WithDescription(message);
            embed.WithColor(GlobalBan.Instance.Configuration.Instance.Discord_Ban_Color == Color.Default ? colors[new System.Random().Next(22)] : GlobalBan.Instance.Configuration.Instance.Discord_Ban_Color);
            Context.Channel.SendMessageAsync("", false, embed.Build());
        }

        [Command("unban")]
        public void Unban([Remainder] string message)
        {
            string[] command = message.Trim().Split(' ');
        }

        [Command("kick")]
        public void Kick([Remainder] string message)
        {
            string[] command = message.Trim().Split(' ');
            if(command.Length == 0 || command.Length > 2)
            {
                SendEmbedError($"Invalid command usage. Syntax: 1. {GlobalBan.Instance.Configuration.Instance.Cmd_Prefix}kick <player_name> \r\n2. {GlobalBan.Instance.Configuration.Instance.Cmd_Prefix}kick <player_name> <reason>");
                return;
            }
            if (!PlayerTool.tryGetSteamPlayer(command[0], out SteamPlayer targetPlayer))
            {
                //UnturnedChat.Say(caller, GlobalBan.Instance.Translate("command_generic_player_not_found"));
                //EmbedBuilder embed = new EmbedBuilder();
                //embed.WithTitle("Error");
                //embed.WithDescription();
                //embed.WithColor(Color.Red);
                //Context.Channel.SendMessageAsync("", false, embed.Build());
                SendEmbedError($"Failed to find a player called: {command[0]}!");
                return;
            }
            Provider.kick(targetPlayer.playerID.steamID, command.Length == 2 ? command[1] : "");
        }

        [Command("ban")]
        public void Ban([Remainder] string message)
        {
            string[] commandOld = message.Trim().Split(' ');
            try
            {
                string[] command = new string[commandOld.Length + 1];
                command[0] = Context.User.Username + " (Discord)";
                for (byte i = 1; i < commandOld.Length; i++)
                    command[i] = commandOld[i - 1];
                CommandBan.Instance.Execute(null, command);
            }
            catch (System.Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, $"Exception was generated from executing ban command from Discord channel: {Context.Channel.Name} \r\n by: {Context.User.Username}");
            }
        }

        [Command("unban")]
        public void UnBan([Remainder] string message)
        {
            string[] command = message.Trim().Split(' ');
            try
            {
                CommandUnban.Instance.Execute(null, command);
            }
            catch (System.Exception ex)
            {
                Rocket.Core.Logging.Logger.LogException(ex, $"Exception was generated from executing ban command from Discord channel: {Context.Channel.Name} \r\n by: {Context.User.Username}");
            }
        }
    }
}
