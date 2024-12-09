using OpenSilver.TemplateWizards.AppCustomizationWindow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
