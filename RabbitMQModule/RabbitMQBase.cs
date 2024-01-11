using RabbitMQ.Client;

namespace RabbitMQModule
{
    public class RabbitMQBase
    {
        private static string UserName { get; } = "guest";
        private static string Password { get; } = "guest";
        private static string Host { get; } = "localhost";
        private static int Port { get; } = 5672;
        protected string? ExchangeName { get; set; } = "DemoExchangeName";
        protected string? RoutingKey { get; set; } = "DemoRoutingKey";
        protected string? QueueName { get; set; } = "DemoQueueName";

        public RabbitMQBase(string? exchangeName = default, string? routingKey = default, string? queueName = default) 
        { 
            this.ExchangeName = exchangeName;
            this.RoutingKey = routingKey;
            this.QueueName = queueName;
        }

        public RabbitMQBase() { }

        protected static Tuple<IModel, IConnection> CreateChannel (string clientProviderAppName,
            string exchangeName, string queueName, string routingKey)
        {
            ConnectionFactory connectionFactory = new();
            connectionFactory.Uri = new Uri($"amqp://{UserName}:{Password}@{Host}:{Port}");
            connectionFactory.ClientProvidedName = clientProviderAppName;
            IConnection connection = connectionFactory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            channel.QueueDeclare(queueName, false, false, false, null);
            channel.QueueBind(queueName, exchangeName, routingKey, null);
            return Tuple.Create(channel, connection);
        }
    }
}
