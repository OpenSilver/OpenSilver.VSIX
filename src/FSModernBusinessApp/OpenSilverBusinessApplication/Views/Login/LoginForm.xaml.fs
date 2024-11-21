namespace $ext_safeprojectname$

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open System.ComponentModel.DataAnnotations
open OpenRiaServices.Client.Authentication
open DataFieldExtensions

type LoginForm() as this =
    inherit LoginFormXaml()

    let mutable parentWindow: ParentRegistrationWindow option = None
    let loginInfo = LoginInfo()
    let mutable userNameTextBox: TextBox option = None

    do
        this.InitializeComponent()
        // Set the DataContext of this control to the LoginInfo instance to allow for easy binding.
        this.DataContext <- loginInfo

    /// Sets the parent window for the current LoginForm.
    member this.SetParentWindow(window: ParentRegistrationWindow) =
        parentWindow <- Some window

    /// Handles DataForm.AutoGeneratingField to provide the PasswordAccessor.
    member this.LoginForm_AutoGeneratingField(sender: obj, e: DataFormAutoGeneratingFieldEventArgs) =
        match e.PropertyName with
        | "UserName" ->
            userNameTextBox <- Some (e.Field.Content :?> TextBox)
        | "Password" ->
            let passwordBox = PasswordBox()
            replaceTextBox e.Field passwordBox PasswordBox.PasswordProperty
            //e.Field.ReplaceTextBox(passwordBox, PasswordBox.PasswordProperty)
            loginInfo.PasswordAccessor <- Some(Func<string>(fun () -> passwordBox.Password))
        | _ -> ()

    /// Completion handler for a LoginOperation.
    member this.LoginOperation_Completed(loginOperation: LoginOperation) =
        if loginOperation.LoginSuccess then
            parentWindow |> Option.iter (fun window -> window.DialogResult <- Nullable true)
        elif loginOperation.HasError then
            ErrorWindow.Show(loginOperation.Error)
            loginOperation.MarkErrorAsHandled()
        elif not loginOperation.IsCanceled then
            loginInfo.ValidationErrors.Add(ValidationResult("Bad User Name Or Password", [| "UserName"; "Password" |]))

    /// Submits the LoginOperation to the server
    member this.LoginButton_Click(sender: obj, e: EventArgs) =
        // Validate the form before submission
        if this.loginForm.ValidateItem() then
            let loginOperation = WebContext.Current.Authentication.Login(loginInfo.ToLoginParameters(), Action<LoginOperation>(this.LoginOperation_Completed), null)
            loginInfo.CurrentLoginOperation <- Some(loginOperation)
            match parentWindow with
            | Some window -> window.AddPendingOperation(loginOperation)
            | None -> ()

    /// Switches to the registration form.
    member this.RegisterNow_Click(sender: obj, e: RoutedEventArgs) =
        parentWindow |> Option.iter (fun window -> window.NavigateToRegistration())

    /// Cancels the login operation if it is in progress and cancellable, otherwise closes the window.
    member this.CancelButton_Click(sender: obj, e: EventArgs) =
        match loginInfo.CurrentLoginOperation with
        | Some operation when operation.CanCancel -> operation.Cancel()
        | _ -> parentWindow |> Option.iter (fun window -> window.DialogResult <- Nullable false)

    /// Maps Esc to the cancel button and Enter to the OK button.
    member this.LoginForm_KeyDown(sender: obj, e: KeyEventArgs) =
        match e.Key with
        | Key.Escape -> this.CancelButton_Click(sender, e)
        | Key.Enter when this.loginButton.IsEnabled -> this.LoginButton_Click(sender, e)
        | _ -> ()

    /// Sets focus to the user name text box.
    member this.SetInitialFocus() =
        userNameTextBox |> Option.iter (fun tb -> tb.Focus() |> ignore )