using System;
using System.Diagnostics;
using Apache.NMS;

namespace JMeter_Orchestrator.App_Code
{
    public class JMeterExecutor
    {
        public static string ExecuteJmeter(string Folder)
        {
            try
            {
                Console.WriteLine("Executing: jmeter -n -t " + Folder + "jmeter-tests.jmx -l " + Folder + "testresults.jtl");
                var process = new Process()
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "jmeter",
                        Arguments = $" -n -t " + Folder + "jmeter-tests.jmx -l " + Folder +  "testresults.jtl",
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
