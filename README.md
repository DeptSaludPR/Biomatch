# Biomatch

System to match records utilizing demographic data.

## CLI Installation

To build the project you need the .NET 8 SDK. You can download it from [here](https://dotnet.microsoft.com/download/dotnet/8.0).

Build the cli tool utilizing the following command:
```zsh
dotnet publish -c Release src/Biomatch.CLI
```

This will create a "**matching**" executable file for your current architecture. To build for other architectures, pass the -r parameter as the following example:
```zsh
dotnet publish -c Release -r linux-x64 src/Biomatch.CLI -o biomatch
```

For more information on the -r parameter, see the [dotnet publish documentation](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish).
For a list of supported runtimes, see the [dotnet RID catalog](https://docs.microsoft.com/en-us/dotnet/core/rid-catalog).

## CLI Usage

The cli tool has a help command that will list all available commands and their parameters.
```zsh
biomatch --help
```

### Template

The template command will create a template file for the matching engine. The template file is a csv utf-8 file.
The template file is used for most matching commands. The template file has the following columns:

| Column Name    | Type     | Description                                |
|----------------|----------|--------------------------------------------|
| RecordId       | string   | The unique id of the record in the source. |
| FirstName      | string   | First name of person.                      |
| MiddleName     | string   | Middle name of person.                     |
| LastName       | string   | Last name of person.                       |
| SecondLastName | string   | Second last name of person.                |
| Birthdate      | DateTime | Birth date of person.                      |
| City           | string   | Physical residence city of person.         |
| PhoneNumber    | string   | Primary phone number of person.            |

```zsh
biomatch template generate -o <output file path>
```

### Find matches between 2 files

Create or use an existing template. (See [Template](#template) section for more information)
```zsh
biomatch template generate -o <output file path>
```

Create word frequency dictionary based on sample data to improve matching accuracy.
```zsh
biomatch dictionary generate <templateFilePath> -o <output folder path>
```

Find matches between 2 files with a score threshold of 0.85.
The following command assumes that the Dictionary has already been generated and is located in the same directory as the executable.
```zsh
biomatch find matches <templateFilePath1> <templateFilePath2> -o <output file path> --score 0.85
```

This will generate a file with all matches found and scores for each one. The higher the score, the more likely the records match.

### Find duplicates between 2 files

Create or use an existing template. (See [Template](#Template) section for more information)
```zsh
biomatch template generate -o <output file path>
```

Create word frequency dictionary based on sample data to improve matching accuracy.
```zsh
biomatch dictionary generate <templateFilePath> -o <output folder path>
```

Find duplicates between 2 files with a score threshold of 0.85.
The following command assumes that the Dictionary has already been generated and is located in the same directory as the executable.
```zsh
biomatch find duplicates <templateFilePath1> <templateFilePath2> -o <output file path>  --score 0.85
```

This will generate a file with all duplicates found and scores for each one. The higher the score, the more likely the records are duplicates.
