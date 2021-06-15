using Discord;
using Discord.WebSocket;
using Discord = Discord;
using Log = Log;
using Resource = Resource;

namespace UI
{
    class TestUI : DiscordChatUI
    {
        public TestUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {

        }
        public override void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            if (!inputMessage.Author.IsBot)
            {
                Display = inputMessage.Content;
                logging.log("Mirror working.");
            }
        }

        public override void UIReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel inputchannel, SocketReaction inputReaction)
        {
            Display = "\""+inputReaction.Emote.Name+ "\"";
            logging.log(inputReaction.Emote.Name);
        }
    }
}
