using Microsoft.Win32;
using OpenSilver.TemplateWizards.AppCustomizationWindow.Models;
using OpenSilver.TemplateWizards.Shared;
using System;
using System.Windows;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppConfigurationWindow : Window
    {
        public ThemeOptions SelectedTheme { get; private set; }

        public DotNetVersion DotNetVersion
        {
            get
            {
                switch (DotNetVersionComboBox.SelectedIndex)
                {
                    case 0:
                        return DotNetVersion.Net7;
                    case 1:
                        return DotNetVersion.Net8;
                    case 2:
                        return DotNetVersion.Net9;
                    default:
                        throw new InvalidOperationException("Error retrieving selected .NET version");
                }
            }
        }

        public MauiHybridPlatform MauiHybridPlatform
        {
            get
            {
                MauiHybridPlatform platforms = MauiHybridPlatform.None;

                if (IosCheckbox.IsChecked == true)
                {
                    platforms |= MauiHybridPlatform.iOS;
                }
                if (AndroidCheckbox.IsChecked == true)
                {
                    platforms |= MauiHybridPlatform.Android;
                }
                if (WindowsCheckbox.IsChecked == true)
                {
                    platforms |= MauiHybridPlatform.Windows;
                }
                if (MacCheckbox.IsChecked == true)
                {
                    platforms |= MauiHybridPlatform.Mac;
                }

                return platforms;
            }
        }

        public AppConfigurationWindow(bool isBusiness = false)
        {
            InitializeComponent();

            if (isBusiness)
            {
                //Modern theme is deactivated for now, for Business Application projects
                chooseThemeLabel.Visibility = Visibility.Collapsed;
                chooseThemesCollection.Visibility = Visibility.Collapsed;
                themeList.Select(ThemeOptions.Classic);
            }
            else
            {
                //Select default theme based on operating system settings
                themeList.Select(IsSystemThemeLight() ? ThemeOptions.Light : ThemeOptions.Dark);
            }
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ThemeCollectionView_SelectionChanged(object sender, ThemeOptions theme)
        {
            SelectedTheme = theme;
            continueBtn.IsEnabled = SelectedTheme != null;
        }

        // https://github.com/dotnet/wpf/blob/090a5230cf6186fe576dbc1729c943b36cb5db71/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/ThemeManager.cs
        private static bool IsSystemThemeLight()
        {
            var useLightTheme = Registry.GetValue(RegPersonalizeKeyPath, "AppsUseLightTheme", null) as int?;

            if (useLightTheme == null)
            {
                useLightTheme = Registry.GetValue(RegPersonalizeKeyPath, "SystemUsesLightTheme", null) as int?;
            }

            return useLightTheme != null && useLightTheme != 0;
        }

        private const string RegPersonalizeKeyPath = "HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
    }
}
