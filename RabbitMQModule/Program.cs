using RabbitMQModule;

 RabbitMQConsumer consumer = new RabbitMQConsumer("name", "routing", null, "app");
/*consumer.SendMesssge<Test>(new Test());
*//*Test t = null;
RabbitMQReciever.GetMesssge<Test>(receivedMessage =>
{
    Console.WriteLine($"Name: {receivedMessage.Name}, Created at: {receivedMessage.Created}");
    // Here, you can perform any actions with the received message
    // For example, assign it to a variable or perform further processing
    t = receivedMessage;
});*/
Console.ReadLine();
