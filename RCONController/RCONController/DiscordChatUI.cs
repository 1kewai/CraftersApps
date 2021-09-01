using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;
using Resource = Resource;


namespace UI
{
    //DiscordChatを扱いやすくするためのbaseClass
    abstract class DiscordChatUI
    {
        //Field
        public Discord::ITextChannel channel;
        public Log::Logging logging;
        public Resource::ResourceSet resourceSet;
        Task thread;

        public DiscordChatUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources)
        {
            //各fieldの初期化
            thread = new Task(()=> { });
            thread.Start();
            channel = inputChannel;
            this.logging = logging;
            resourceSet = resources;
        }


        public async Task WriteToChatLog(string message)
        {
            //チャンネルに対してメッセージ送信
            await channel.SendMessageAsync(message);
        }

        public async Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            //前のチャットを処理しているスレッドが完了するまで待機
            thread.Wait();
            //新たなメッセージを処理するスレッドを作成
            thread = new Task(() =>
              {

                  AllMessage(inputMessage);
                  if (inputMessage.Channel.Id != channel.Id) { return; }
                  if (inputMessage.Author.IsBot) { return; }
                  UIMessageReceived(inputMessage);
              });
            //スレッド開始
            thread.Start();
        }

        public async Task ReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {
            //前のチャットを処理しているスレッドが完了するまで待機
            thread.Wait();
            //新たなメッセージを処理するスレッドを作成
            thread = new Task(() =>
              {

                  if (inputchannel.Id != channel.Id) { return; }
                  UIReactionAdded(cache, inputchannel, inputReaction);
              });
            //スレッド開始
            thread.Start();
        }

        //Abstract
        //ChatUI側で実際に実装するクラスのabstract
        public abstract void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage);
        public abstract void AllMessage(Discord::WebSocket.SocketMessage inputMessage);
        public abstract void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction);
    }
}