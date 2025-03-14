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
        _ = configurationWindow.ShowDialog();

        var theme = configurationWindow.SelectedTheme;
        var dotNetVersion = configurationWindow.DotNetVersion;
        var mauiHybridPlatform = configurationWindow.MauiHybridPlatform;

        MainWindow.Close();
    }
}

