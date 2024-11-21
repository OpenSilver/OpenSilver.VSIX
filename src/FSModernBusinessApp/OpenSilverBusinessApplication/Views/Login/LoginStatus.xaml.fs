namespace $ext_safeprojectname$

open System
open System.Globalization
open System.Windows
open System.ComponentModel
open OpenRiaServices.Client.Authentication

type LoginStatus() as this =
    inherit LoginStatusXaml()

    do
        // Initialize the component
        this.InitializeComponent()

        this.Loaded.Add(fun e ->

            if DesignerProperties.GetIsInDesignMode(this) then
                // If in design mode, go to "loggedOut" visual state
                VisualStateManager.GoToState(this, "loggedOut", false) |> ignore
            else
                // Set DataContext to the current WebContext
                this.DataContext <- WebContext.Current

                // Subscribe to authentication events
                WebContext.Current.Authentication.LoggedIn.AddHandler(fun sender e -> this.Authentication_LoggedIn(sender, e))
                WebContext.Current.Authentication.LoggedOut.AddHandler(fun sender e -> this.Authentication_LoggedOut(sender, e))

                // Update the login state
                this.UpdateLoginState()
        )

    // Handles the Logout button click event
    member this.LogoutButton_Click(sender: obj, e: RoutedEventArgs) =
        WebContext.Current.Authentication.Logout(
            (fun logoutOperation ->
                if logoutOperation.HasError then
                    ErrorWindow.Show(logoutOperation.Error)
                    logoutOperation.MarkErrorAsHandled()
            ),
            userState = null
        ) |> ignore

    // Handles the Login button click event
    member this.LoginButton_Click(sender: obj, e: RoutedEventArgs) =
        let loginWindow = LoginRegistrationWindow()
        loginWindow.Show()

    // Handles the LoggedIn event
    member this.Authentication_LoggedIn(sender: obj, e: AuthenticationEventArgs) =
        this.UpdateLoginState()

    // Handles the LoggedOut event
    member this.Authentication_LoggedOut(sender: obj, e: AuthenticationEventArgs) =
        this.UpdateLoginState()

    // Updates the visual state and welcome text based on the login state
    member private this.UpdateLoginState() =

        if WebContext.Current.User.IsAuthenticated then
            this.welcomeText.Text <- String.Format(
                CultureInfo.CurrentUICulture,
                "Welcome {0}",
                WebContext.Current.User.DisplayName
            )
        else
            this.welcomeText.Text <- "authenticating..."

        match WebContext.Current.Authentication with
        | :? WindowsAuthentication ->
            VisualStateManager.GoToState(this, "windowsAuth", true) |> ignore
        | _ ->
            VisualStateManager.GoToState(this, (if WebContext.Current.User.IsAuthenticated then "loggedIn" else "loggedOut"), true) |> ignore