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
        //プロセスを開始するための情報
        ProcessStartInfo boot;//MineCraftサーバーの起動プロセス
        ProcessStartInfo shutdown;//MineCraftサーバーの終了プロセス
        ProcessStartInfo GetIP;//MineCraftサーバーのIP取得に用いるプロセス

        //コンストラクタ
        public MCMachineUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            logging.log("[MCMachineUI] UI init");

            //プロセス開始情報の準備
            logging.log("[MCMachineUI] preparing Process Info...");
            boot = new ProcessStartInfo("az", "vm start --resource-group " + resourceSet.settings["ResourceGroup"] + " --name " + resourceSet.settings["MCServerName"]);
            shutdown = new ProcessStartInfo("az", "vm deallocate --resource-group " + resourceSet.settings["ResourceGroup"] + " --name " + resourceSet.settings["MCServerName"]);
            GetIP = new ProcessStartInfo("az", "network public-ip list");
            GetIP.RedirectStandardOutput = true;

            Display = "";
        }

        //メッセージ受信時の動作
        public override void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            if (inputMessage.Content == "Start")//MineCraftサーバー起動
            {
                try
                {
                    logging.log("[MCMachineUI] Starting VM...");

                    //サーバー起動中のことを表示
                    Display = "サーバーを起動しています...";
                    refresh().GetAwaiter().GetResult();

                    //起動プロセス呼び出し
                    Process process = Process.Start(boot);
                    process.WaitForExit();

                    //サーバーIP取得
                    logging.log("[MCMachineUI] VM Started. Getting Server IP...");
                    process = Process.Start(GetIP);
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    IP[] ips = JsonSerializer.Deserialize<IP[]>(output);//デシリアライズ先オブジェクト作成
                    string addr = "";
                    //デシリアライズ
                    foreach (IP i in ips)
                    {
                        if (i.name == resourceSet.settings["MineCraft-ip"]) { addr = i.ipAddress; }
                    }

                    //メッセージ送信
                    Display = "@everyone 起動しました！アドレスは" + addr + ":12345です、お楽しみください！";
                    return;
                }
                catch (Exception e)
                {
                    //エラー発生時はログを出力しエラー発生の旨Discordに送信する
                    logging.log("[MCMachineUI] Error occured while processing boot. \n" + e);
                    Display = "エラーが発生しました。\n" + e;
                    return;
                }
            }
            if (inputMessage.Content == "Stop")
            {
                //サーバー停止時処理
                logging.log("[MCMachineUI] Shutting down VM...");
                Display = "サーバーをシャットダウンしています...";
                refresh().GetAwaiter().GetResult();
                Process process = Process.Start(shutdown);
                process.WaitForExit();
                Display = "サーバーをシャットダウンしました！";
            }
            if (inputMessage.Content == "Maintenance")
            {
                logging.log("[MCMachineUI] Starting VM for maintenance...");

                try
                {
                    //サーバー起動中のことを表示
                    Display = "メンテナンスのためサーバーを起動しています...";
                    refresh().GetAwaiter().GetResult();

                    //起動プロセス呼び出し
                    Process process = Process.Start(boot);
                    process.WaitForExit();

                    //サーバーIP取得
                    logging.log("[MCMachineUI] VM Started. Getting Server IP...");
                    process = Process.Start(GetIP);
                    process.WaitForExit();
                    string output = process.StandardOutput.ReadToEnd();
                    IP[] ips = JsonSerializer.Deserialize<IP[]>(output);//デシリアライズ先オブジェクト作成
                    string addr = "";
                    //デシリアライズ
                    foreach (IP i in ips)
                    {
                        if (i.name == resourceSet.settings["MineCraft-ip"]) { addr = i.ipAddress; }
                    }

                    //メッセージ送信
                    Display = "メンテナンスのためサーバーを起動しました。アドレスは" + addr + "です。遊べない場合がありますのでご注意ください。";
                    return;
                }
                catch (Exception e)
                {
                    //エラー発生時はログを出力しエラー発生の旨Discordに送信する
                    logging.log("[MCMachineUI] Error occured while processing boot. \n" + e);
                    Display = "エラーが発生しました。\n" + e;
                    return;
                }
            }
        }

        public override void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {

        }

        //Azureから取得したデータをデシリアライズするためのオブジェクト
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