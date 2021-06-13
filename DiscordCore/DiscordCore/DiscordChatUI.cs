using System.Threading.Tasks;
using Discord = Discord;
using Log = Log;


namespace UI
{
    class DiscordChatUI
    {
        //Field
        string Display;
        Discord::ITextChannel channel;

        public DiscordChatUI(Discord::ITextChannel inputChannel,  Log::Logging logging, string initialmessage)
        {
            channel = inputChannel;
            Display = initialmessage;
            logging.MessageSend(Display);
            refresh().GetAwaiter().GetResult();
        }

        public async Task refresh()
        {
            await channel.SendMessageAsync(Display);
        }
    }
}