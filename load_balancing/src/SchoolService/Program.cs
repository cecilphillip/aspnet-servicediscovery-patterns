using System;

namespace SchoolService
{
    public class Program
    {
        public static void Main(string[] args)
        {  var config = new QueueConfig
            {
               HostName = "10.211.55.2",
               UserName = "admin",
               Password = "admin",
               QueueName = "api.school"
            };

            DisplayRabbitSettings(config);
            Console.WriteLine("Starting School Service Queue Processor....");
            Console.WriteLine();

          
            var processor = new QueueProcessor(config);
            processor.Start();            
        }

        private static void DisplayRabbitSettings(QueueConfig config)
        {
            Console.WriteLine("*********************");
            Console.WriteLine("Host: {0}", config.HostName);
            Console.WriteLine("Username: {0}", config.UserName);
            Console.WriteLine("Password: {0}", config.Password);
            Console.WriteLine("QueueName: {0}", config.QueueName);
            Console.WriteLine("*********************");
            Console.WriteLine();
        }
    }
}
