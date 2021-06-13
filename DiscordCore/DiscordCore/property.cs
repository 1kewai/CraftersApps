using System;
using System.Collections.Generic;
using System.IO;

namespace Resource
{
    class property
    {
        //Field
        Dictionary<string,string> settings;
        public void load()
        {
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

        }
    }
}
