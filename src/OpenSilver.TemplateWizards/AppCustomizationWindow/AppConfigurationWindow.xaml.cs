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
        public string SelectedTheme { get; private set; }

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

            //Modern theme is deactivated for now, for Business Application projects
            if (isBusiness)
            {
                chooseThemesCollection.Visibility = Visibility.Collapsed;
                SelectedTheme = ThemeOptions.Classic;
                continueBtn.IsEnabled = true;
            }
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void ThemeCollectionView_SelectionChanged(object sender, EventArgs e)
        {
            SelectedTheme = (sender as ThemeOptions).Name;
            continueBtn.IsEnabled = true;
        }
    }
}
