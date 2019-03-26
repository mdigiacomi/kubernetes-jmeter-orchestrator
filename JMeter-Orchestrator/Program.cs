using System;
using System.Threading;
using System.Threading.Tasks;
using Apache.NMS;
using Apache.NMS.Util;
using JMeter_Orchestrator.App_Code;

namespace JMeter_Orchestrator
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Console.WriteLine("ProcessExit!");
                cts.Cancel();
            };

            Uri connecturi = new Uri(Environment.GetEnvironmentVariable("ActiveMQ"));

            Console.WriteLine("About to connect to " + connecturi);

            bool controlloop = true;

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
                    while (controlloop)
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
                            JMXWriter.writeJMXFile(message);
                            Console.WriteLine("Executing JMeter Tests");
                            JMeterExecutor.ExecuteJmeter(message);
                            Console.WriteLine("Process Finished");
                            message.Acknowledge();
                        }

                        Console.WriteLine("Closing ActiveMQ Connection");
                        //Stops the connection
                        connection.Stop();
                    }
                }
            }
            return 0;
        }
    }
}
