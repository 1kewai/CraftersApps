using Discord;
using Discord.WebSocket;
using Log;
using Resource;
using System;
using System.Diagnostics;

namespace UI
{
    //画像認識UI
    class Darknet : DiscordChatUI
    {
        ProcessStartInfo wget;//Discordからの画像取得に使用するwgetのプロセス開始情報
        Process wget_process;//wgetのプロセス
        ProcessStartInfo darknet;//darknetのプロセス開始情報
        Process darknet_process;//darknetのプロセス
        public Darknet(ITextChannel inputChannel, Logging logging, string initialmessage, ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            //darknetのプロセス開始の準備をする
            darknet = new ProcessStartInfo("./darknet", "detect cfg/yolov3.cfg yolov3.weights image.jpg");
        }

        public override void AllMessage(SocketMessage inputMessage)
        {
            //実装しない
            return;
        }

        public override void UIMessageReceived(SocketMessage inputMessage)
        {
            //メッセージ受信時の動作
            foreach(var i in inputMessage.Attachments)
            {
                try
                {
                    logging.log("[darknet] Attachment received.");
                    logging.log("[darknet] downloading " + i.Url);
                    //画像をDiscordから取得する
                    wget = new ProcessStartInfo("wget", i.Url + " -O image.jpg");
                    wget_process = Process.Start(wget);
                    wget_process.WaitForExit();
                    //darknetによる画像認識を行う
                    darknet_process = Process.Start(darknet);
                    darknet_process.WaitForExit();
                    //予測結果を送信する
                    channel.SendFileAsync("predictions.jpg").GetAwaiter().GetResult();
                    return;
                }catch(Exception e)
                {
                    //エラー時処理
                    WriteToChatLog("エラーが発生しました。").GetAwaiter().GetResult();
                }
            }
            foreach (var i in inputMessage.Embeds)
            {
                try
                {
                    //Embedsに対しても添付ファイルと同じ処理を行う
                    logging.log("[darknet] Embed received.");
                    if (i.Image==null)
                    {
                        return;
                    }
                    logging.log("[darknet] downloading " + i.Url);
                    wget = new ProcessStartInfo("wget", i.Url + " -o darknet/image.jpg");
                    wget_process = Process.Start(wget);
                    wget_process.WaitForExit();
                    darknet_process = Process.Start(darknet);
                    darknet_process.WaitForExit();
                    channel.SendFileAsync("darknet/predictions.jpg").GetAwaiter().GetResult();
                    return;
                }
                catch (Exception e)
                {
                    //エラー時処理
                    WriteToChatLog("エラーが発生しました。").GetAwaiter().GetResult();
                }
            }
        }

        public override void UIReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel inputchannel, SocketReaction inputReaction)
        {
            //実装しない
            return;
        }
    }
}
