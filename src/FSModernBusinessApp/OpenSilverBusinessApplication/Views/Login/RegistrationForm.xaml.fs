namespace $ext_safeprojectname$

open System
open System.Collections.Generic
open System.Linq
open System.Windows
open System.Windows.Controls
open System.Windows.Input
open System.Windows.Data
open $ext_safeprojectname$.Models
open $ext_safeprojectname$.Services
open OpenRiaServices.Client
open OpenRiaServices.Client.Authentication

type RegistrationForm() as this =
    inherit RegistrationFormXaml()

    let mutable parentWindow: ParentRegistrationWindow option = None
    let registrationData = RegistrationData()
    let userRegistrationContext = UserRegistrationContext()
    let mutable userNameTextBox: TextBox option = None

    do
        this.InitializeComponent()
        this.DataContext <- registrationData

    /// Sets the parent window for the current RegistrationForm.
    member this.SetParentWindow(window: ParentRegistrationWindow) =
        parentWindow <- Some window

    /// Wire up the Password and PasswordConfirmation accessors as fields get generated.
    member this.RegisterForm_AutoGeneratingField(dataForm: obj, e: DataFormAutoGeneratingFieldEventArgs) =
        e.Field.Mode <- DataFieldMode.AddNew
        match e.PropertyName with
        | "UserName" ->
            let tb = e.Field.Content :?> TextBox
            userNameTextBox <- Some tb
            tb.LostFocus.AddHandler(fun sender e -> this.UserNameLostFocus(sender, e))
        | "Password" ->
            let passwordBox = PasswordBox()
            DataFieldExtensions.replaceTextBox e.Field passwordBox PasswordBox.PasswordProperty
            //e.Field.ReplaceTextBox(passwordBox, PasswordBox.PasswordProperty)
            registrationData.PasswordAccessor <- Func<string>(fun () -> passwordBox.Password)
        | "PasswordConfirmation" ->
            let passwordConfirmationBox = PasswordBox()
            DataFieldExtensions.replaceTextBox e.Field passwordConfirmationBox PasswordBox.PasswordProperty
            //e.Field.ReplaceTextBox(passwordConfirmationBox, PasswordBox.PasswordProperty)
            registrationData.PasswordConfirmationAccessor <- Func<string>(fun () -> passwordConfirmationBox.Password)
        | "Question" ->
            let questionComboBox = ComboBox()
            questionComboBox.ItemsSource <- RegistrationForm.GetSecurityQuestions() 
            DataFieldExtensions.replaceTextBoxWithSetup
                e.Field 
                questionComboBox 
                ComboBox.SelectedItemProperty 
                (Some (Action<Binding>(fun binding -> binding.Converter <- TargetNullValueConverter())))
        | _ -> ()

    /// Callback for when the UserName TextBox loses focus.
    member private this.UserNameLostFocus(sender: obj, e: RoutedEventArgs) =
        registrationData.UserNameEntered((sender :?> TextBox).Text)

    /// Returns a list of security questions.
    static member GetSecurityQuestions() =
        [ "What is the name of your favorite childhood friend?"
          "What was your childhood nickname?"
          "What was the color of your first car?"
          "What was the make and model of your first car?"
          "In what city or town was your first job?"
          "Where did you vacation last year?"
          "What is your maternal grandmother's maiden name?"
          "What is your mother's maiden name?"
          "What is your pet's name?"
          "What school did you attend for sixth grade?"
          "In what year was your father born?" ]

    /// Submit the new registration.
    member this.RegisterButton_Click(sender: obj, e: RoutedEventArgs) =
        if this.registerForm.ValidateItem() then
            let operation = userRegistrationContext.CreateUser(
                registrationData,
                registrationData.Password,
                Action<InvokeOperation<IEnumerable<string>>>(this.RegistrationOperation_Completed),
                null)
            registrationData.CurrentOperation <- operation
            parentWindow |> Option.iter (fun window -> window.AddPendingOperation(operation))

    /// Completion handler for the registration operation.
    member private this.RegistrationOperation_Completed(operation: InvokeOperation<IEnumerable<string>>) =
        if not operation.IsCanceled then
            if operation.HasError then
                ErrorWindow.Show(operation.Error)
                operation.MarkErrorAsHandled()
            elif not (operation.Value.Any()) then
                let loginOperation = WebContext.Current.Authentication.Login(
                    registrationData.ToLoginParameters(),
                    Action<LoginOperation>(this.LoginOperation_Completed),
                    null)
                registrationData.CurrentOperation <- loginOperation
                parentWindow |> Option.iter (fun window -> window.AddPendingOperation(loginOperation))
            else
                ErrorWindow.Show("Errors have occurred: ", String.Join(Environment.NewLine, operation.Value |> Seq.map (fun e -> $"- {e}")))

    /// Completion handler for the login operation after a successful registration.
    member private this.LoginOperation_Completed(loginOperation: LoginOperation) =
        if not loginOperation.IsCanceled then
            parentWindow |> Option.iter (fun window -> window.DialogResult <- Nullable true)
            if loginOperation.HasError then
                ErrorWindow.Show(sprintf "Registration was successful but there was a problem while trying to log in with your credentials: %s" loginOperation.Error.Message)
                loginOperation.MarkErrorAsHandled()
            elif not loginOperation.LoginSuccess then
                ErrorWindow.Show("Registration was successful but there was a problem while trying to log in with your credentials: The user name or password is incorrect")

    /// Switches to the login window.
    member this.BackToLogin_Click(sender: obj, e: RoutedEventArgs) =
        parentWindow |> Option.iter (fun window -> window.NavigateToLogin())

    /// If a registration or login operation is in progress and is cancellable, cancel it. Otherwise, close the window.
    member this.CancelButton_Click(sender: obj, e: EventArgs) =
        match registrationData.CurrentOperation with
        | null -> parentWindow |> Option.iter (fun window -> window.DialogResult <- Nullable false)
        | operation when operation.CanCancel -> operation.Cancel()
        | _ -> parentWindow |> Option.iter (fun window -> window.DialogResult <- Nullable false)

    /// Maps Esc to the cancel button and Enter to the OK button.
    member this.RegistrationForm_KeyDown(sender: obj, e: KeyEventArgs) =
        match e.Key with
        | Key.Escape -> this.CancelButton_Click(sender, e)
        | Key.Enter when this.registerButton.IsEnabled -> this.RegisterButton_Click(sender, e)
        | _ -> ()

    /// Sets focus to the user name text box.
    member this.SetInitialFocus() =
        userNameTextBox |> Option.iter (fun tb -> tb.Focus() |> ignore)