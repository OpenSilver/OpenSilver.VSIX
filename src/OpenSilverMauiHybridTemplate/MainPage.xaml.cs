using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Controls.PlatformConfiguration.iOSSpecific;

namespace $safeprojectname$;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Prevents content from overlapping the status bar on iOS
        On<iOS>().SetUseSafeArea(true);
    }
}
