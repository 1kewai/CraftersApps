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
using System.Threading;

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
            client.MessageReceived += MessageReceived;
            client.ReactionAdded += ReactionAdded;
            logging.log("[Bot] Event setup complete.");

            //Discordへ接続
            logging.log("[Bot] Logging in to Discord...");
            await client.LoginAsync(TokenType.Bot, prop.settings["DiscordToken"]);
            await client.StartAsync();

            //UIのリストへの追加
            Thread.Sleep(10000);


            //MirrorUI(Test)の追加
            UIList = new List<UI::DiscordChatUI>();
            SocketTextChannel channel = client.GetGuild(ulong.Parse(prop.settings["GuildID"])).GetTextChannel(853932957445128202);
            UIList.Add(new UI::TestUI(channel, logging, "ようこそ！"));


            //CLIUI
            bool CLICONT = true;
            while (CLICONT)
            {
                try {
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
                        //ヘルプ表示
                        case "h":
                            CLIHelp();
                            break;
                        case "help":
                            CLIHelp();
                            break;
                        //Botプログラム再起動
                        case "restart":
                            Restart = true;
                            CLICONT = false;
                            break;
                        //メッセージ削除
                        case "delete":
                            Console.Write("RoomID:");
                            string RoomID_del = Console.ReadLine();
                            Console.Write("MessageID:");
                            string MessageID_del = Console.ReadLine();
                            IMessage del = await client.GetGuild(ulong.Parse(prop.settings["GuildID"])).GetTextChannel(ulong.Parse(RoomID_del)).GetMessageAsync(ulong.Parse(MessageID_del));
                            string content = del.Author + " : " + del.Content;
                            await del.DeleteAsync();
                            logging.log("Deleted the message :" + content);
                            break;
                        //メッセージ送信
                        case "send":
                            //roomIDとメッセージの送信内容の入力
                            Console.Write("RoomID:");
                            ulong roomID_Send = ulong.Parse(Console.ReadLine());
                            Console.Write("Message:");
                            string message_Send = Console.ReadLine();
                            //送信
                            logging.MessageSend(message_Send);
                            await client.GetGuild(ulong.Parse(prop.settings["GuildID"])).GetTextChannel(roomID_Send).SendMessageAsync(message_Send);
                            break;
                        //ステータス確認
                        case "ConnectionState":
                            Console.WriteLine(client.ConnectionState.ToString());
                            break;
                        case "latency":
                            Console.WriteLine(client.Latency);
                            break;
                        //Memory関係/不停止メンテナンス用
                        case "Memory":
                            Console.WriteLine("Memory usage : " + GC.GetTotalMemory(false));
                            break;
                        case "GC":
                            GC.Collect();
                            Console.WriteLine("Collected.");
                            break;
                    }
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            //終了処理
            logging.finalize();
        }

        public void CLIHelp()
        {
            //CLI使用上のヘルプを表示
            Console.WriteLine("h,help : show this help page.");
            Console.WriteLine("q,quit,exit : Shut down this bot program.");
            Console.WriteLine("delete : delete the message designated by messageID.");
            Console.WriteLine("send : send a message to the designated Text channel.");
            Console.WriteLine("restart : restart this bot Program.");
            Console.WriteLine("ConnectionState : Get the status of connection against Discord.");
            Console.WriteLine("latency : Get the latency between the server and Discord.");
            Console.WriteLine("Memory : Display the memory usage of this Bot.");
            Console.WriteLine("GC : Force Collector to do GC immediately.");
        }

        //イベントハンドラ
        //メッセージ受信時
        public async Task MessageReceived(SocketMessage inputMessage)
        {

            logging.MessageReceived(inputMessage);
            foreach (UI::DiscordChatUI i in UIList)
            {
                await i.MessageReceived(inputMessage);
            }
        }
        //リアクション追加時
        public async Task ReactionAdded(Cacheable<IUserMessage, ulong> cache, ISocketMessageChannel inputchannel, SocketReaction inputReaction)
        {
            foreach(UI::DiscordChatUI i in UIList)
            {
                await i.ReactionAdded(cache, inputchannel, inputReaction);
            }
        }
    }
}