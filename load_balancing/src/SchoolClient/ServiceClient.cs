using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using SchoolClient.Models;

namespace SchoolClient
{
    public class ServiceClient
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private Subscription _subscription;
        private IModel _model;
        private string _sendQueue;
        private string _replyQueueName;

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

            _replyQueueName = _model.QueueDeclare().QueueName;
            _subscription = new Subscription(_model, _replyQueueName, true);

        }

        public IEnumerable<Student> GetStudents() => SendRequest<IEnumerable<Student>>("students");

        public IEnumerable<Course> GetCourses() => SendRequest<IEnumerable<Course>>("courses");

        private T SendRequest<T>(string message)
        {
            var corrId = Guid.NewGuid().ToString();

            var props = _model.CreateBasicProperties();
            props.ReplyTo = _replyQueueName;
            props.CorrelationId = corrId;

            var messageBytes = Encoding.UTF8.GetBytes(message);
            _model.BasicPublish("", _sendQueue, props, messageBytes);

            while (true)
            {
                var delivery = _subscription.Next();
                if (delivery.BasicProperties.CorrelationId != corrId) continue;

                var resultString = Encoding.UTF8.GetString(delivery.Body);
                var result = JsonConvert.DeserializeObject<T>(resultString);
                return result;
            }
        }
    }
}