using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using OpenSilver.TemplateWizards.AppCustomizationWindow.Models;
using OpenSilver.TemplateWizards.Shared;
using OpenSilver.TemplateWizards.Utils;
using System;
using System.Linq;
using System.Windows;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppConfigurationWindow : Window
    {
        private EnvDTE.DTE _dte;

        private bool? _isMauiInstalled;

        public bool IsMauiInstalled
        {
            get
            {
                if (_isMauiInstalled == null)
                {
                    if (_dte != null)
                    {
                        ThreadHelper.ThrowIfNotOnUIThread();
                        var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_dte);

                        var checker = new MauiInstallationChecker(serviceProvider);
                        _isMauiInstalled = checker.IsMauiInstalled();
                    }
                    else
                    {
                        // for local debug
                        _isMauiInstalled = false;
                    }
                }
                return _isMauiInstalled.Value;
            }
        }

        public ThemeOptions SelectedTheme { get; private set; }

        public DotNetVersion DotNetVersion => (DotNetVersion)DotNetVersionComboBox.SelectedValue;

        public MauiHybridPlatform MauiHybridPlatform
        {
            get
            {
                MauiHybridPlatform platforms = MauiHybridPlatform.None;

                foreach (var platform in platformList.SelectedItems.OfType<TargetPlatform>())
                {
                    switch (platform.Title.ToLower())
                    {
                        case "ios":
                            platforms |= MauiHybridPlatform.iOS;
                            break;
                        case "android":
                            platforms |= MauiHybridPlatform.Android;
                            break;
                        case "windows":
                            platforms |= MauiHybridPlatform.Windows;
                            break;
                        case "macos":
                            platforms |= MauiHybridPlatform.Mac;
                            break;
                        default:
                            break;
                    }
                }

                return platforms;
            }
        }

        public AppConfigurationWindow(EnvDTE.DTE dte = null, bool isBusiness = false)
        {
            InitializeComponent();

            _dte = dte;

            if (isBusiness)
            {
                //Modern theme is deactivated for now, for Business Application projects
                chooseThemeLabel.Visibility = Visibility.Collapsed;
                themeList.Visibility = Visibility.Collapsed;
                themeList.Select(ThemeOptions.Classic);
            }
            else
            {
                //Select default theme based on operating system settings
                themeList.Select(IsSystemThemeLight() ? ThemeOptions.Light : ThemeOptions.Dark);
            }
        }

        private bool ValidateMaui()
        {
            if (MauiHybridPlatform == MauiHybridPlatform.None)
            {
                return true;
            }

            if (_dte != null)
            {
                ThreadHelper.ThrowIfNotOnUIThread();
            }

            try
            {
                if (_dte != null)
                {
                    var serviceProvider = new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)_dte);

                    var checker = new MauiInstallationChecker(serviceProvider);
                    bool mauiInstalled = checker.IsMauiInstalled();

                    if (mauiInstalled)
                    {
                        return true;
                    }
                }

                return new InstallMauiWindow().ShowDialog() == true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Exception: {ex.Message}");
                return true;
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

        private void PlatformList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (MauiHybridPlatform != MauiHybridPlatform.None && e.AddedItems.Count != 0 && !IsMauiInstalled)
            {
                if (new InstallMauiWindow().ShowDialog() != true)
                {
                    foreach (var item in e.AddedItems)
                    {
                        platformList.SelectedItems.Remove(item);
                    }
                }
            }

            mauiTip.Visibility = MauiHybridPlatform == MauiHybridPlatform.None ? Visibility.Hidden : Visibility.Visible;
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
