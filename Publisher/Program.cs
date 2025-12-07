using Publisher;
using RabbitMQ.Client;
using System.Text;
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

for (int i = 0; i < 10; i++)
{
    var order = new OrderPayedMessage(Guid.NewGuid(), DateTime.UtcNow);
    var message = JsonSerializer.Serialize(order);
    var body = Encoding.UTF8.GetBytes(message);
    var properties = new BasicProperties
    {
        Persistent = true
    };
    await channel.BasicPublishAsync(
        exchange: "",
        routingKey: "message-queue",
        mandatory: true,
        basicProperties: properties,
        body: body);
    Console.WriteLine($" [x] Sent {order}");

    await Task.Delay(1000);
}
