using OpenSilver.TemplateWizards.Shared;
using System.Windows.Controls;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for DotNetVersionSelector.xaml
    /// </summary>
    public partial class DotNetVersionSelector : UserControl
    {
        public DotNetVersionSelector()
        {
            InitializeComponent();
        }

        public DotNetVersion DotNetVersion => (DotNetVersion)DotNetVersionComboBox.SelectedValue;

        public event SelectionChangedEventHandler SelectionChanged;

        private void DotNetVersionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
