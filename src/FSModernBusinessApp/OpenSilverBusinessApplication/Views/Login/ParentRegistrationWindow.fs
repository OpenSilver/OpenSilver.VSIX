namespace $ext_safeprojectname$

open OpenRiaServices.Client
open System.Windows.Controls

type ParentRegistrationWindow() as this =
    inherit ChildWindow()

    abstract member AddPendingOperation: OperationBase -> unit default this.AddPendingOperation(operation: OperationBase) = ()
    abstract member NavigateToRegistration: unit -> unit default this.NavigateToRegistration() = ()
    abstract member NavigateToLogin: unit -> unit default this.NavigateToLogin() = ()