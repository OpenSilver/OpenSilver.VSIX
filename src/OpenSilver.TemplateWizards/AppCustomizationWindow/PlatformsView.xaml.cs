using System.Windows;
using System.Windows.Controls;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for PlatformsView.xaml
    /// </summary>
    public partial class PlatformsView : ListBox
    {
        public static readonly DependencyProperty ShowAllPlatformsProperty =
            DependencyProperty.Register(nameof(ShowAllPlatforms), typeof(bool), typeof(PlatformsView), new PropertyMetadata(false));

        public PlatformsView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When false, only the always-selected platform (Web) is shown.
        /// </summary>
        public bool ShowAllPlatforms
        {
            get => (bool)GetValue(ShowAllPlatformsProperty);
            set => SetValue(ShowAllPlatformsProperty, value);
        }
    }
}
