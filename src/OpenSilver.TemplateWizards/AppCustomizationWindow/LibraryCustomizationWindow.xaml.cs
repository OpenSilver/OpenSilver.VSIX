using OpenSilver.TemplateWizards.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for LibraryCustomizationWindow.xaml
    /// </summary>
    public partial class LibraryCustomizationWindow : Window
    {
        public LibraryCustomizationWindow()
        {
            InitializeComponent();
        }

        public DotNetVersion DotNetVersion => VersionSelector.DotNetVersion;

        private void ButtonContinue_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
