using System;
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

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();
            _model.BasicQos(0, 1, false);

            _model.QueueDeclare(config.QueueName, true, false, true, null);

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

                switch (message)
                {
                    case "students":
                        result = JsonConvert.SerializeObject(_dataStore.Students);
                        break;
                    case "courses":
                        result = JsonConvert.SerializeObject(_dataStore.Courses);
                        break;
                    default:
                        Console.WriteLine($"Cannot process: {message}");
                        break;
                }

                var resultBytes = Encoding.UTF8.GetBytes(result);
                _model.BasicPublish("", props.ReplyTo, replyProps, resultBytes);
                _model.BasicAck(ea.DeliveryTag, false);
            };

            _model.BasicConsume(_config.QueueName, false, consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}