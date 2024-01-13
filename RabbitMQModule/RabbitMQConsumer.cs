using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitMQModule
{
    public class RabbitMQConsumer<T> : RabbitMQBase
    {
        private string? ClientProviderAppName { get; set; } = "Eviatar-RabbitMQAPP-CONSUMER";
        private List<T> UnSentMessages { get; set; } = new List<T>();

        public RabbitMQConsumer(string? exchangeName = default, string?
            routingKey = default, string? queueName = default, 
            string clientProviderAppName = default) 
            : base(exchangeName, routingKey, queueName)
        {
            this.ClientProviderAppName = clientProviderAppName;
        }


        private static string SerializeMessage<T>(T instance)
        {
            return JsonSerializer.Serialize(instance);
        }

        private bool SendMessage<T>(T message)
        {
            try
            {
                (IModel channel, IConnection connection) = CreateChannel(this.ClientProviderAppName, this.ExchangeName, this.QueueName, this.RoutingKey);
                byte[] messageToSend = Encoding.UTF8.GetBytes(SerializeMessage<T>(message));
                channel.BasicPublish(this.ExchangeName, this.RoutingKey, null, messageToSend);

                channel.Close();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return false;
            }
        }

        public bool SendMessage(T message, int times, int interval, bool exponentialInterval)
        {
            int internalTimes = 1, internalInterval = 2, powInterval = 0;
            if (times > 0 && times < 5)
            {
                internalTimes = times;
            }

            if (interval > 0)
            {
                internalInterval = interval;
            }
            bool sentMessage = false;
            for (int i = 0; i < internalTimes; i++)
            {
                sentMessage = this.SendMessage(message);
                if (sentMessage)
                {
                    return true;
                }
                else
                {
                    if (!exponentialInterval)
                    {
                        Thread.Sleep(interval);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            Thread.Sleep(internalInterval);
                        }
                        else
                        {
                            Thread.Sleep((int)Math.Pow(internalInterval, internalInterval + powInterval));
                            powInterval++;
                        }
                    }
                }
            }
            if (!sentMessage)
            {
                this.UnSentMessages.Add(message);
            }
                return false;
        }

        public void ResendAllMessages()
        {
            if (!this.IsSuccess())
            {
                for(int i = 0; i < this.UnSentMessages.Count; i++)
                {
                    if (this.SendMessage(this.UnSentMessages[i]))
                    {
                        this.UnSentMessages.RemoveAt(i);
                    }
                }
            }
        }

        public bool IsSuccess()
        {
            return this.UnSentMessages.Count == 0 ? true : false;
        }
    }
}
