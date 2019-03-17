using System;
using System.IO;

namespace JMeter_Orchestrator.App_Code
{
    public class JMXWriter
    {
        public static void writeJMXFile(string jmxmessage)
        {
            using (var writer = File.CreateText("/tmp/jmeter-tests.jmx"))
            {
                writer.Write(jmxmessage); //or .Write(), if you wish
            }
        }
    }
}
