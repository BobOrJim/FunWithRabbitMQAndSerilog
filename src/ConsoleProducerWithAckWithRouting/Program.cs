using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

namespace ConsoleProducerWithAckWithRouting
{
    class Program
    {
        public static void Main()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                int messageCounter = 0;
                channel.ExchangeDeclare(exchange: "ThievesDen", type: "direct");

                while (true)
                {
                    string[] destination = { "ReservoirDog.MrBrown", "ReservoirDog.MrBlue", "ReservoirDog.MrBlonde" };
                    string message = $"MrPink is a COP!. messageId = {messageCounter}";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "ThievesDen", routingKey: destination[messageCounter % 3], basicProperties: null, body: body);
                    Console.WriteLine($"Sent message with id {messageCounter} to {destination[messageCounter % 3]}", destination[messageCounter % 3], message);
                    Thread.Sleep(100);
                    messageCounter++;
                }
            }
        }
    }
}
