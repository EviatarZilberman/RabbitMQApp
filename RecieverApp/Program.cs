using RecieverApp;

//Test t = null;
RRR reciever = new RRR();
reciever.GetMesssge<Test>(receivedMessage =>
{
    Console.WriteLine($"Recieved Message- Name: {receivedMessage.Name}, Created at: {receivedMessage.Created}, Number: {receivedMessage.number}");
    // Here, you can perform any actions with the received message
    // For example, assign it to a variable or perform further processing
    //t = receivedMessage;
});
Console.ReadLine();