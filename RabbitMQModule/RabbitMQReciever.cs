using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RabbitMQModule
{
    public class RabbitMQReciever : RabbitMQBase
    {
        private string? ClientProviderAppName { get; set; } = "Eviatar-RabbitMQAPP-RECIEVER";
        public RabbitMQReciever(string? exchangeName = default, string? routingKey = default, string? queueName = default, string? clientProviderAppName = default) : base(exchangeName, routingKey, queueName)
        {
            this.ClientProviderAppName = clientProviderAppName;
        }

        public RabbitMQReciever() { }

        private static T DeserializeMessage<T>(string message)
        {
            return JsonSerializer.Deserialize<T>(message);
        }

        public void GetMesssge<T>(Action<T> messageProcessor)
        {
            T? result = default;
            try
            {
                (IModel channel, IConnection connection) = CreateChannel(this.ClientProviderAppName, this.ExchangeName, this.QueueName, this.RoutingKey);
                channel.BasicQos(0, 1, false);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, args) =>
                {
                    var body = args.Body.ToArray();

                    T deserializedMessage = DeserializeMessage<T>(Encoding.UTF8.GetString(body));
                    result = deserializedMessage;
                    messageProcessor?.Invoke(deserializedMessage);
                    channel.BasicAck(args.DeliveryTag, false);
                };

                string consumerTag = channel.BasicConsume(this.QueueName, false, consumer);

                Console.ReadLine();

                channel.Close();
                connection.Close();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
            }
        }
    }
}
