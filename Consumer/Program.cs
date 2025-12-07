using Publisher;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = await factory.CreateConnectionAsync();
using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "message-queue",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (sender, eventArgs) =>
{
    var body = eventArgs.Body.ToArray();
    var order = JsonSerializer.Deserialize<OrderPayedMessage>(body);
    Console.WriteLine($" [x] Received {order.Id}");
    await Task.Delay(500);
    await channel.BasicAckAsync(deliveryTag: eventArgs.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(
    queue: "message-queue",
    autoAck: false,
    consumer: consumer);

Console.ReadLine();
