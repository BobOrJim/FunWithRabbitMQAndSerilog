using System;
using RabbitMQ.Client;
using System.Text;
using System.Threading;

namespace ConsoleProducerWithAck
{
    class Program
    {
        public static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                int messageCounter = 0;
                while (true)
                {

                    channel.QueueDeclare(queue: "task_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);

                    //var message = GetMessage(args);
                    string message = $"Hello World!{messageCounter}";

                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "", routingKey: "task_queue", basicProperties: properties, body: body);

                    Console.WriteLine($" [x] Sent {messageCounter}", message);
                    Thread.Sleep(300);

                    messageCounter++;
                }
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args)
        {
            return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
        }
    }
}
