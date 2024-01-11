using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQModule
{
    public class RabbitMQConsumer : RabbitMQBase
    {
        private string? ClientProviderAppName { get; set; } = "Eviatar-RabbitMQAPP-CONSUMER";
        public RabbitMQConsumer(string? exchangeName = default, string? routingKey = default, string? queueName = default, string clientProviderAppName = default) : base(exchangeName, routingKey, queueName)
        {
            this.ClientProviderAppName = clientProviderAppName;
        }


        private static string SerializeMessage<T>(T instance)
        {
            return JsonSerializer.Serialize(instance);
        }

        public void SendMesssge<T>(T message)
        {
            try
            {
                (IModel channel, IConnection connection) = CreateChannel(this.ClientProviderAppName, this.ExchangeName, this.QueueName, this.RoutingKey);
                    byte[] messageToSend = Encoding.UTF8.GetBytes(SerializeMessage<T>(message));
                    channel.BasicPublish(this.ExchangeName, this.RoutingKey, null, messageToSend);
     
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
