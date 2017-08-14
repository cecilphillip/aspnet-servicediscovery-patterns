using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SchoolService.Infrastructure;

namespace SchoolService
{
    public class QueueProcessor
    {
        private readonly QueueConfig _config;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _model;
        private DataStore _dataStore;

        public QueueProcessor(QueueConfig config)
        {
            _config = config;

            _connectionFactory = new ConnectionFactory
            {
                HostName = config.HostName,
                UserName = config.UserName,
                Password = config.Password
            };

            _connection = _connectionFactory.CreateConnection("QueueProcessor Connection");
            _model = _connection.CreateModel();
            _model.BasicQos(0, 1, false);

            _model.QueueDeclare(config.QueueName, false, false, true, null);

            _dataStore = new DataStore();
        }

        public void Start()
        {
            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += (model, ea) =>
            {
                var props = ea.BasicProperties;
                var replyProps = _model.CreateBasicProperties();

                var body = ea.Body;
                replyProps.CorrelationId = props.CorrelationId;

                var message = Encoding.UTF8.GetString(body);

                var result = string.Empty;

                Console.WriteLine("*** Processing Request ***");
                Console.WriteLine($"*** Process ID {Process.GetCurrentProcess().Id} ***");
                switch (message)
                {
                    case "students":
                        Console.WriteLine("Retrieving Students");
                        result = JsonConvert.SerializeObject(_dataStore.Students);
                        break;
                    case "courses":
                        Console.WriteLine("Retrieving Courses");
                        result = JsonConvert.SerializeObject(_dataStore.Courses);
                        break;
                    default:
                        Console.WriteLine($"Could Not Process: {message}");
                        break;
                }

                var resultBytes = Encoding.UTF8.GetBytes(result);
                _model.BasicPublish("", props.ReplyTo, replyProps, resultBytes);
                _model.BasicAck(ea.DeliveryTag, false);
            };

            _model.BasicConsume(_config.QueueName, false, consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
            
            _model.Dispose();
            _connection.Dispose();
        }
    }
}