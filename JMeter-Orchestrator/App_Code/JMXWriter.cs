using System;
using System.IO;
using Apache.NMS;

namespace JMeter_Orchestrator.App_Code
{
    public class JMXWriter
    {
        public static void writeJMXFile(ITextMessage jmxmessage)
        {
            System.IO.Directory.CreateDirectory("/JMeter/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID + "/" + Environment.MachineName + "-" + DateTime.Now.ToString("MMddyy-HHmmss") + "/");
            using (var writer = File.CreateText("/JMeter/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID + "/" + Environment.MachineName + "-" + DateTime.Now.ToString("MMddyy-HHmmss") + "/jmeter-tests.jmx"))
            {
                writer.Write(jmxmessage.Text); //or .Write(), if you wish
            }
        }
    }
}
