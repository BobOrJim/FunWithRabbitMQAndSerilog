using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using Serilog;
using System.IO;

namespace ConsoleConsumerWithAckWithRouting
{
    class Program
    {
        public static void Main(string[] args)
        {
            var solutionDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.Parent;
            var myDirectory = Directory.GetParent(solutionDirectory.ToString()) + @"\Logs\MySolutionLog.txt";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                //.WriteTo.File(myDirectory, rollingInterval: RollingInterval.Day, shared: true) // stops append from working?
                .WriteTo.File(myDirectory, shared: true)
                .CreateLogger();
            Log.Information($" consumer starting ");


            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "ThievesDen", type: "direct");
                var queueName = channel.QueueDeclare().QueueName;

                string[] possibleRecievers = { "ReservoirDog.MrBrown", "ReservoirDog.MrBlue", "ReservoirDog.MrBlonde" };
                Random rnd = new Random();
                string thisReciever = possibleRecievers[rnd.Next(0, 3)];

                while (true)
                {
                    Thread.Sleep(1000);
                    channel.QueueBind(queue: queueName, exchange: "ThievesDen", routingKey: thisReciever);
                    var consumer = new EventingBasicConsumer(channel);
                    
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        var routingKey = ea.RoutingKey;
                        Console.WriteLine($" {thisReciever} picked up a message with address: {routingKey}. The message is: {message}");
                        Log.Information($" {thisReciever} read a message with the text: {message}");
                        Thread.Sleep(1000);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

                    };
                    channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                }
            }
        }
    }
}
