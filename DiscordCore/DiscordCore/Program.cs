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

        }
    }
}
