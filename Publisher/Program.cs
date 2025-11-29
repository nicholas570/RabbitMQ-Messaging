using RabbitMQ.Client;

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
    var message = $"{DateTime.UtcNow} - {Guid.NewGuid()}";
    var body = System.Text.Encoding.UTF8.GetBytes(message);
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
    Console.WriteLine($" [x] Sent {message}");

    await Task.Delay(1000);
}
