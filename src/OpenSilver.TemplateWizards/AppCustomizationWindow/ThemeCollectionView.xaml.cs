using OpenSilver.TemplateWizards.AppCustomizationWindow.Models;
using System;
using System.Windows.Controls;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow
{
    /// <summary>
    /// Interaction logic for ThemeCollectionView.xaml
    /// </summary>
    public partial class ThemeCollectionView : UserControl
    {
        public event EventHandler SelectionChanged;
        public ThemeCollectionView()
        {
            InitializeComponent();
            themeList.ItemsSource = ThemeOptions.GeThemes();
            themeList.SelectionChanged += ThemeList_SelectionChanged;
        }

        private void ThemeList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            SelectionChanged?.Invoke(themeList.SelectedItem, e);
        }
    }
}
