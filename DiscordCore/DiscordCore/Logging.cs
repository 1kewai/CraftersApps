using System;
using System.IO;
using System.Text;
using Discord.WebSocket;

namespace Log
{
    class Logging
    {
        StreamWriter writer;
        string SessionLog;
        public Logging(string filename)
        {
            Encoding enc = Encoding.GetEncoding("UTF-8");
            writer = new StreamWriter(filename, true, enc);
            SessionLog = "";
            return;
        }

        public void finalize()
        {
            writer.Close();
            return;
        }

        public void log(string input)
        {
            DateTime dt = DateTime.Now;
            string log = "[" + dt.Year + "/" + dt.Month + "/" + dt.Day + " " + dt.Hour + ":" + dt.Minute + ":" + dt.Second + ":" + dt.Millisecond + "] " + input;
            Console.WriteLine(log);
            writer.WriteLine(log);
            SessionLog += log + "\n";
            return;
        }

        public void MessageReceived(SocketMessage inputMessage)
        {
            log("[Message] " + inputMessage.Author.Username + " sent a message\n" + inputMessage.Content + "\nat " + inputMessage.Channel.Name);
            return;
        }

        public void MemberUpdate(SocketGuildUser inputUser, int i)
        {
            string[] tmp = new string[2];
            tmp[0] = "joined";
            tmp[1] = "left";
            log("[MemberUpdate] " + inputUser.Username + " " + tmp[i]);
            return;
        }

        public void MessageUpdated(SocketMessage inputMessage)
        {
            log("[Message] " + inputMessage.Author.Username + " updated a message\n" + inputMessage.Content + "\nat " + inputMessage.Channel.Name);
        }

        public string LogReader()
        {
            return SessionLog;
        }
    }
}
