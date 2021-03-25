using System;
using System.Collections;
using System.Collections.Generic;
using CommandLine;
using NTFSPermissions.Lang;

namespace NTFSPermissions
{
    class Program
    {
        
        private static string _scanLocation = "";
        private static string _exportLocation = "";
        private static bool _showSystemAccount = false;
        private static bool _forceScan = false;
        private static bool _summary = true;
        private static string _csvFilename = "";

        public class Options
        {
            public Options(string scanLocationPath, string exportLocationPath)
            {
                ScanLocationPath = scanLocationPath ?? throw new ArgumentNullException(nameof(scanLocationPath));
                ExportLocationPath = exportLocationPath ?? throw new ArgumentNullException(nameof(exportLocationPath));
            }

            public Options(bool verbose = default, bool forceScan = default, bool allowSystemAccounts = default,
                string csvFilename = null, bool summary = default)
            {
                Verbose = verbose;
                ForceScan = forceScan;
                AllowSystemAccounts = allowSystemAccounts;
                CsvFilename = csvFilename;
                Summary = summary;
            }

            public Options(bool summary, string csvFilename, string scanLocationPath, string exportLocationPath, bool forceScan, bool allowSystemAccounts)
            {
                Summary = summary;
                CsvFilename = csvFilename;
                ScanLocationPath = scanLocationPath;
                ExportLocationPath = exportLocationPath;
                ForceScan = forceScan;
                AllowSystemAccounts = allowSystemAccounts;
            }

            [Option('v', "verbose", Required = false, HelpText = @"Set output to verbose messages.")]
            public bool Verbose { get; }

            [Option('i', "input", Required = true, HelpText = @"ScanLocation")]
            public string ScanLocationPath { get; }

            [Option('o', "output", Required = true, HelpText = @"Export Location for csv file")]
            public string ExportLocationPath { get; }

            [Option('f', "force", Required = false, Default = false, HelpText = @"Force skip check for scan")]
            public bool ForceScan { get; }

            [Option('s', "systemAccounts", Default = false, Required = false,
                HelpText = @"Allow to include system Accounts in the report.")]
            public bool AllowSystemAccounts { get; }

            [Option('p', "prefix", Default = "", Required = false, HelpText = @"Prefix of the report")]
            public string CsvFilename { get; }

            [Option('m', "summary", Default = false, Required = false, HelpText = @"Summary on the end of a report")]
            public bool Summary { get; }
        }

        private static void RunOptions(Options opts)
        {
            Console.WriteLine(opts.Verbose
                ? $@"Verbose output enabled. Current Arguments: -v {opts.Verbose.ToString()}"
                : $@"Current Arguments: -v {opts.Verbose.ToString()}");
            /*Console.WriteLine(opts.Verbose
                ? $@"Verbose output enabled. Current Arguments: -v {opts.Verbose}"
                : $@"Current Arguments: -v {opts.Verbose}");*/

            if (opts.ScanLocationPath != "")
                _scanLocation = opts.ScanLocationPath;
            else
                Console.WriteLine(Language_Main.NotDefined, Language_Main.Scan);

            if (opts.ExportLocationPath != "")
                _exportLocation = opts.ExportLocationPath;
            else
                Console.WriteLine(Language_Main.NotDefined, Language_Main.Export);


            if (opts.CsvFilename != "")
                _csvFilename = opts.CsvFilename;
            _summary = opts.Summary;
            Console.WriteLine(@"Summary: {0}", _summary.ToString());
            _forceScan = opts.ForceScan;

            /*Console.WriteLine(opts.AllowSystemAccounts
                ? $@"System Accounts shown in report -s"
                : $@"systemAccounts not shown in report -s");*/
            _showSystemAccount = opts.AllowSystemAccounts;
            
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine(errs);
            // Environment.Exit(0);
        }

        private static void Main(string[] args)
        {

            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(RunOptions)
                .WithNotParsed(HandleParseError);

                Console.WriteLine(Language_Main.ScanLocation, _scanLocation);
            Console.WriteLine(Language_Main.ExportLocation, _exportLocation);
            Console.WriteLine(Language_Main.ShowSystemAccount, _showSystemAccount.ToString());

            if (!_forceScan)
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
                        Audit audit = new Audit(_scanLocation, _exportLocation, "9999999", false, _showSystemAccount,
                            _csvFilename, _summary);
                        Console.WriteLine(audit);
                        //TODO FIXME Wait for scan to be done, then exit itself.
                        Console.Write(Language_Main.ScanFinished);
                        Console.ReadKey();
                        Console.WriteLine(Language_Main.Exiting);
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
                Audit audit = new Audit(_scanLocation, _exportLocation, "9999999", false, _showSystemAccount, _csvFilename, _summary);
                Console.WriteLine(audit);
                Console.ReadKey();
                Console.WriteLine(Language_Main.Exiting);
                Environment.Exit(0);
            }

            Console.ReadKey();
        }
    }
}