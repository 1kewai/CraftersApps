using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;
using Resource = Resource;


namespace UI
{
    abstract class DiscordChatUI
    {
        //Field
        public Discord::ITextChannel channel;
        public Log::Logging logging;
        public Resource::ResourceSet resourceSet;
        Task thread;


        public DiscordChatUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources)
        {
            thread = new Task(()=> { });
            thread.Start();
            channel = inputChannel;
            this.logging = logging;
            resourceSet = resources;
        }


        public async Task WriteToChatLog(string message)
        {
            await channel.SendMessageAsync(message);
        }

        public async Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            thread.Wait();
            thread = new Task(() =>
              {

                  AllMessage(inputMessage);
                  if (inputMessage.Channel.Id != channel.Id) { return; }
                  if (inputMessage.Author.IsBot) { return; }
                  UIMessageReceived(inputMessage);
              });
            thread.Start();
        }

        public async Task ReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {
            thread.Wait();
            thread = new Task(() =>
              {

                  if (inputchannel.Id != channel.Id) { return; }
                  UIReactionAdded(cache, inputchannel, inputReaction);
              });
            thread.Start();
        }

        //Abstract
        public abstract void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage);
        public abstract void AllMessage(Discord::WebSocket.SocketMessage inputMessage);
        public abstract void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction);
    }
}