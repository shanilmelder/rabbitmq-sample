using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories
{
    public class MessagingQue : IMessagingQue
    {
        //private readonly IConnection _connection;
        //private readonly IModel _channel;
        //public MessagingQue()
        //{
        //    var factory = new ConnectionFactory() { HostName = "localhost" };
        //    try
        //    {
        //        _connection = factory.CreateConnection();
        //        _channel = _connection.CreateModel();

        //        _channel.ExchangeDeclare(exchange: "fund_transfer", type: ExchangeType.Fanout);

        //        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        //        Console.WriteLine("--> Connected to MessageBus");

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}");
        //    }
        //}

        //public void SendMessage(string exchange, string message)
        //{
        //    var body = Encoding.UTF8.GetBytes(message);

        //    _channel.BasicPublish(exchange: exchange,
        //                    routingKey: "",
        //                    basicProperties: null,
        //                    body: body);
        //    Console.WriteLine($"--> We have sent {message}");
        //}

        //public void SendMessage(string queName, string message)
        //{
        //    try
        //    {
        //        var factory = new ConnectionFactory() { HostName = "localhost" };
        //        using (var connection = factory.CreateConnection())
        //        {
        //            using (var channel = connection.CreateModel())
        //            {
        //                channel.QueueDeclare(queue: queName,
        //                             durable: false,
        //                             exclusive: false,
        //                             autoDelete: false,
        //                             arguments: null);

        //                var body = Encoding.UTF8.GetBytes(message);

        //                channel.BasicPublish(exchange: "",
        //                                     routingKey: queName,
        //                                     basicProperties: null,
        //                                     body: body);
        //                Console.WriteLine(" [x] Sent {0}", message);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //}

        public string SendMessageWithResponse(string queName, string message)
        {
            try
            {
                BlockingCollection<string> respQueue = new BlockingCollection<string>();
                var factory = new ConnectionFactory() { HostName = "localhost" };

                IConnection connection = factory.CreateConnection();
                IModel channel = connection.CreateModel();  
                channel.QueueDeclare(queName);
                EventingBasicConsumer consumer = new EventingBasicConsumer(channel);

                IBasicProperties props = channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = queName;

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        respQueue.Add(response);
                    }
                };

                channel.BasicConsume(
                    consumer: consumer,
                    queue: queName,
                    autoAck: true);

                var messageBytes = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(
                    exchange: "",
                    routingKey: "rpc_" + queName,
                    basicProperties: props,
                    body: messageBytes);

                connection.Close();

                return respQueue.Take();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        //private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        //{
        //    Console.WriteLine("--> RabbitMQ Connection Shutdown");
        //}

        //public void Dispose()
        //{
        //    Console.WriteLine("MessageBus Disposed");
        //    if (_channel.IsOpen)
        //    {
        //        _channel.Close();
        //        _connection.Close();
        //    }
        //}
    }
}
