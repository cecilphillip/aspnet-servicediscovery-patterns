using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SchoolClient.Models;

namespace SchoolClient
{
    public class ServiceClient
    {
        private ConnectionFactory _connectionFactory;
        private QueueingBasicConsumer _consumer;
        private IConnection _connection;
        private IModel _model;

        private string _sendQueue;
        private string replyQueueName;

        public ServiceClient(IConfigurationRoot configuration)
        {
            _connectionFactory = new ConnectionFactory
            {
                HostName = configuration.GetSection("rabbitmq-settings")["hostName"],
                UserName = configuration.GetSection("rabbitmq-settings")["userName"],
                Password = configuration.GetSection("rabbitmq-settings")["password"]
            };

            _connection = _connectionFactory.CreateConnection();
            _model = _connection.CreateModel();

            _sendQueue = configuration.GetSection("rabbitmq-settings")["sendQueue"];
            _model.QueueDeclare(_sendQueue, false, false, true, null);

            replyQueueName = _model.QueueDeclare().QueueName;

            _consumer = new QueueingBasicConsumer(_model);
            _model.BasicConsume(replyQueueName, true, _consumer);
        }

        public IEnumerable<Student> GetStudents() => SendRequest<IEnumerable<Student>>("students");

        public IEnumerable<Course> GetCourses() => SendRequest<IEnumerable<Course>>("courses");

        private T SendRequest<T>(string message)
        {
            var corrId = Guid.NewGuid().ToString();

            var props = _model.CreateBasicProperties();
            props.ReplyTo = replyQueueName;
            props.CorrelationId = corrId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            _model.BasicPublish("", _sendQueue, props, messageBytes);

            while (true)
            {
                var ea = (BasicDeliverEventArgs)_consumer.Queue.Dequeue();
                if (ea.BasicProperties.CorrelationId == corrId)
                {
                    var resultString = Encoding.UTF8.GetString(ea.Body);
                    var result = JsonConvert.DeserializeObject<T>(resultString);
                    return result;
                }
            }
        }
    }
}