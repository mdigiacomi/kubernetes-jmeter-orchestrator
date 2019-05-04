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

            Uri connecturi = new Uri(Environment.GetEnvironmentVariable("ActiveMQ-Server"));

            Console.WriteLine("About to connect to " + connecturi);

            bool controlloop = true;

            // NOTE: ensure the nmsprovider-activemq.config file exists in the executable folder.
            IConnectionFactory factory = new NMSConnectionFactory(connecturi);

            using (IConnection connection = factory.CreateConnection(Environment.GetEnvironmentVariable("ActiveMQ-UserName"), Environment.GetEnvironmentVariable("ActiveMQ-Password")))
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
                        if (!(consumer.Receive() is ITextMessage jmxmessage))
                        {
                            Console.WriteLine("No message received!");
                        }
                        else
                        {
                            string Folder = "/JMeter/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID + "/" + Environment.MachineName + "-" + DateTime.Now.ToString("MMddyy-HHmmss") + "/";

                            //Creating Folder for Test Results
                            System.IO.Directory.CreateDirectory(Folder);

                            Console.WriteLine("Received message with ID:   " + jmxmessage.NMSMessageId);
                            Console.WriteLine("Writing To File");
                            JMXWriter.writeJMXFile(jmxmessage, Folder);
                            Console.WriteLine("Executing JMeter Tests");
                            JMeterExecutor.ExecuteJmeter(Folder);
                            Console.WriteLine("Process Finished");
                            jmxmessage.Acknowledge();
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
