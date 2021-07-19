using System;
using System.Text;
using Discord = Discord;
using Resource = Resource;
using Log = Log;
using System.Diagnostics;
using System.Text.Json;
using System.IO;

namespace UI
{
    class ServerStatus : DiscordChatUI
    {
        ProcessStartInfo listVM;//VM一覧を表示するプロセスの開始情報
        ProcessStartInfo listIP;//IP一覧を表示するプロセスの開始情報

        //コンストラクタ
        public ServerStatus(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage, Resource::ResourceSet resources) : base(inputChannel, logging, initialmessage, resources)
        {
            logging.log("[ServerStatus] UI init");
            //VM, NWの情報を取得する準備をする
            logging.log("[ServerStatus] Preparing Process Info...");
            listVM = new ProcessStartInfo("az", "vm list");
            listIP = new ProcessStartInfo("az", "network public-ip list");
            listVM.RedirectStandardOutput = true;
            listIP.RedirectStandardOutput = true;

            //初期画面を表示して終了する
            welcomeScreen();
            refresh().GetAwaiter().GetResult();
            logging.log("[ServerStatus] init finished.");
        }

        public override void UIMessageReceived(Discord::WebSocket.SocketMessage inputMessage)//メッセージ受信時処理
        {
            //inputMessageの内容に応じて画面遷移や書き込み等を行う
            if (inputMessage.Content == "VMList")
            {
                reportVM();
            }
            if (inputMessage.Content == "IPList")
            {
                reportIP();
            }
            if (inputMessage.Content == "ReportAll")
            {
                reportAll();
            }
        }

        public override void UIReactionAdded(Discord::Cacheable<Discord::IUserMessage, ulong> cache, Discord::WebSocket.ISocketMessageChannel inputchannel, Discord::WebSocket.SocketReaction inputReaction)
        {

        }

        public void welcomeScreen()
        {
            //ウェルカムスクリーン
            Display = "サーバーステータス確認UI\n\n";
            Display += "VMList : Azure VMの一覧と情報を表示\n";
            Display += "IPList : Azure public-ipの一覧を表示\n";
            Display += "ReportAll : 全出力を送信";
            return;
        }

        public void reportVM()
        {
            //VMリスト表示
            try
            {
                logging.log("[ServerStatus] Connecting to azure...");

                //azプロセス開始
                Process process = Process.Start(listVM);
                process.WaitForExit();

                //プロセス結果取得
                string output = process.StandardOutput.ReadToEnd();
                string result = "Azure VMステータス\n\n";
                logging.log("[ServerStatus] Information fetch finished. Serializing...");

                //デシリアライズするオブジェクトの作成とJsonデシリアライズ
                VM[] vms = JsonSerializer.Deserialize<VM[]>(output);
                foreach (VM i in vms)
                {
                    string data = "";
                    data += "VM名 : " + i.name;
                    data += "\n位置 : " + i.location;
                    data += "\nVMタイプ : " + i.hardwareProfile.vmSize;
                    data += "\n\n";
                    result += data;
                }

                //結果を送信
                WriteToChatLog(result).GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                //エラー発生時はログを出力した上でエラー発生の旨Discordに送信する
                logging.log("[ServerStatus] Error occured while processing remortVM(). \n" + e);
                string result = "Azure VMステータス確認中にエラーが発生しました。しばらく待ってもう一度お試しください。\n" + e;
                WriteToChatLog(result).GetAwaiter().GetResult();
            }
        }

        public void reportIP()//IPアドレスの情報を取得して表示
        {
            try
            {
                logging.log("[ServerStatus] Connecting to azure...");

                //プロセス開始
                Process process = Process.Start(listIP);
                process.WaitForExit();

                //出力取得
                string output = process.StandardOutput.ReadToEnd();
                string result = "Azure public-ipリスト\n\n";
                logging.log("[ServerStatus] Information fetch finished. Serializing...");

                //デシリアライズ先オブジェクトの作成とJsonデシリアライズ
                NW[] nws = JsonSerializer.Deserialize<NW[]>(output);
                foreach (NW i in nws)
                {
                    result += "名前 : " + i.name;
                    result += "\nアドレス : " + i.ipAddress;
                    result += "\n\n";
                }

                //結果を送信
                WriteToChatLog(result).GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                //エラー発生時はログを出力した上でエラー発生の旨Discordに送信する
                logging.log("[ServerStatus] Error occured while processing remortNW(). \n" + e);
                string result = "Azure IPステータス確認中にエラーが発生しました。しばらく待ってもう一度お試しください。\n" + e;
                WriteToChatLog(result).GetAwaiter().GetResult();
            }
        }

        public void reportAll()
        {
            //全出力取得を行いログファイルとして保存、ファイルとして送信
            try
            {
                logging.log("[ServerStatus] Connecting to azure...");

                //プロセス開始
                Process process0 = Process.Start(listVM);
                Process process1 = Process.Start(listIP);
                process0.WaitForExit();
                process1.WaitForExit();
                logging.log("[ServerStatus] Saving log data...");

                //出力のファイル保存
                Encoding enc = Encoding.GetEncoding("UTF-8");
                using (StreamWriter w = new StreamWriter("AllReport.log", true, enc))
                {
                    w.WriteLine(process0.StandardOutput.ReadToEnd());
                    w.WriteLine(process1.StandardOutput.ReadToEnd());
                }

                //送信
                logging.log("Sending report file...");
                channel.SendFileAsync("AllReport.log");

            }
            catch (Exception e)
            {
                //エラー発生時はログを出力した上でエラー発生の旨Discordに送信する
                logging.log("[ServerStatus] Error occured while processing remortAll(). \n" + e);
                string result = "エラーが発生しました。\n" + e;
                WriteToChatLog(result).GetAwaiter().GetResult();
            }
        }

        //azからのJson出力をデシリアライズするためのオブジェクト
        public class VM
        {
            public object additionalCapabilities { get; set; }
            public object availabilitySet { get; set; }
            public object billingProfile { get; set; }
            public Diagnosticsprofile diagnosticsProfile { get; set; }
            public object evictionPolicy { get; set; }
            public object extendedLocation { get; set; }
            public object extensionsTimeBudget { get; set; }
            public Hardwareprofile hardwareProfile { get; set; }
            public object host { get; set; }
            public object hostGroup { get; set; }
            public string id { get; set; }
            public object identity { get; set; }
            public object instanceView { get; set; }
            public object licenseType { get; set; }
            public string location { get; set; }
            public string name { get; set; }
            public Networkprofile networkProfile { get; set; }
            public Osprofile osProfile { get; set; }
            public object plan { get; set; }
            public object platformFaultDomain { get; set; }
            public object priority { get; set; }
            public string provisioningState { get; set; }
            public object proximityPlacementGroup { get; set; }
            public string resourceGroup { get; set; }
            public object resources { get; set; }
            public object scheduledEventsProfile { get; set; }
            public object securityProfile { get; set; }
            public Storageprofile storageProfile { get; set; }
            public object tags { get; set; }
            public string type { get; set; }
            public object userData { get; set; }
            public object virtualMachineScaleSet { get; set; }
            public string vmId { get; set; }
            public object zones { get; set; }

            public class Diagnosticsprofile
            {
                public Bootdiagnostics bootDiagnostics { get; set; }
            }

            public class Bootdiagnostics
            {
                public bool enabled { get; set; }
                public object storageUri { get; set; }
            }

            public class Hardwareprofile
            {
                public string vmSize { get; set; }
            }

            public class Networkprofile
            {
                public object networkApiVersion { get; set; }
                public object networkInterfaceConfigurations { get; set; }
                public Networkinterface[] networkInterfaces { get; set; }
            }

            public class Networkinterface
            {
                public object deleteOption { get; set; }
                public string id { get; set; }
                public object primary { get; set; }
                public string resourceGroup { get; set; }
            }

            public class Osprofile
            {
                public object adminPassword { get; set; }
                public string adminUsername { get; set; }
                public bool allowExtensionOperations { get; set; }
                public string computerName { get; set; }
                public object customData { get; set; }
                public Linuxconfiguration linuxConfiguration { get; set; }
                public bool requireGuestProvisionSignal { get; set; }
                public object[] secrets { get; set; }
                public object windowsConfiguration { get; set; }
            }

            public class Linuxconfiguration
            {
                public bool disablePasswordAuthentication { get; set; }
                public Patchsettings patchSettings { get; set; }
                public bool provisionVmAgent { get; set; }
                public object ssh { get; set; }
            }

            public class Patchsettings
            {
                public string assessmentMode { get; set; }
                public string patchMode { get; set; }
            }

            public class Storageprofile
            {
                public object[] dataDisks { get; set; }
                public Imagereference imageReference { get; set; }
                public Osdisk osDisk { get; set; }
            }

            public class Imagereference
            {
                public string exactVersion { get; set; }
                public object id { get; set; }
                public string offer { get; set; }
                public string publisher { get; set; }
                public string sku { get; set; }
                public string version { get; set; }
            }

            public class Osdisk
            {
                public string caching { get; set; }
                public string createOption { get; set; }
                public object deleteOption { get; set; }
                public object diffDiskSettings { get; set; }
                public int? diskSizeGb { get; set; }
                public object encryptionSettings { get; set; }
                public object image { get; set; }
                public Manageddisk managedDisk { get; set; }
                public string name { get; set; }
                public string osType { get; set; }
                public object vhd { get; set; }
                public object writeAcceleratorEnabled { get; set; }
            }

            public class Manageddisk
            {
                public object diskEncryptionSet { get; set; }
                public string id { get; set; }
                public string resourceGroup { get; set; }
                public string storageAccountType { get; set; }
            }
        }

        public class NW
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