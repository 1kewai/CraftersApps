using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;


namespace UI
{
    class DiscordChatUI
    {
        //Field
        string Display;
        Discord::ITextChannel channel;

        public DiscordChatUI(Discord::ITextChannel inputChannel,  Log::Logging logging, string initialmessage)
        {
            channel = inputChannel;
            Display = initialmessage;
            logging.MessageSend(Display);
            refresh().GetAwaiter().GetResult();
        }

        public async Task refresh()
        {
            await channel.SendMessageAsync(Display);
        }

        public async Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            //個々の継承先で実装
        }

        public async Task ReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {

        }
    }
}