using System.Windows;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    public partial class InstallMauiWindow : Window
    {
        public InstallMauiWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}

