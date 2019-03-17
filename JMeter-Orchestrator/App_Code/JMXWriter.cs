using System;
using System.IO;
using Apache.NMS;

namespace JMeter_Orchestrator.App_Code
{
    public class JMXWriter
    {
        public static void writeJMXFile(ITextMessage jmxmessage)
        {
            System.IO.Directory.CreateDirectory("/tmp/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID + "/");
            using (var writer = File.CreateText("/tmp/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID +  "/jmeter-tests.jmx"))
            {
                writer.Write(jmxmessage.Text); //or .Write(), if you wish
            }
        }
    }
}
