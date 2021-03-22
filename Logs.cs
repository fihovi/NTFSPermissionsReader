using System;
using System.Collections.Generic;
using System.IO;

namespace NTFSPermissions
{
    internal class Logs
    {
        private string permissionsLogFileLocation;
        private string auditLogFileLocation;

        public string GenerateFileName(string exportLocation, string type, string extension, string logfileName)
        {
            var dt = DateTime.Now;
            var logfileLocation = exportLocation + @"\" + dt.ToString("yy-MM-dd_HH-mm-ss-ffffff") + "-" + type + "." + extension;
            if (logfileName != "") { logfileLocation = exportLocation + @"\" + logfileName + dt.ToString("MM-dd_HH-mm-ss") + "-" + type + "." + extension; }
            Console.WriteLine(@"Created log file: " + logfileLocation);
            switch (type)
            {
                case "permissions":
                    permissionsLogFileLocation = logfileLocation;
                    break;
                case "audit":
                    auditLogFileLocation = logfileLocation;
                    break;
            }
            return logfileLocation;
        }

        // Add single lines to the logfile
        public void AddToPermissionsLogFile(string log)
        {
            try
            {
                using var sw = File.AppendText(permissionsLogFileLocation);
                sw.WriteLine(log);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error writing to log file: " + permissionsLogFileLocation + @" \n" + e);
            }
        }

        // Add multiple lines to the logfile
        public void AddListToPermissionsLogFile(IEnumerable<string> logs)
        {
            try
            {
                using var sw = File.AppendText(permissionsLogFileLocation);
                foreach (var l in logs)
                {
                    sw.WriteLine(l);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($@"Error writing to log file: {permissionsLogFileLocation} \n{e}");
            }
        }

        // Add single line to logfile
        public void AddToAuditLogFile(string log)
        {
            try
            {
                using var sw = File.AppendText(auditLogFileLocation);
                sw.WriteLine(log);
            }
            catch (Exception e)
            {
                Console.WriteLine(@"Error writing to audit file: " + auditLogFileLocation + " \n" + e);
            }
        }

    }
}
