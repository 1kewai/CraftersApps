using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord = Discord;
using Resource = Resource;
using Log = Log;
using System.Diagnostics;

namespace UI
{
    class ServerStatus : DiscordChatUI
    {
        ProcessStartInfo listVM;
        ProcessStartInfo listIP;
        public ServerStatus(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            //VM, NWの情報を取得する準備をする
            listVM = new ProcessStartInfo("az", "vm list");
            listIP = new ProcessStartInfo("az", "network public-ip list");
            listVM.RedirectStandardOutput = true;
            listIP.RedirectStandardOutput = true;

            //初期画面を表示して終了する
            welcomeScreen();
        }

        public override void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {

        }

        public override void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {

        }

        public void welcomeScreen()
        {
            Display = "サーバーステータス確認UI\n\n";
            Display += "VMList : Azure VMの一覧と情報を表示\n";
            Display += "IPList : Azure public-ipの一覧を表示";
            return;
        }

        public void reportVM()
        {

        }

        public void reportIP()
        {

        }
    }
}
