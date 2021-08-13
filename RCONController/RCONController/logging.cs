using System;
using System.IO;
using System.Text;
using Discord.WebSocket;

//ロギングに使うためのクラス
namespace Log
{
    class Logging
    {
        StreamWriter writer;//DiskIOのためのStreamWriterの準備
        string SessionLog;//RAM内にも現Sessionのログを保存

        //コンストラクタ
        public Logging(string filename)
        {
            //StreamWriter準備
            Encoding enc = Encoding.GetEncoding("UTF-8");
            writer = new StreamWriter(filename, true, enc);
            SessionLog = "";
            return;
        }

        //終了時処理
        public void finalize()
        {
            writer.Close();
            return;
        }

        //ログ保存
        public void log(string input)
        {
            //日時取得
            DateTime dt = DateTime.Now;
            string log = "[" + dt.Year + "/" + dt.Month + "/" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second + ":" + dt.Millisecond + "] " + input;

            //ログ記録
            Console.WriteLine(log);
            writer.WriteLine(log);
            SessionLog += log + "\n";

            return;
        }

        //メッセージ受信ログ
        public void MessageReceived(SocketMessage inputMessage)
        {
            log("[Message] " + inputMessage.Author.Username + " sent a message " + inputMessage.Content + " at " + inputMessage.Channel.Name);
            return;
        }

        //メッセージ送信ログ
        public void MessageSend(string Message)
        {
            log("[Message] " + "Sending a message : " + Message);
        }

        //メンバーアップデートログ
        public void MemberUpdate(SocketGuildUser inputUser, int i)
        {
            string[] tmp = new string[2];
            tmp[0] = "joined";
            tmp[1] = "left";
            log("[MemberUpdate] " + inputUser.Username + " " + tmp[i]);
            return;
        }

        //メッセージ更新ログ
        public void MessageUpdated(SocketMessage inputMessage)
        {
            log("[Message] " + inputMessage.Author.Username + " updated a message " + inputMessage.Content + " at " + inputMessage.Channel.Name);
        }

        //ログ取得
        public string LogReader()
        {
            return SessionLog;
        }
    }
}
