using OpenRiaServices.DomainServices.Client;
using OpenRiaServices.DomainServices.Client.ApplicationServices;
using $ext_safeprojectname$.Web;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;

namespace $ext_safeprojectname$
{
    public partial class App : Application
    {
        public App()
        {
            Current.Host.Settings.DefaultSoapCredentialsMode = CredentialsMode.Enabled;

            this.Startup += this.Application_Startup;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();

            WebContext webContext = new WebContext();
            webContext.Authentication = new FormsAuthentication();
            //webContext.Authentication = new WindowsAuthentication();
            this.ApplicationLifetimeObjects.Add(webContext);

            DomainContext.DomainClientFactory = new OpenRiaServices.DomainServices.Client.Web.WebAssemblySoapDomainClientFactory()
            {
                ServerBaseUri = new Uri("https://localhost:$ext_sslserverport$/"),
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // This will enable you to bind controls in XAML to WebContext.Current properties.
            this.Resources.Add("WebContext", WebContext.Current);

            // This will automatically authenticate a user when using Windows authentication or when the user chose "Keep me signed in" on a previous login attempt.
            WebContext.Current.Authentication.LoadUser(this.Application_UserLoaded, null);

            this.RootVisual = new MainPage();
        }

        /// <summary>
        /// Invoked when the <see cref="LoadUserOperation"/> completes.
        /// Use this event handler to switch from the "loading UI" you created in <see cref="InitializeRootVisual"/> to the "application UI".
        /// </summary>
        private void Application_UserLoaded(LoadUserOperation operation)
        {
            if (operation.HasError)
            {
                ErrorWindow.Show(operation.Error);
                operation.MarkErrorAsHandled();
            }
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ErrorWindow.Show(e.ExceptionObject);
        }
    }
}