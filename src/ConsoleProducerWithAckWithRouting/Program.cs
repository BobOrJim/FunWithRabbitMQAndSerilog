using System;
using System.Linq;
using RabbitMQ.Client;
using System.Text;
using System.Threading;
using System.IO;
using Serilog;

namespace ConsoleProducerWithAckWithRouting
{
    class Program
    {
        public static void Main()
        {


            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
            var myDirectory = Directory.GetParent(solutionDirectory.ToString()) + @"\Logs\MySolutionLog.txt";
            Log.Logger = new LoggerConfiguration()
                //.WriteTo.File(myDirectory, rollingInterval: RollingInterval.Day, shared: true) // stops append from working?
                .WriteTo.File(myDirectory, shared: true)
                .CreateLogger();
            Log.Information($" producer starting ");

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
                    Log.Information($" A message is sent to {destination}. With the text: {message}");
                    Thread.Sleep(100);
                    messageCounter++;
                }
            }
        }
    }
}
