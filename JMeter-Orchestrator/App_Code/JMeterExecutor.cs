using System;
using System.Diagnostics;
using Apache.NMS;

namespace JMeter_Orchestrator.App_Code
{
    public class JMeterExecutor
    {
        public static string ExecuteJmeter(ITextMessage jmxmessage)
        {
            try
            {
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "jmeter",
                        Arguments = $" -n -t /JMeter/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID + "/" + Environment.MachineName + "-" + DateTime.Now.ToString("MMddyy-HHmmss") + "/jmeter-tests.jmx -l /tmp/JMeter/" + jmxmessage.Properties["TeamID"] + "/" + jmxmessage.Properties["Application"] + "/" + jmxmessage.NMSCorrelationID + "/" + Environment.MachineName + "-" + DateTime.Now.ToString("MMddyy-HHmmss") +  "/testresults.jtl",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                string result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine(result);
                return result;
            }
            catch(Exception error)
            {
                Console.WriteLine(error.Message);
                return "Error - " + error.Message;
            }
        }
    }
}
