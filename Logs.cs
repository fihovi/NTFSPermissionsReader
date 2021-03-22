﻿using System;
using System.Collections.Generic;
using System.IO;

namespace NTFSPermissions
{
    class Logs
    {
        private string permissionsLogFileLocation;
        private string auditLogFileLocation;

        public string generateFileName(string exportLocation, string type, string extension, string logfileName)
        {
            DateTime dt = DateTime.Now;
            string logfileLocation = exportLocation + @"\" + dt.ToString("yy-MM-dd_HH-mm-ss-ffffff") + "-" + type + "." + extension;
            if (logfileName != "") { logfileLocation = exportLocation + @"\" + logfileName + dt.ToString("MM-dd_HH-mm-ss") + "-" + type + "." + extension; }
            Console.WriteLine("Created log file: " + logfileLocation);
            if (type == "permissions") { permissionsLogFileLocation = logfileLocation; }
            else if (type == "audit") { auditLogFileLocation = logfileLocation; }
            return logfileLocation;
        }

        // Add single lines to the logfile
        public void addToPermissionsLogFile(string log)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(permissionsLogFileLocation))
                {
                    sw.WriteLine(log);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to log file: " + permissionsLogFileLocation + " \n" + e);
            }
        }

        // Add multiple lines to the logfile
        public void addListToPermissionsLogFile(List<string> logs)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(permissionsLogFileLocation))
                {
                    foreach (string l in logs)
                    {
                        sw.WriteLine(l);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to log file: " + permissionsLogFileLocation + " \n" + e);
            }
        }

        // Add single line to logfile
        public void addToAuditLogFile(string log)
        {
            try
            {
                using (StreamWriter sw = File.AppendText(auditLogFileLocation))
                {
                    sw.WriteLine(log);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error writing to audit file: " + auditLogFileLocation + " \n" + e);
            }
        }

    }
}
