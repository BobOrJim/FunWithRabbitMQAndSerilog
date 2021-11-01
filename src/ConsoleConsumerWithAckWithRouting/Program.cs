using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;

namespace ConsoleConsumerWithAckWithRouting
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "ThievesDen", type: "direct");
                var queueName = channel.QueueDeclare().QueueName; //

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
                        Thread.Sleep(1000);
                    };
                    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

                }
            }
        }
    }
}
