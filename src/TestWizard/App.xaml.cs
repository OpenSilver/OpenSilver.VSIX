using OpenSilver.TemplateWizards.AppCustomizationWindow;
using System.Windows;

namespace TestWizard;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        MainWindow = new Window();

        var configurationWindow = new AppConfigurationWindow();
        var res = configurationWindow.ShowDialog();

        var theme = configurationWindow.SelectedTheme;
        var dotNetVersion = configurationWindow.DotNetVersion;
        var mauiHybridPlatform = configurationWindow.MauiHybridPlatform;
        var isPhotinoSelected = configurationWindow.IsPhotinoSelected;

        if (res == true)
        {
            MessageBox.Show(
                $"Theme: {configurationWindow.SelectedTheme}\n" +
                $".NET Version: {configurationWindow.DotNetVersion}\n" +
                $"MAUI Hybrid Platform: {configurationWindow.MauiHybridPlatform}\n" +
                $"Is Photino Selected: {configurationWindow.IsPhotinoSelected}",
                "Configuration Details",
                MessageBoxButton.OK);
        }
        MainWindow.Close();
    }
}

