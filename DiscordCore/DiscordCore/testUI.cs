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

        public override void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {
            Display = "\""+inputReaction.Emote.Name+ "\"";
            logging.log(inputReaction.Emote.Name);
        }
    }
}
