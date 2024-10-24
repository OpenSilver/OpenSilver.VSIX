using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace OpenSilver.TemplateWizards
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class OpenSilverAppWindow : Window
    {
        public NewAppViewModel ViewModel { get; set; } = new NewAppViewModel();

        public OpenSilverAppWindow()
        {
            InitializeComponent();

            DataContext = ViewModel;
        }

        public OpenSilverAppWindow(Language language) : this()
        {
            ViewModel.Language = language;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            mainTabControl.SelectedIndex--;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            mainTabControl.SelectedIndex++;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            btnMaximize.Visibility = Visibility.Collapsed;
            btnRestore.Visibility = Visibility.Visible;
        }

        private void btnRestore_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
            btnMaximize.Visibility = Visibility.Visible;
            btnRestore.Visibility = Visibility.Collapsed;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }

    public class PlatformItem : ViewModelBase
    {
        private bool _isSelected = false;
        private bool _isEnabled = true;
        private PlatformType _platformType;
        private string _description;

        public PlatformType PlatformType
        {
            get => _platformType;
            set => SetPropertyValue(ref _platformType, value);
        }

        public string Description
        {
            get => _description;
            set => SetPropertyValue(ref _description, value);
        }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetPropertyValue(ref _isSelected, value);
        }
        public bool IsEnabled
        {
            get => _isEnabled;
            set => SetPropertyValue(ref _isEnabled, value);
        }

    }

    public class StringToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString() == parameter?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is true ? $"{parameter}" : Binding.DoNothing;
        }
    }

    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return null;
        }
    }



}