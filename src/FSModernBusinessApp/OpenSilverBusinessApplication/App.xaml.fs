namespace $ext_safeprojectname$

open System
open System.Net.Http
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open System.Windows
open OpenRiaServices.Client
open OpenRiaServices.Client.DomainClients
open OpenRiaServices.Client.Authentication
open $ext_safeprojectname$.Services

type App = class
    inherit AppXaml

    static let mutable _services : IServiceProvider = null
    static let mutable _configuration : IConfiguration = null

    new () as this = {} then

        this.UnhandledException.Add(fun e ->
            e.Handled <- true
            ErrorWindow.Show(e.ExceptionObject)
        )
        this.Startup.Add(fun e ->

            this.InitializeOpenRiaServices()

            // This will enable you to bind controls in XAML to WebContext.Current properties.
            this.Resources.Add("WebContext", WebContext.Current)

            // This will automatically authenticate a user when using Windows authentication or when the user chose "Keep me signed in" on a previous login attempt.
            WebContext.Current.Authentication.LoadUser(
                (fun operation ->
                    if operation.HasError then
                        ErrorWindow.Show(operation.Error)
                        operation.MarkErrorAsHandled()
                ),
                ()
            ) |> ignore

            let mainPage = MainPage()
            this.RootVisual <- mainPage
            Window.Current.Content <- mainPage;

            |> ignore
        )
        this.InitializeComponent()
        // Enter construction logic here...

    new (serviceProvider: IServiceProvider, configuration: IConfiguration) =
        App()
        then
            _services <- serviceProvider
            _configuration <- configuration

    member this.InitializeOpenRiaServices(): unit =
        let serverBaseUri = _configuration.["ServiceUrl"]
        let httpClientFactory = 
            match _services.GetService<IHttpClientFactory>() with
            | null -> None
            | factory -> Some(factory)

        DomainContext.DomainClientFactory <- 
            new BinaryHttpDomainClientFactory(
                new Uri(serverBaseUri, UriKind.Absolute),
                (fun () -> 
                    match httpClientFactory with
                    | Some(factory) -> factory.CreateClient("openria")
                    | None -> new HttpClient())
            )

        let webContext = new WebContext()
        webContext.Authentication <- 
            new FormsAuthentication(
                DomainContext = new AuthenticationContext()
            )

end