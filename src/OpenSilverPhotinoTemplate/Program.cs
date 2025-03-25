using OpenSilver.Photino;
using Photino.NET;

namespace $safeprojectname$
{
    //NOTE: To hide the console window, go to the project properties and change the Output Type to Windows Application.
    // Or edit the .csproj file and change the <OutputType> tag from "WinExe" to "Exe".
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // Window title declared here for visibility
            string windowTitle = "$rootprojectname$";

            // Creating a new PhotinoWindow instance with the fluent API
            var window = new PhotinoWindow()
                .SetTitle(windowTitle)
                .SetUseOsDefaultSize(true)
                .Center() // Center window in the middle of the screen
                .SetResizable(false) // Users can resize windows by default. Let's make this one fixed instead.
                .SetLogVerbosity(0)
                .ConfigureOpenSilver<App>() // Configure OpenSilver App
                .Load("wwwroot/index.html"); // Can be used with relative path strings or "new URI()" instance to load a website.

            window.WaitForClose(); // Starts the application event loop
        }
    }
}
