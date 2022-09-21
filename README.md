# MatchingEngine

System to match records from different sources utilizing demographic data.

## CLI Installation

To build the project you need the .NET 7 SDK. You can download it from [here](https://dotnet.microsoft.com/download/dotnet/7.0).

Build the cli tool utilizing the following command:
```bash
$ dotnet publish -c Release MatchingEngine.CLI
```

This will create an executable file for Linux-x64 systems. To build for other architectures, pass the -r parameter as the following example:
```bash
$ dotnet publish -c Release -r win-x64 MatchingEngine.CLI
```

For more information on the -r parameter, see the [dotnet publish documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish).
For a list of supported runtimes, see the [dotnet RID catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

## CLI Usage

The cli tool has a help command that will list all available commands and their parameters.
```bash
$ matching --help
```

### Find duplicates



Create or use an existing template.
```bash
$ matching template generate -o <output file path>
```

Create word frequency dictionary based on sample data to improve matching accuracy.
```bash
$ matching dictionary generate  <templateFilePath> -o <output folder path>
```

Find duplicates between 2 files.
This assumes that the Dictionary has already been generated and is located in the same directory as the executable.
```bash
$ matching duplicates find <templateFilePath1> <templateFilePath2> -o <output file path>
```

This will generate a file with all duplicates found and scores for each duplicate. The higher the score, the more likely the records are duplicates.