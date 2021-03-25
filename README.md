[![.NET CI](https://github.com/fihovi/NTFSPermissionsReader/actions/workflows/dotnet.yml/badge.svg)](https://github.com/fihovi/NTFSPermissionsReader/actions/workflows/dotnet.yml)
[![Build Status](https://dev.azure.com/fihovi/NTFSPermission/_apis/build/status/fihovi.NTFSPermissionsReader?branchName=main)](https://dev.azure.com/fihovi/NTFSPermission/_build/latest?definitionId=1&branchName=main)

# NTFSPermissionsReader
Reads permissions of a folder and all subfolders

## Usage 
```
Required option 'i, input'.
Required option 'o, output'.

  -v, --verbose           Set output to verbose messages.

  -i, --input             Required. ScanLocation

  -o, --output            Required. Export Location for csv file

  -f, --force             (Default: false) Force skip check for scan

  -s, --systemAccounts    (Default: false) Allow to include system Accounts in the report.

  -p, --prefix            (Default: "") Prefix of the report

  -m, --summary           (Default: false) Summary on the end of a report

  --help                  Display this help screen.

  --version               Display version information.
  ```

## Known bugs
- [ ] Too many commas behind the permissions

## ToDo's
- [x] Localization
- [x] CI/CD
  - [x] GitHub Actions
  - [ ] Azure Pipelines
  - [ ] ~~AWS CodePipeline~~
