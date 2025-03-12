using System;
using System.Windows.Controls;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for ThemeCollectionView.xaml
    /// </summary>
    public partial class ThemeCollectionView : UserControl
    {
        public ThemeCollectionView()
        {
            InitializeComponent();
            themeList.SelectionChanged += ThemeList_SelectionChanged;
        }

        public event EventHandler<ThemeOptions> SelectionChanged;

        public void Select(ThemeOptions theme)
        {
            themeList.SelectedItem = theme;
        }

        private void ThemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, (ThemeOptions)themeList.SelectedItem);
        }
    }
}
