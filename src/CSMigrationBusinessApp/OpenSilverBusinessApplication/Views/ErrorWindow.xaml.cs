using System;
using System.Windows;
using System.Windows.Controls;

namespace $ext_safeprojectname$
{
    public partial class ErrorWindow : ChildWindow
    {
        public static void Show(Exception ex)
        {
            var errorWindow = new ErrorWindow(ex);
            errorWindow.Show();
        }

        public static void Show(Uri uri, Exception ex = null)
        {
            var errorWindow = new ErrorWindow(uri, ex);
            errorWindow.Show();
        }

        public static void Show(string message)
        {
            var errorWindow = new ErrorWindow(message);
            errorWindow.Show();
        }

        private ErrorWindow(Exception ex)
        {
            InitializeComponent();
            if (ex != null)
            {
                ErrorTextBox.Text = ex.ToString();
            }
        }

        private ErrorWindow(Uri uri, Exception ex = null)
        {
            InitializeComponent();
            if (uri != null)
            {
                ErrorTextBox.Text = $"Problem loading page: '{uri}'{Environment.NewLine}{ex}";
            }
        }

        private ErrorWindow(string message)
        {
            InitializeComponent();
            ErrorTextBox.Text = message;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}