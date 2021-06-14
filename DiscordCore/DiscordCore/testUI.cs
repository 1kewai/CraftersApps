using System;
using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;

namespace UI
{
    class TestUI : DiscordChatUI
    {
        public TestUI(Discord::ITextChannel inputChannel, Log::Logging logging, string initialmessage) : base(inputChannel, logging, initialmessage)
        {

        }
        public override async Task MessageReceived(Discord::WebSocket.SocketMessage inputMessage)
        {
            Console.WriteLine("Working0");
            if (!inputMessage.Author.IsBot)
            {
                Display = inputMessage.Content;
                logging.log("Mirror working.");
                await refresh();
            }
        }
    }
}
