using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace fr34kyn01535.GlobalBan
{
    internal class CommandHandler
    {
        DiscordSocketClient _client;
        CommandService _service;

        public void InitializeAsync(DiscordSocketClient client)
        {
            _client = client;
            _service = new CommandService();_service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            _client.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            var context = new SocketCommandContext(_client, msg);
            if (context.User.IsBot) return;

            // Mute check
            //var userAccount = UserAccounts.GetAccount(context.User);
            //if (userAccount.IsMuted)
            //{
            //    await context.Message.DeleteAsync();
            //    return;
            //}

            // Leveling up
            //Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);

            int argPos = 0;
            if (msg.HasStringPrefix(GlobalBan.Instance.Configuration.Instance.Cmd_Prefix, ref argPos)
                || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos, null);
                if (!result.IsSuccess && result.Error != CommandError.UnknownCommand)
                {
                    Console.WriteLine(result.ErrorReason);
                }
            }
        }
    }
}