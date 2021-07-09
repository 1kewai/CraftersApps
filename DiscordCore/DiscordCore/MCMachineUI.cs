using System;
using Discord = Discord;
using Resource = Resource;
using Log = Log;
using System.Diagnostics;
using System.Text.Json;

namespace UI
{
    class MCMachineUI : DiscordChatUI
    {
        ProcessStartInfo boot;
        ProcessStartInfo shutdown;
        ProcessStartInfo GetIP;
        public MCMachineUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            logging.log("[MCMachineUI] UI init");
            Display = "社不クラフトへようこそ！\n\n";
            Display += "操作方法\n";
            Display = "Start : マイクラサーバー起動　Stop : マイクラサーバー停止\n\n";
            Display += "使用後は必ず停止するようにしてください";
            logging.log("[MCMachineUI] preparing Process Info...");
            boot = new ProcessStartInfo("az", "vm start --resource-group " + resourceSet.settings["ResourceGroup"] + " --name " + resourceSet.settings["MCServerName"]);
            shutdown = new ProcessStartInfo("az", "vm deallocate --resource-group " + resourceSet.settings["ResourceGroup"] + " --name " + resourceSet.settings["MCServerName"]);
            GetIP = new ProcessStartInfo("az", "network public-ip list");
            GetIP.RedirectStandardOutput = true;
            refresh().GetAwaiter().GetResult();
            logging.log("[MCMachineUI] init finished.");
        }

        public override void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            if (inputMessage.Content == "Start")
            {
                try
                {
                    logging.log("[MCMachineUI] Starting VM...");
                    Display = "サーバーを起動しています...";
                    refresh().GetAwaiter().GetResult();
                    Process process = Process.Start(boot);
                    process.WaitForExit();
                    logging.log("[MCMachineUI] VM Started. Getting Server IP...");
                    process = Process.Start(GetIP);
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    IP[] ips = JsonSerializer.Deserialize<IP[]>(output);
                    string addr="";
                    foreach(IP i in ips)
                    {
                        if (i.name == resourceSet.settings["MineCraft-ip"]) { addr = i.ipAddress; }
                    }
                    Display = "起動しました！アドレスは" + addr + ":12345です、お楽しみください！";
                    return;
                }catch(Exception e)
                {
                    logging.log("[MCMachineUI] Error occured while processing boot. \n" + e);
                    Display = "エラーが発生しました。\n" + e;
                    return;
                }
            }
            if (inputMessage.Content == "Stop")
            {
                logging.log("[MCMachineUI] Shutting down VM...");
                Display = "サーバーをシャットダウンしています...";
                refresh().GetAwaiter().GetResult();
                Process process = Process.Start(shutdown);
                process.WaitForExit();
                Display = "サーバーをシャットダウンしました！";
            }
        }

        public override void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {

        }

        public class IP
        {
            public object ddosSettings { get; set; }
            public object deleteOption { get; set; }
            public object dnsSettings { get; set; }
            public string etag { get; set; }
            public object extendedLocation { get; set; }
            public string id { get; set; }
            public int idleTimeoutInMinutes { get; set; }
            public string ipAddress { get; set; }
            public Ipconfiguration ipConfiguration { get; set; }
            public object[] ipTags { get; set; }
            public object linkedPublicIpAddress { get; set; }
            public string location { get; set; }
            public object migrationPhase { get; set; }
            public string name { get; set; }
            public object natGateway { get; set; }
            public string provisioningState { get; set; }
            public string publicIpAddressVersion { get; set; }
            public string publicIpAllocationMethod { get; set; }
            public object publicIpPrefix { get; set; }
            public string resourceGroup { get; set; }
            public string resourceGuid { get; set; }
            public object servicePublicIpAddress { get; set; }
            public Sku sku { get; set; }
            public object tags { get; set; }
            public string type { get; set; }
            public object zones { get; set; }

            public class Ipconfiguration
            {
                public object etag { get; set; }
                public string id { get; set; }
                public object name { get; set; }
                public object privateIpAddress { get; set; }
                public object privateIpAllocationMethod { get; set; }
                public object provisioningState { get; set; }
                public object publicIpAddress { get; set; }
                public string resourceGroup { get; set; }
                public object subnet { get; set; }
            }

            public class Sku
            {
                public string name { get; set; }
                public string tier { get; set; }
            }
        }
    }
}
