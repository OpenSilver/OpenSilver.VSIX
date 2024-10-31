
# OpenSilver CLI Templates

Welcome to the OpenSilver CLI Templates! This NuGet package provides a set of templates for creating OpenSilver applications and libraries quickly and efficiently. With templates designed for applications, business applications (built on OpenRia), and class libraries across multiple languages (C#, VB, and F#), this package empowers developers to kickstart OpenSilver projects with the structure they need.

## Templates Included

1. **OpenSilver Application**
   - A standard OpenSilver application template to create web applications using the power of OpenSilver and .NET. Supports C#, VB, and F#.
  
2. **OpenSilver Business Application**
   - A business-focused template that leverages OpenRia (ported to OpenSilver), ideal for enterprise-level applications needing robust data management and communication. Supports C# and VB.

3. **OpenSilver Class Library**
   - Class libraries in C#, VB, and F# to support modularity and reusability in OpenSilver applications.

Each template is designed to help developers focus on building applications by providing a clear, pre-configured structure that works out of the box with OpenSilver.

## Installation

To install the CLI templates, use the following command:

```shell
dotnet new install OpenSilver.Templates
```

Once installed, you can use `dotnet new` commands to create new projects from these templates.

## Usage

### Creating an OpenSilver Project

To create an OpenSilver Application, specify the language or let C# be the default:

```shell
dotnet new opensilverapp -n MyOpenSilverApp
```

For a Business Application (using OpenRia):

```shell
dotnet new opensilverbusinessapp -n MyOpenSilverBusinessApp
```

To create a Class Library, choose the language-specific template or default to C#:

```shell
dotnet new opensilverlib -lang C# -n MyOpenSilverClassLibrary
```

Alternatively, for VB or F#, replace `C#` with `VB` or `F#` in the `-lang` parameter.

These commands will generate a new project folder with the specified name, template structure, and language.

## Template Overview

| Template                   | Short Name              | Language(s) Supported | Default Language |
|----------------------------|-------------------------|------------------------|------------------|
| OpenSilver Application     | opensilverapp           | C#, VB, F#            | C#               |
| OpenSilver Business App    | opensilverbusinessapp   | C#, VB                | C#               |
| OpenSilver Class Library   | opensilverlib           | C#, VB, F#            | C#               |

## Support

For issues, please visit the [OpenSilver repository](https://github.com/OpenSilver/OpenSilver) and submit an issue. For more information on OpenSilver, visit the [OpenSilver website](https://opensilver.net/).
