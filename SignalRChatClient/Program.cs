using Microsoft.AspNetCore.SignalR.Client;
using ProductsCore.Models;
using System;
using System.Threading.Tasks;

namespace SignalRChatClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await StartChatAsync();
        }

        static async Task StartChatAsync()
        {
            HubConnection connection
               = new HubConnectionBuilder()
               .WithUrl("https://localhost:5001/chat")
               .Build();
            ConsoleColor currentColor = ConsoleColor.Yellow;

            connection.On<ChatMessage>("ReceiveMessage",
                (chatMessage) =>
                {
                    var temp = currentColor;
                    Console.ForegroundColor = chatMessage.MessageColor;

                    var newMessage = $"{chatMessage.Sender}: {chatMessage.Text}";
                    Console.WriteLine(newMessage);

                    Console.ForegroundColor = temp;
                });

            connection.On<ConsoleColor>("ColorChanged",
                (newColor) =>
                {
                    currentColor = newColor;
                    Console.ForegroundColor = currentColor;
                    Console.WriteLine($"Color changed to {newColor}");
                });

            await connection.StartAsync();

            string message;
            do
            {
                message = Console.ReadLine();
                await connection.InvokeAsync("SendMessage", message);
            } while (!string.IsNullOrEmpty(message));
        }
    }
}
