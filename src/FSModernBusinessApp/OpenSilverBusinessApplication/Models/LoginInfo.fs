namespace $ext_safeprojectname$

open System
open System.ComponentModel.DataAnnotations
open OpenRiaServices.Client
open OpenRiaServices.Client.Authentication

type LoginInfo() as this =
    inherit ComplexObject()

    let mutable userName = ""
    let mutable rememberMe = false
    let mutable currentLoginOperation: LoginOperation option = None

    /// Gets and sets the user name.
    [<Display(Name = "User name")>]
    [<Required>]
    member this.UserName
        with get() = userName
        and set(value) =
            if userName <> value then
                this.ValidateProperty("UserName", value)
                userName <- value
                this.RaisePropertyChanged("UserName")

    /// Gets or sets a function that returns the password.
    [<Display(AutoGenerateField = false)>]
    member val PasswordAccessor: Func<string> option = None with get, set

    /// Gets and sets the password.
    [<Required>]
    member this.Password
        with get() = 
            match this.PasswordAccessor with
            | Some accessor -> accessor.Invoke()
            | None -> ""
        and set(value : string) =
            this.ValidateProperty("Password", value)
            this.RaisePropertyChanged("Password")

    /// Gets and sets the value indicating whether the user's authentication information should be recorded for future logins.
    [<Display(Name = "Keep me signed in")>]
    member this.RememberMe
        with get() = rememberMe
        and set(value) =
            if rememberMe <> value then
                this.ValidateProperty("RememberMe", value)
                rememberMe <- value
                this.RaisePropertyChanged("RememberMe")

    /// Gets or sets the current login operation.
    [<Display(AutoGenerateField = false)>]
    member this.CurrentLoginOperation
        with get() = currentLoginOperation
        and set(value) =
            if currentLoginOperation <> value then
                match currentLoginOperation with
                | Some operation -> operation.Completed.RemoveHandler(fun _ _ -> this.CurrentLoginOperationChanged())
                | None -> ()
                
                currentLoginOperation <- value
                
                match currentLoginOperation with
                | Some operation -> operation.Completed.AddHandler(fun _ _ -> this.CurrentLoginOperationChanged())
                | None -> ()
                
                this.CurrentLoginOperationChanged()

    /// Gets a value indicating whether the user is presently being logged in.
    [<Display(AutoGenerateField = false)>]
    member this.IsLoggingIn =
        match currentLoginOperation with
        | Some operation when not operation.IsComplete -> true
        | _ -> false

    /// Gets a value indicating whether the user can presently log in.
    [<Display(AutoGenerateField = false)>]
    member this.CanLogIn = not this.IsLoggingIn

    /// Raises operation-related property change notifications when the current login operation changes.
    member private this.CurrentLoginOperationChanged() =
        this.RaisePropertyChanged("IsLoggingIn")
        this.RaisePropertyChanged("CanLogIn")

    /// Creates a new LoginParameters instance using the data stored in this entity.
    member this.ToLoginParameters() =
        LoginParameters(this.UserName, this.Password, this.RememberMe, null)