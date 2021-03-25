using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.IO;
using System.Security.AccessControl; // For DirectorySecurity class
using System.Security.Principal; // For NTAccount

namespace NTFSPermissions
{
    class Audit
    {
        private readonly BackgroundWorker bgWorker_Audit;
        private string auditLocation;
        private string exportLocation;
        private bool showSystemAccounts;
        private bool summary;
        private bool auditEventLogs;
        private Logs log;
        private int foldersCounted = 0;
        private int foldersScanned = 0;
        private int foldersAccessErrors = 0;
        private int numLevelsDeepSetting = 3;
        private string currentFolder = "";
        private string csvFilename;

        // Entry point for the audit - manages the various configurations surrounding the audit
        public Audit(string _auditLocation, string _exportLocation, string numLevelsDeep, bool _auditEventLogs,
            bool _showSystemAccounts, string _csvFilename, bool summary)
        {
            this.auditLocation = _auditLocation;
            this.exportLocation = _exportLocation;
            this.auditEventLogs = _auditEventLogs;
            this.csvFilename = _csvFilename;
            this.showSystemAccounts = _showSystemAccounts;
            this.summary = summary;

            // Check how many levels deep the application needs to scan and audit
            CalculateHowManyLevelsDeepToScan(numLevelsDeep);

            // Configure logging
            log = new Logs();
            log.GenerateFileName(auditLocation, exportLocation, "permissions", "csv", _csvFilename);

            // Background worker for scanning and reporting
            bgWorker_Audit = new BackgroundWorker {WorkerReportsProgress = true};
            bgWorker_Audit.DoWork += new DoWorkEventHandler(bgWorker_Scan_DoWork);
            bgWorker_Audit.RunWorkerCompleted += new RunWorkerCompletedEventHandler((sender, e) =>
                bgWorker_Scan_RunWorkerCompleted(sender, e, this.summary));
            bgWorker_Audit.WorkerReportsProgress = true;
            bgWorker_Audit.RunWorkerAsync();
        }

        // Check how many levels deep the application needs to scan and audit
        private void CalculateHowManyLevelsDeepToScan(string levelsDeep)
        {
            if (levelsDeep == "unlimited")
            {
            }

            this.numLevelsDeepSetting = levelsDeep switch
            {
                "1" => 1,
                "2" => 2,
                "3" => 3,
                "4" => 4,
                "5" => 5,
                "Unlimited" => 9999999,
                _ => this.numLevelsDeepSetting
            };
        }

        private void bgWorker_Scan_DoWork(object sender, DoWorkEventArgs e)
        {
            Thread.Sleep(1500); // Sleep to stop panel flicker on shorter audits (< 1s)
            DoesFileExist();
            DirectorySearch(auditLocation, exportLocation, showSystemAccounts, 1);
        }

        private void bgWorker_Scan_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e,
            bool summary)
        {
            var dt = DateTime.Now;

            // Add finished audit information to the logfile
            if (!summary)
            {
                log.AddToPermissionsLogFile(System.Environment.NewLine + System.Environment.NewLine +
                                            System.Environment.NewLine);
                log.AddToPermissionsLogFile("Folders Scanned: " + foldersScanned +
                                            " | Folders with access errors (can not scan deeper):  " +
                                            foldersAccessErrors);
                log.AddToPermissionsLogFile("Date/Time log completed:" + dt.ToString("HH:mm:ss dd/MM/yy"));
            }

            if (foldersAccessErrors > 0)
            {
                // log.AddToPermissionsLogFile(System.Environment.NewLine);
            }
        }

        private void DoesFileExist()
        {
            var file = exportLocation + @"\" + csvFilename + @"_" + auditLocation.Replace(@"\", "_").Replace(@":", "") +
                       "-permissions.csv";
            if (File.Exists(file))
                File.Delete(file);
        }

        // This function is where directories are recursively audited
        private void DirectorySearch(string location, string exportLocation, bool showSystemAccounts, int currentLevel)
        {
            if (currentLevel > numLevelsDeepSetting)
            {
                // Reached levels limit so do not continue
            }
            else
            {
                var dirs = new List<string>();
                try
                {
                    foreach (var dir in Directory.GetDirectories(location))
                    {
                        foldersCounted++;
                        dirs.Add(dir);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    // Suppress error because you want the audit scan to continue
                    foldersAccessErrors++;
                }

                foreach (var dir in dirs)
                {
                    currentFolder = dir;
                    foldersScanned++;

                    try
                    {
                        var logListCSV = new List<string>();
                        var di = new DirectoryInfo(dir);
                        var permissions = di.Attributes;
                        var csvDir = dir;
                        var csvPermissions = "";

                        var dSecurity = Directory.GetAccessControl(dir);
                        foreach (FileSystemAccessRule rule in dSecurity.GetAccessRules(true, true, typeof(NTAccount)))
                        {
                            // Console.WriteLine(csvDir + " ::: " + rule.IdentityReference + " [" + rule.AccessControlType + ": " + rule.FileSystemRights + "]"); 

                            // Check that the results do not contain numeric ACLs -> msdn.microsoft.com/en-us/library/aa364399.aspx
                            if
                            (
                                !rule.FileSystemRights.ToString().Contains("268435456") &&
                                !rule.FileSystemRights.ToString().Contains("-536805376") &&
                                !rule.FileSystemRights.ToString().Contains("-1610612736")
                            )
                            {
                                if (showSystemAccounts)
                                {
                                    string csvFormat = rule.IdentityReference + " [" + rule.AccessControlType + ": " +
                                                       rule.FileSystemRights + "]";
                                    string csvSantised = csvFormat.Replace(",", " ");
                                    csvPermissions += "; " + csvSantised;
                                }
                                else
                                {
                                    // Check results do not show system accounts
                                    if (!rule.IdentityReference.ToString().Contains("BUILTIN") &&
                                        !rule.IdentityReference.ToString().Contains(@"NT AUTHORITY\SYSTEM"))
                                    {
                                        string csvFormat = rule.IdentityReference + " [" + rule.AccessControlType +
                                                           ": " + rule.FileSystemRights + "]";
                                        string csvSantised = csvFormat.Replace(",", " ");
                                        csvPermissions += ";" + csvSantised;
                                    }
                                }
                            }
                        }

                        // Add string to List<string>
                        // logListCSV.Add(csvDir + ", " + csvPermissions);
                        // Console.WriteLine(csvPermissions);
                        csvPermissions = csvPermissions.Replace(";", " \n");
                        string[] Array = csvPermissions.Split("\n"[0]);
                        for (int i = 0; i < Array.Length; i++)
                        {
                            logListCSV.Add(csvDir + ";" + Array[i]);
                        }
                        // Console.ReadKey();

                        // Add List<string> to logfile function
                        // Console.WriteLine(string.Join("\n", logListCSV));
                        log.AddListToPermissionsLogFile(logListCSV);
                    }
                    catch (Exception e)
                    {
                        foldersAccessErrors++;
                        log.AddToPermissionsLogFile(" -----------------> Error accessing directory. " + e);
                        continue;
                    }

                    // Keep recursive search going! 
                    DirectorySearch(dir, exportLocation, showSystemAccounts, currentLevel + 1);
                }
            }
        }
    }
}