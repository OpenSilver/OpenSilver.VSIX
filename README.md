# How to build the VSIX installer?

Building the VSIX is only useful if you wish to test your changes to the Project Templates, the Item Templates, or the XAML editor. For all the other scenarios, building the Runtime or the NuGet package is sufficient to test your changes.

1. If needed, change the version number in "source.extension.vsixmanifest" (located in the project "OpenSilver.VSIX")
2. With Visual Studio 2022, open the solution "VSExtension.OpenSilver.sln" to build the VSIX of OpenSilver
3. Visual Studio may prompt you to install additional necessary components. If prompted, proceed with the installation.
4. Rebuild the solution
5. The VSIX will be generated in a subfolder of the "bin" folder of the .VSIX project

# OpenSilver cross-platform CLI tools
Now OpenSilver support two ways to create a new project starting from a template, the first one is the classi VSIX that install in VisualStudio few ready to use templates. The second option is using the cross-platform .net core CLI interface.

# What is the .net CLI?
The .NET Command-Line Interface (CLI) is a cross-platform toolchain for developing, building, running, and publishing .NET applications1. The .NET CLI is included with the .NET SDK and now OpenSilver supports also this workflow.

That way you can create new projects on any platform like Linux, MacOS or Widnows and then use your favourite IDE for development, like VSCode.

# A quick intro
.net core CLI has a different templating approach than VisualStudio, the big difference is that it does not use any special token in the source files, in that way CLI templates are also called "runneble templates", in fact if you go in the folder .src/OpenSilver.Cli/Templates you will find different solutions (in C# and VB) that can be open and customized via Visual Studio or by hand if you prefer.

Then you can compile a nuget package for distribution/install containing all the templates. To do so simply open the solution file OpenSilverTemplates.sln in the ./src/Opensilder.CLI folder to compile your nuget package via the standard build command.

After you can isntall it by issuing this command in the CLI (you have to move in the .\nugetpkg folder to find the package created by Visual Studio):
dotnet new install .\OpenSilver.CLI.Templates.1.0.0.nupkg

From now on you will have three new templates available via the CLI, on any platform.

You can list the available .NET CLI templates by using the command: 

	dotnet new --list or dotnet new -l 

This command will display the pre-installed .NET Core project templates. The list includes details such as the name of the templates, the short name of the template, default programming language, and the template tags. That way you should see three new templates under the category OpenSilver.CLI.Tools

# Create your first OpenSilver project via the CLI

In order to create your first OpenSilver app you have to choose the available template you want to use (WebApp, Business app etc). and then issue the command:

	dotnet new opensilverapp -n MyProject -ta net7.0

This will create in the current directory an new Opensilver web project called "MyProject" targeting .net 7.0 (the default). Remember that you must use the short name of the template not the full template name. Our templates will also take care of restoring the necessary nuget packages from the default repositories.

For more information about the command line switches you can run this command to get all the information you need for each template.

	dotnet new opensilverapp -h

If you need help for another OpenSilver template, let's say the Class Library template just change the name of the template as follows:

	dotnet new opensilvercl -h

If you still encounter any issues, please contact:
- the OpenSilver team at: https://opensilver.net/contact.aspx
