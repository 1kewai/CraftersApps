using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord = Discord;
using Resource = Resource;
using Log = Log;

namespace UI
{
    class MCMachineUI : DiscordChatUI
    {
        public MCMachineUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            Display = "社不クラフトへようこそ！\n\n";
            Display += "操作方法\n";
            Display = "Start : マイクラサーバー起動　Stop : マイクラサーバー停止\n\n";
            Display += "使用後は必ず停止するようにしてください";
            refresh().GetAwaiter().GetResult();
        }

        public override void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {

        }

        public override void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {

        }
    }
}
