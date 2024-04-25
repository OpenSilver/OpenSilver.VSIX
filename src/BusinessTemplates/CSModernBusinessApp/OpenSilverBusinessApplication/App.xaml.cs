﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenRiaServices.Client;
using OpenRiaServices.Client.Authentication;
using $ext_safeprojectname$.Services;
using System;
using System.Net.Http;
using System.Windows;

namespace $ext_safeprojectname$
{
    public partial class App : Application
    {
        private static IServiceProvider _services;
        private static IConfiguration _configuration;
        public static IServiceProvider Services => _services;
        public static IConfiguration Configuration => _configuration;

        public App()
        {
            this.Startup += this.Application_Startup;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();
        }

        public App(IServiceProvider serviceProvider, IConfiguration configuration) : this()
        {
            _services = serviceProvider;
            _configuration = configuration;

            InitializeOpenRiaServices();
        }

        private void InitializeOpenRiaServices()
        {
            var httpClientFactory = Services?.GetService<IHttpClientFactory>();
            var serverBaseUri = Configuration["ServiceUrl"];

            DomainContext.DomainClientFactory = new OpenRiaServices.Client.DomainClients.BinaryHttpDomainClientFactory(
                new Uri(serverBaseUri, UriKind.Absolute),
                () => httpClientFactory?.CreateClient("openria") ?? new HttpClient());

            var webContext = new WebContext()
            {
                Authentication = new FormsAuthentication()
                {
                    DomainContext = new AuthenticationContext()
                }
            };
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // This will enable you to bind controls in XAML to WebContext.Current properties.
            this.Resources.Add(nameof(WebContext), WebContext.Current);

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