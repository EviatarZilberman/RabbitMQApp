using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQModule;
using RecieverApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RecieverApp
{
    public class RRR : RabbitMQBase
    {
        public static string ClientProviderAppName { get; } = "Eviatar-RabbitMQAPP-RECIEVER";

        private static T DeserializeMessage<T>(string message)
        {
            return JsonSerializer.Deserialize<T>(message);
        }

        public void GetMesssge<T>(Action<T> messageProcessor) where T : Test
        {
            T result = default;
            try
            {
                /*ConnectionFactory connectionFactory = new();
                connectionFactory.Uri = new Uri($"amqp://{UserName}:{Password}@{Host}:{Port}");
                connectionFactory.ClientProvidedName = ClientProviderAppName;
                IConnection connection = connectionFactory.CreateConnection();
                IModel channel = connection.CreateModel();*/
                (IModel channel, IConnection connection) = CreateChannel(ClientProviderAppName, this.ExchangeName, this.QueueName, this.RoutingKey);
                /*channel.ExchangeDeclare(this.ExchangeName, ExchangeType.Direct);
                channel.QueueDeclare(this.QueueName, false, false, false, null);
                channel.QueueBind(this.QueueName, this.ExchangeName, this.RoutingKey, null);*/
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

                Console.WriteLine(ex.Message);
            }
        }
    }
}
