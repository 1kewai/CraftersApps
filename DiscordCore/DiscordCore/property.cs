using System;
using System.Collections.Generic;
using System.IO;

namespace Resource
{
    class property
    {
        //Field
        public Dictionary<string,string> settings;
        public void load()
        {
            settings = new Dictionary<string, string>();
            fakeLoad();
            return;
            //読み込み
            string readfile;
            using(StreamReader sr=new StreamReader("DiscordSetup.inf"))
            {
                readfile = sr.ReadToEnd();
            }
            //解釈
            string[] tmp0 = readfile.Split("\n");
            for(int i = 0; i < tmp0.Length; i++)
            {
                try
                {
                    string[] tmp1 = tmp0[i].Split("=");
                    settings[tmp1[0]] = tmp1[1];
                }catch(Exception e)
                {
                    //pass
                }
            }
        }

        //テスト用
        public void fakeLoad()
        {
            settings["DiscordToken"] = "NzM5NzAxOTE4Mjc1NzMxNDY3.XyeTGA.A15iGP_a2q8J6CyNNR4p-CxUr9I";
            settings["GuildID"] = "712253807685533698";
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
}