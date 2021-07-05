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

        //Fields
        /*
         * DiscordToken : Discordへの認証トークン
         * GuildID : Discordサーバーid
         * MC : Minecraft サーバー起動用チャットルームのid
         * ServerStatusID : サーバーステータス表示用チャットルームのid
         * ResourceGroup : インスタンスのリソースグループ
         * MCServername
         */
    }

    class ResourceSet
    {
        public Dictionary<string, string> settings;
        public Discord::WebSocket.BaseSocketClient client;
    }
}