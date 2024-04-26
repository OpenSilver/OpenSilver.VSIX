using System;
using System.Windows;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class AppConfigurationWindow : Window
    {
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
                        return DotNetVersion.Net6;
                    case 1:
                        return DotNetVersion.Net7;
                    case 2:
                        return DotNetVersion.Net8;
                    default:
                        throw new InvalidOperationException("Error retrieving selected .NET version");
                }
            }
        }

        public AppConfigurationWindow(string openSilverType)
        {
            InitializeComponent();

            if (openSilverType == "Library")
            {
                DotNetVersionStackPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
