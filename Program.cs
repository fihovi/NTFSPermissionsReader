using System;
using CommandLine;


namespace NTFSPermissions
{
    internal abstract class Program
    {
        private abstract class Options
        {
            protected Options(string exportLocationPath, string scanLocationPath, bool verbose, bool forceScan, bool allowSystemAccounts, string csvFilename)
            {
                ExportLocationPath = exportLocationPath;
                ScanLocationPath = scanLocationPath;
                Verbose = verbose;
                ForceScan = forceScan;
                AllowSystemAccounts = allowSystemAccounts;
                CsvFilename = csvFilename;
            }

            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; private set; }
            [Option('i', "input", Required = true, HelpText = "ScanLocation")]
            public string ScanLocationPath { get; private set; }
            [Option('o', "output", Required = true, HelpText = "Export Location for csv file")]
            public string ExportLocationPath { get; private set; }
            [Option('f', "force", Required = false, Default = false, HelpText = "Force skip check for scan")]
            public bool ForceScan { get; private set; }

            [Option('s', "systemAccounts", Default = false, Required = false, HelpText = "Allow to include system Accounts in the report.")]
            public bool AllowSystemAccounts { get; private set; }
            [Option('n', "filename", Default = "", Required = false, HelpText = "Define filename of the report")]
            public string CsvFilename { get; private set; }
        }

        private static void Main(string[] args)
        {
            string scanLocation = "";
            string exportLocation = "";
            bool showSystemAccount = false;
            bool forceScan = false;
            string csvFilename = "";

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    Console.WriteLine(o.Verbose
                        ? $@"Verbose output enabled. Current Arguments: -v {o.Verbose.ToString()}"
                        : $@"Current Arguments: -v {o.Verbose.ToString()}");

                    if (o.ScanLocationPath != "")
                        scanLocation = o.ScanLocationPath;
                    else
                        Console.WriteLine(@"Error, scanLocation is not defined.");

                    if (o.ExportLocationPath != "")
                        exportLocation = o.ExportLocationPath;
                    else
                        Console.WriteLine(@"exportLocation is not defined.");

                    if (o.CsvFilename != "")
                        csvFilename = o.CsvFilename;

                    forceScan = o.ForceScan;

                    Console.WriteLine(o.AllowSystemAccounts
                    ? $@"System Accounts shown in report -s"
                    : $@"systemAccounts not shown in report -s");
                    showSystemAccount = o.AllowSystemAccounts;
                });

            Console.WriteLine(@"Scan location: " + scanLocation);
            Console.WriteLine(@"csv output: " + exportLocation);
            Console.WriteLine(@"Show system accounts in report: {0}", showSystemAccount.ToString());

            if (!forceScan)
            {

                // ReSharper disable StringLiteralTypo
                Console.Write(@"Chcete pokračovat? (y/n)");
                var userResponse = Console.ReadLine();
                switch (userResponse)
                {
                    case "y":
                        // ReSharper disable StringLiteralTypo
                        Console.WriteLine(@"Calling scan function");
                        var audit = new Audit(scanLocation, exportLocation, "9999999", false, showSystemAccount, csvFilename);
                        Console.ReadKey();
                        break;
                    default:
                        // ReSharper disable StringLiteralTypo
                        Console.WriteLine(@"Program is turning off");
                        Console.ReadKey();
                        break;
                }
            }
            else
            {
                var audit = new Audit(scanLocation, exportLocation, "9999999", false, showSystemAccount, csvFilename);
            }

            Console.ReadKey();
        }
    }
}
