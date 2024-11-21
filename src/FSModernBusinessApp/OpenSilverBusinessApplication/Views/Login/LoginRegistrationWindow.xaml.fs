namespace $ext_safeprojectname$

open System
open System.Collections.Generic
open System.ComponentModel
open System.Windows
open OpenRiaServices.Client

type LoginRegistrationWindow() as this =
    inherit LoginRegistrationWindowXaml()

    let possiblyPendingOperations = new List<OperationBase>()

    do
        this.InitializeComponent()
        this.registrationForm.SetParentWindow(this)
        this.loginForm.SetParentWindow(this)
        this.NavigateToLogin()

    /// Ensures the visual state and focus are correct when the window is opened.
    override this.OnOpened() =
        base.OnOpened()
        this.NavigateToLogin()

    /// Updates the window title according to which panel (registration / login) is currently being displayed.
    member private this.UpdateTitle(sender: obj, eventArgs: EventArgs) =
        this.Title <- if this.registrationForm.Visibility = Visibility.Visible then "Register" else "Login"

    /// Notifies the window that it can only close if the operation is finished or can be cancelled.
    override this.AddPendingOperation(operation: OperationBase) =
        possiblyPendingOperations.Add(operation)

    /// Changes the VisualStateManager to the "AtLogin" state.
    override this.NavigateToLogin() =
        this.loginForm.Visibility <- Visibility.Visible
        this.registrationForm.Visibility <- Visibility.Collapsed
        this.UpdateTitle(this, EventArgs.Empty)

    /// Changes the VisualStateManager to the "AtRegistration" state.
    override this.NavigateToRegistration() =
        this.loginForm.Visibility <- Visibility.Collapsed
        this.registrationForm.Visibility <- Visibility.Visible
        this.UpdateTitle(this, EventArgs.Empty)

    /// Prevents the window from closing while there are operations in progress.
    member this.LoginWindow_Closing(sender: obj, eventArgs: CancelEventArgs) =
        for operation in possiblyPendingOperations do
            if not operation.IsComplete then
                if operation.CanCancel then
                    operation.Cancel()
                else
                    eventArgs.Cancel <- true