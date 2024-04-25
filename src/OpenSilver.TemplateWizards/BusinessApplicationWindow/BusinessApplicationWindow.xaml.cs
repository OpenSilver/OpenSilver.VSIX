using System;
using System.Collections.Generic;
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
    public partial class BusinessApplicationWindow : Window
    {
        public NewProjectViewModel ViewModel { get; set; } = new NewProjectViewModel();

        public BusinessApplicationWindow()
        {
            InitializeComponent();

            DataContext = ViewModel;
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

    public enum FrameworkVersion
    {
        Net7 = 7,
        Net8 = 8,
        Net9 = 9
    }

    public enum Backend
    {
        Migration,
        Modern
    }

    public enum Database
    {
        Sqlite,
        SqlServer,
        PostgreSQL
    }

    public enum LanguageCode
    { 
        CS,
        VB,
        FS
    }

    public class NewProjectViewModel : INotifyPropertyChanged
    {
        private FrameworkVersion _frameworkVersion = FrameworkVersion.Net8;
        private Backend _backend = Backend.Modern;
        private Database _database = Database.Sqlite;
        private LanguageCode _languageCode = LanguageCode.CS;

        public Backend Backend
        {
            get => _backend;
            set => SetPropertyValue(ref _backend, value);
        }

        public Database Database
        {
            get => _database;
            set => SetPropertyValue(ref _database, value);
        }

        public FrameworkVersion FrameworkVersion
        {
            get => _frameworkVersion;
            set => SetPropertyValue(ref _frameworkVersion, value);
        }
        public LanguageCode LanguageCode
        {
            get => _languageCode;
            set => SetPropertyValue(ref _languageCode, value);
        }

        private void SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!EqualityComparer<T>.Default.Equals(field, value))
            {
                field = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
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
}