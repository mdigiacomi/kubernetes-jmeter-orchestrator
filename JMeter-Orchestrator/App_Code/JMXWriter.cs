using System;
using System.IO;
using Apache.NMS;

namespace JMeter_Orchestrator.App_Code
{
    public class JMXWriter
    {
        public static void writeJMXFile(ITextMessage jmxmessage, string Folder)
        {
            using (var writer = File.CreateText(Folder + "jmeter-tests.jmx"))
            {
                writer.Write(jmxmessage.Text); //or .Write(), if you wish
            }
        }
    }
}
