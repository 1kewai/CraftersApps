using System;
using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;


namespace UI
{
    abstract class DiscordChatUI
    {
        //Field
        public string Display;
        public Discord::ITextChannel channel;
        public Log::Logging logging;

        public DiscordChatUI(Discord::ITextChannel inputChannel,  Log::Logging logging, string initialmessage)
        {
            channel = inputChannel;
            Display = initialmessage;
            this.logging = logging;
            this.logging.MessageSend(Display);
            refresh().GetAwaiter().GetResult();
        }

        public async Task refresh()
        {
            await channel.SendMessageAsync(Display);
        }

        public abstract Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage);

        public abstract Task ReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction);
    }
}