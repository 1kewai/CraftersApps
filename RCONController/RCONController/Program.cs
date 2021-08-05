using System;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using CoreRCON;
using System.Net;

namespace RCONController
{
    class Program
    {
        static void Main(string[] args)
        {
            var serveraddress = IPAddress.Parse("127.0.0.1");
            ushort port = 25575;
            var serverpass = "IMadeThisServer";
            var connection = new RCON(serveraddress, port, serverpass);
            connection.ConnectAsync().GetAwaiter().GetResult();
            connection.SendCommandAsync("say RCONの接続テスト").GetAwaiter().GetResult();
        }
    }
}
