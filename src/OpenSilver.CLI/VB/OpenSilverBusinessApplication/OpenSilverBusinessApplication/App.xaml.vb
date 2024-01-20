Imports System.Windows
Imports System.Net
Imports OpenRiaServices.DomainServices.Client
Imports OpenRiaServices.DomainServices.Client.Web
Imports OpenRiaServices.DomainServices.Client.ApplicationServices
Imports OpenSilverBusinessApplication.Web

Partial Public Class App
    Inherits Application

    Public Sub New()

        Current.Host.Settings.DefaultSoapCredentialsMode = CredentialsMode.Enabled

        AddHandler Me.Startup, AddressOf Me.Application_Startup
        AddHandler Me.UnhandledException, AddressOf Me.Application_UnhandledException

        InitializeComponent()

        Dim webContext As WebContext = New WebContext()
        webContext.Authentication = New FormsAuthentication()
        'webContext.Authentication = New WindowsAuthentication()
        Me.ApplicationLifetimeObjects.Add(webContext)

        DomainContext.DomainClientFactory = New WebAssemblySoapDomainClientFactory() With
        {
            .ServerBaseUri = New Uri("https://localhost:44350/")
        }

    End Sub

    Private Sub Application_Startup(sender As Object, e As StartupEventArgs)
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