Imports System.Net.Http
Imports System.Windows
Imports Microsoft.Extensions.Configuration
Imports Microsoft.Extensions.DependencyInjection
Imports OpenRiaServices.Client
Imports OpenRiaServices.Client.Authentication
Imports $ext_safeprojectname$.Services

Partial Public Class App
    Inherits Application

    Private Shared _services As IServiceProvider
    Private Shared _configuration As IConfiguration

    Public Shared ReadOnly Property Services As IServiceProvider
        Get
            Return _services
        End Get
    End Property

    Public Shared ReadOnly Property Configuration As IConfiguration
        Get
            Return _configuration
        End Get
    End Property

    Public Sub New()

        AddHandler Me.Startup, AddressOf Me.Application_Startup
        AddHandler Me.UnhandledException, AddressOf Me.Application_UnhandledException

        InitializeComponent()
    End Sub

    Public Sub New(serviceProvider As IServiceProvider, configuration As IConfiguration)

        Me.New()

        _services = serviceProvider
        _configuration = configuration
    End Sub

    Private Sub InitializeOpenRiaServices()

        Dim httpClientFactory = Services?.GetService(Of IHttpClientFactory)()
        Dim serverBaseUri = Configuration("ServiceUrl")

        DomainContext.DomainClientFactory = New OpenRiaServices.Client.DomainClients.BinaryHttpDomainClientFactory(
                                New Uri(serverBaseUri, UriKind.Absolute),
                                Function() If(httpClientFactory?.CreateClient("openria"), New HttpClient()))

        Dim webContext = New WebContext() With
        {
            .Authentication = New FormsAuthentication() With
            {
                .DomainContext = New AuthenticationContext()
            }
        }

    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs)

        InitializeOpenRiaServices()

        ' This will enable you to bind controls in XAML to WebContext.Current properties.
        Me.Resources.Add("WebContext", WebContext.Current)

        ' This will automatically authenticate a user when using Windows authentication or when the user chose "Keep me signed in" on a previous login attempt.
        WebContext.Current.Authentication.LoadUser(AddressOf Me.Application_UserLoaded, Nothing)

        Me.RootVisual = New MainPage()

    End Sub

    ''' <summary>
    ''' Invoked when the <see cref="LoadUserOperation"/> completes.
    ''' Use this event handler to switch from the "loading UI" you created in <see cref="InitializeRootVisual"/> to the "application UI".
    ''' </summary>
    Private Sub Application_UserLoaded(operation As LoadUserOperation)

        If operation.HasError Then
            ErrorWindow.Show(operation.Error)
            operation.MarkErrorAsHandled()
        End If

    End Sub

    Private Sub Application_UnhandledException(sender As Object, e As ApplicationUnhandledExceptionEventArgs)

        e.Handled = True
        ErrorWindow.Show(e.ExceptionObject)

    End Sub

End Class