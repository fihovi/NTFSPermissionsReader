using System;
using CommandLine;
using NTFSPermissions.Lang;

namespace NTFSPermissions
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = @"Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('i', "input", Required = true, HelpText = @"ScanLocation")]
            public string ScanLocationPath { get; set; }

            [Option('o', "output", Required = true, HelpText = @"Export Location for csv file")]
            public string ExportLocationPath { get; set; }

            [Option('f', "force", Required = false, Default = false, HelpText = @"Force skip check for scan")]
            public bool ForceScan { get; set; }

            [Option('s', "systemAccounts", Default = false, Required = false,
                HelpText = @"Allow to include system Accounts in the report.")]
            public bool AllowSystemAccounts { get; set; }

            [Option('p', "prefix", Default = "", Required = false, HelpText = @"Prefix of the report")]
            public string CsvFilename { get; set; }

            [Option('m', "summary", Default = false, Required = false, HelpText = @"Summary on the end of a report")]
            public bool Summary { get; set; }
        }

        private static void Main(string[] args)
        {
            string scanLocation = "";
            string exportLocation = "";
            bool showSystemAccount = false;
            bool forceScan = false;
            bool summary = true;
            string csvFilename = "";

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    Console.WriteLine(o.Verbose
                        ? $@"Verbose output enabled. Current Arguments: -v {o.Verbose.ToString()}"
                        : $@"Current Arguments: -v {o.Verbose.ToString()}");
                    /*Console.WriteLine(o.Verbose
                        ? $@"Verbose output enabled. Current Arguments: -v {o.Verbose}"
                        : $@"Current Arguments: -v {o.Verbose}");*/

                    if (o.ScanLocationPath != "")
                        scanLocation = o.ScanLocationPath;
                    else
                        Console.WriteLine(Language_Main.NotDefined, Language_Main.Scan);

                    if (o.ExportLocationPath != "")
                        exportLocation = o.ExportLocationPath;
                    else
                        Console.WriteLine(Language_Main.NotDefined, Language_Main.Export);


                    if (o.CsvFilename != "")
                        csvFilename = o.CsvFilename;
                    summary = o.Summary;
                    Console.WriteLine(@"Summary: {0}", summary.ToString());
                    forceScan = o.ForceScan;

                    /*Console.WriteLine(o.AllowSystemAccounts
                        ? $@"System Accounts shown in report -s"
                        : $@"systemAccounts not shown in report -s");*/
                    showSystemAccount = o.AllowSystemAccounts;
                });

            Console.WriteLine(Language_Main.ScanLocation, scanLocation);
            Console.WriteLine(Language_Main.ExportLocation, exportLocation);
            Console.WriteLine(Language_Main.ShowSystemAccount, showSystemAccount.ToString());

            if (!forceScan)
            {
                // ReSharper disable StringLiteralTypo
                Console.Write(Language_Main.Continue);
                string userResponse = Console.ReadLine();
                switch (userResponse)
                {
                    case "y":
                    case "Y":
                    case "a":
                    case "A":
                        // ReSharper disable StringLiteralTypo
                        Console.WriteLine(@Language_Main.Scanning);
                        Audit audit = new Audit(scanLocation, exportLocation, "9999999", false, showSystemAccount,
                            csvFilename, summary);
                        //TODO FIXME Wait for scan to be done, then exit itself.
                        Console.Write(Language_Main.ScanFinished);
                        Console.ReadKey();
                        Console.Write(Language_Main.ScanFinished);
                        Environment.Exit(0);
                        break;
                    default:
                        // ReSharper disable StringLiteralTypo
                        Console.WriteLine(Language_Main.Exiting);
                        Console.ReadKey();
                        Console.WriteLine(Language_Main.Exiting);
                        Environment.Exit(0);
                        break;
                }
            }
            else
            {
                Audit audit = new Audit(scanLocation, exportLocation, "9999999", false, showSystemAccount, csvFilename, summary);
                Console.ReadKey();
                Console.WriteLine(Language_Main.Exiting);
                Environment.Exit(0);
            }

            Console.ReadKey();
        }
    }
}