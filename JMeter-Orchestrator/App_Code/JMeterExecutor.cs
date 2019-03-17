using System;
using System.Diagnostics;

namespace JMeter_Orchestrator.App_Code
{
    public class JMeterExecutor
    {
        public static string ExecuteJmeter()
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "jmeter",
                    Arguments = $" -n -t /tmp/jmeter-tests.jmx -l /tmp/testresults.jtl",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return result;
        }
    }
}
