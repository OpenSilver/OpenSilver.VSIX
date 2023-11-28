# How to build the VSIX installer?

Building the VSIX is only useful if you wish to test your changes to the Project Templates, the Item Templates, or the XAML editor. For all the other scenarios, building the Runtime or the NuGet package is sufficient to test your changes.

1. If needed, change the version number in "source.extension.vsixmanifest" (located in the project "OpenSilver.VSIX")
2. With Visual Studio 2022, open the solution "VSExtension.OpenSilver.sln" to build the VSIX of OpenSilver
3. Visual Studio may prompt you to install additional necessary components. If prompted, proceed with the installation.
4. Rebuild the solution
5. The VSIX will be generated in a subfolder of the "bin" folder of the .VSIX project

If you still encounter any issues, please contact:
- the OpenSilver team at: https://opensilver.net/contact.aspx
