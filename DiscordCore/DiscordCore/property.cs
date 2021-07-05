using System;
using System.Collections.Generic;
using System.IO;
using Discord = Discord;

namespace Resource
{
    class property
    {
        //Field
        public Dictionary<string,string> settings;
        public void load()
        {
            //読み込みを行うための領域確保
            settings = new Dictionary<string, string>();
            //読み込みを行う
            string data;
            try
            {
                using (StreamReader r = new StreamReader("bot.conf"))
                {
                    data = r.ReadToEnd();
                }
            }catch(Exception e)
            {
                Console.WriteLine("[Error] No config file found. Quitting...\n"+e);
                return;
            }
            //読み込んだデータの解釈
            try
            {
                string[] temp = data.Split(char.Parse("\n"));
                foreach(string i in temp)
                {
                    if (i.Contains("="))
                    {
                        string[] temp0 = i.Split(char.Parse("="));
                        settings[temp0[0]] = temp0[1];
                    }
                }
            }catch(Exception e)
            {
                Console.WriteLine("[Error] Failed to load config file. Quitting...\n"+e);
                return;
            }
            settings["SuccessLoad"] = "[Message] Successfully loaded bot.conf";
            return;
        }

        //テスト用
        public void fakeLoad()
        {
            settings["DiscordToken"] = "ODU4MzE3NjYzMjA0OTMzNjcy.YNcYnQ.zUSgPWKJCGD0KujgOsBIxQ9XunE";
            settings["GuildID"] = "857883603262505000";
            settings["MC"] = "861504079011381279";
            settings["init"] = "Start : 起動 Stop : 停止";
        }

        //Fields
        /*
         * DiscordToken : Discordへの認証トークン
         * GuildID : Discordサーバーid
         * 
         * 
         * 
         */
    }

    class ResourceSet
    {
        public Dictionary<string, string> settings;
        public Discord::WebSocket.BaseSocketClient client;
    }
}