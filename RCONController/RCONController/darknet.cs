using Discord;
using Discord.WebSocket;
using Log;
using Resource;
using System;
using System.Diagnostics;

namespace UI
{
    class Darknet : DiscordChatUI
    {
        ProcessStartInfo wget;
        Process wget_process;
        ProcessStartInfo darknet;
        Process darknet_process;
        public Darknet(ITextChannel inputChannel, Logging logging, string initialmessage, ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            darknet = new ProcessStartInfo("./darknet", "detect cfg/yolov3.cfg yolov3.weights image.jpg");
        }

        public override void AllMessage(SocketMessage inputMessage)
        {
            return;
        }

        public override void UIMessageReceived(SocketMessage inputMessage)
        {
            foreach(var i in inputMessage.Attachments)
            {
                try
                {
                    logging.log("[darknet] Attachment received.");
                    logging.log("[darknet] downloading " + i.Url);
                    wget = new ProcessStartInfo("wget", i.Url + " -O image.jpg");
                    wget_process = Process.Start(wget);
                    wget_process.WaitForExit();
                    darknet_process = Process.Start(darknet);
                    darknet_process.WaitForExit();
                    channel.SendFileAsync("predictions.jpg").GetAwaiter().GetResult();
                    return;
                }catch(Exception e)
                {
                    WriteToChatLog("エラーが発生しました。").GetAwaiter().GetResult();
                }
            }
            foreach (var i in inputMessage.Embeds)
            {
                try
                {
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
                    WriteToChatLog("エラーが発生しました。").GetAwaiter().GetResult();
                }
            }
        }

        public override void UIReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel inputchannel, SocketReaction inputReaction)
        {
            return;
        }
    }
}
