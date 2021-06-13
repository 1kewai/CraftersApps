using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using Log = Log;
using UI = UI;
using Resource = Resource;
using System.Collections.Generic;

namespace DiscordCore
{
    class Program
    {
        //Field
        private DiscordSocketClient client;
        public static CommandService commands;
        public static IServiceProvider services;
        System.Timers.Timer restarter;//再接続タイマー
        Log::Logging logging;
        List<UI::DiscordChatUI> UIList;
        Resource::property prop;
        public static bool Restart = false;

        static void Main(string[] args)
        {
            while (true)
            {
                new Program().Bot().GetAwaiter().GetResult();
                if (!Restart)
                {
                    break;
                }
                Console.WriteLine("Restarting Bot Program...");
                GC.Collect();
            }
        }

        //Bot本体
        public async Task Bot()
        {
            //設定読み込み
            prop = new Resource::property();
            prop.load();

            //loggingの初期化、初期ログ出力
            logging = new Log::Logging("Discord.log");
            logging.log("[Bot] Starting...");

            //Discordインスタンス初期化
            logging.log("[Bot] Loading Discord Library...");
            client = new DiscordSocketClient();
            commands = new CommandService();
            services=new ServiceCollection().BuildServiceProvider();
            logging.log("[Bot] Loaded Discord Library.");

            //restarter準備
            logging.log("[Bot] Starting error-restarter...");
            restarter = new System.Timers.Timer(10 * 1000);
            restarter.Enabled = false;//初期状態では無効
            restarter.AutoReset = true;//成功するまで何度でも試す
            restarter.Elapsed += async (object source, ElapsedEventArgs e) =>
            {
                //切断時の再接続処理
                logging.log("[Bot] reconnecting to Discord...");
                await client.LoginAsync(TokenType.Bot, prop.settings["DiscordToken"]);//ログインを行う
                await client.StartAsync();//接続する
            };
            client.Disconnected += async (Exception e) => {
                //切断時の処理
                logging.log("[Bot] Disconnected from Discord.");
                logging.log("[Bot] error:" + e);
                restarter.Enabled = true;//再接続を試みる
            };
            client.Connected += async () => {
                //接続時の処理
                logging.log("[Bot] Connected to Discord.");
                restarter.Enabled = false;//再接続を中断する
            };

            //イベントのセットアップ
            logging.log("[Bot] Setting up events...");

            logging.log("[Bot] Event setup complete.");

            //GUIのリストへの追加

            //Discordへ接続
            logging.log("[Bot] Logging in to Discord...");
            await client.LoginAsync(TokenType.Bot, prop.settings["DiscordToken"]);
            await client.StartAsync();

            //CLIUI
            bool CLICONT = false;
            while (CLICONT)
            {
                string stdin = Console.ReadLine();
                switch (stdin)
                {
                    //CLIからの終了指示
                    case "q":
                        logging.log("[CLI] Program Termination Requested.");
                        CLICONT = false;
                        Restart = false;
                        break;
                    case "quit":
                        logging.log("[CLI] Program Termination Requested.");
                        CLICONT = false;
                        Restart = false;
                        break;
                    case "exit":
                        logging.log("[CLI] Program Termination Requested.");
                        CLICONT = false;
                        Restart = false;
                        break;

                }
            }
        }

        public void CLIHelp()
        {
            //CLI使用上のヘルプを表示

        }
    }
}
