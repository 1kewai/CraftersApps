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
        ProcessStartInfo listNW;
        public ServerStatus(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            //VM, NWの情報を取得する準備をする
            string VMInfo;
            string NWInfo;
            listVM = new ProcessStartInfo("az", "vm list");
            listNW = new ProcessStartInfo("az", "network public-ip list");
            listVM.RedirectStandardOutput = true;
            listNW.RedirectStandardOutput = true;

            //コマンドを実行
            Process VMProcess = Process.Start(listVM);
            Process NWProcess = Process.Start(listNW);

            //実行終了待ち
            VMProcess.WaitForExit();
            NWProcess.WaitForExit();

            //実行結果取得
            VMInfo = VMProcess.StandardOutput.ReadToEnd();
            NWInfo = NWProcess.StandardOutput.ReadToEnd();

            //UIにこれらを吐いて終了
            Display = VMInfo + "\n\n" + NWInfo;
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
