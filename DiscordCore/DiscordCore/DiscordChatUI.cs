using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;
using Resource = Resource;


namespace UI
{
    abstract class DiscordChatUI
    {
        //Field
        public string Display;
        public Discord::ITextChannel channel;
        public Log::Logging logging;
        public Discord::Rest.RestUserMessage CurrentMessage;
        public Resource::ResourceSet resourceSet;


        public DiscordChatUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources)
        {
            channel = inputChannel;
            Display = initialmessage;
            this.logging = logging;
            this.logging.MessageSend(Display);
            resourceSet=resources;
            refresh().GetAwaiter().GetResult();
        }

        public async Task refresh()
        {
            if (CurrentMessage != null)
            {
                await CurrentMessage.DeleteAsync();
            }
            CurrentMessage = (Discord::Rest.RestUserMessage)await channel.SendMessageAsync(Display);
<<<<<<< HEAD
        }

        public async Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            if (inputMessage.Author.IsBot)
            {
                return;
            }
            UIMessageReceived(inputMessage);
            await refresh();
        }

        public async Task ReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {
            if (inputReaction.User.Value.IsBot)
            {
                return;
            }
            UIReactionAdded(cache, inputchannel, inputReaction);
            await refresh();
        }

        //Abstract
        public abstract void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage);
        public abstract void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction);
=======
        }

        //Abstract
        public abstract Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage);
        public abstract Task ReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction);
>>>>>>> a033b71e2aa5b46e1d8d5ea92a93bf95992a89fd
    }
}