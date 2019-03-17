using System;
using Apache.NMS;
using Apache.NMS.Util;
using JMeter_Orchestrator.App_Code;

namespace JMeter_Orchestrator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Example connection strings:
            //    activemq:tcp://activemqhost:61616
            //    stomp:tcp://activemqhost:61613
            //    ems:tcp://tibcohost:7222
            //    msmq://localhost

            Uri connecturi = new Uri("activemq:tcp://activemq.digitaladrenalin.net:61616");

            Console.WriteLine("About to connect to " + connecturi);

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);

            using (IConnection connection = factory.CreateConnection())
            using (ISession session = connection.CreateSession())
            {

                IDestination destination = SessionUtil.GetDestination(session, "queue://JMeter");
                Console.WriteLine("Using destination: " + destination);

                // Create a consumer and producer
                using (IMessageConsumer consumer = session.CreateConsumer(destination))
                {
                    Console.WriteLine("Opening ActiveMQ Connection");
                    // Start the connection
                    connection.Start();

                    // Consume a message
                    if (!(consumer.Receive() is ITextMessage message))
                    {
                        Console.WriteLine("No message received!");
                    }
                    else
                    {
                        Console.WriteLine("Received message with ID:   " + message.NMSMessageId);
                        Console.WriteLine("Received message with text: " + message.Text);
                        Console.WriteLine("Writing To File");
                        JMXWriter.writeJMXFile(message.Text);
                        Console.WriteLine("Executing JMeter Tests");
                        JMeterExecutor.ExecuteJmeter();
                        Console.WriteLine("Process Finished");
                    }

                    Console.WriteLine("Closing ActiveMQ Connection");
                    //Stops the connection
                    connection.Stop();
                }
            }
        }
    }
}
