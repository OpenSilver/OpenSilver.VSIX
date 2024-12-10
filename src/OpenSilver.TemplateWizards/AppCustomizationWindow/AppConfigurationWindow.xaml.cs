using OpenSilver.TemplateWizards.AppCustomizationWindow.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppConfigurationWindow : Window
    {
        public IDictionary<ThemeSelectionEnum, string> Themes { get; set; } = new Dictionary<ThemeSelectionEnum, string>();
        public string SelectedTheme { get; set; }
        public OpenSilverBuildType OpenSilverBuildType
        {
            get
            {
                return OpenSilverBuildType.Stable;
            }
        }

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


        public AppConfigurationWindow(string openSilverType, bool isBusiness = false)
        {

            InitilizeThemes();
            InitializeComponent();
            if (openSilverType == "Library")
            {
                DotNetVersionPanel.Visibility = Visibility.Collapsed;
            }

            //Modern theme is deactivated for now, for Business Application projects
            if (isBusiness)
            {
                chooseThemesCollection.Visibility = Visibility.Collapsed;
                SelectedTheme = Themes[ThemeSelectionEnum.Classic];
                continueBtn.IsEnabled = true;
                this.Height = 300;
            }
        }

        private void InitilizeThemes()
        {
            Themes.Add(ThemeSelectionEnum.Dark, "Dark");
            Themes.Add(ThemeSelectionEnum.Light, "Light");
            Themes.Add(ThemeSelectionEnum.Classic, "Classic");
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void OnThemSelected(object sender, RoutedEventArgs e)
        {

            if (sender is RadioButton selectedRadio)
            {
                try
                {
                    SelectedTheme = Themes[(ThemeSelectionEnum)selectedRadio.Tag];
                }
                catch
                {
                    throw new InvalidOperationException("Error retrieving selected theme option");
                }
            }

        }

        private void ThemeCollectionView_SelectionChanged(object sender, EventArgs e)
        {
            SelectedTheme = (sender as ThemeOptions).Name;
            continueBtn.IsEnabled = true;
        }
    }
}
