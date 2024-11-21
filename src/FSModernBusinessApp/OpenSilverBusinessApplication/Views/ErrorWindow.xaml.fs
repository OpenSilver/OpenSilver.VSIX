namespace $ext_safeprojectname$

open System
open System.Windows

type ErrorWindow (message: string option, uri: Uri option, ex: Exception option) as this =
    inherit ErrorWindowXaml()

    do
        this.InitializeComponent()
        match message, uri, ex with
        | Some msg, _, _ ->
            this.ErrorTextBox.Text <- msg
        | None, Some uri, Some ex ->
            this.ErrorTextBox.Text <- sprintf "Problem loading page: '%s'%s%s" (uri.ToString()) Environment.NewLine (ex.ToString())
        | None, Some uri, None ->
            this.ErrorTextBox.Text <- sprintf "Problem loading page: '%s'" (uri.ToString())
        | None, None, Some ex ->
            this.ErrorTextBox.Text <- ex.ToString()
        | _ -> ()

    static member Show(ex: Exception) =
        let errorWindow = ErrorWindow(None, None, Some ex)
        errorWindow.Show()

    static member Show(uri: Uri, ?ex: Exception) =
        let errorWindow = ErrorWindow(None, Some uri, ex)
        errorWindow.Show()

    static member Show(message: string, ?details: string) =
        let fullMessage = message + Environment.NewLine + Environment.NewLine + (defaultArg details "")
        let errorWindow = ErrorWindow(Some fullMessage, None, None)
        errorWindow.Show()

    member this.OKButton_Click(sender: obj, e: RoutedEventArgs) =
        this.DialogResult <- Nullable true