namespace $ext_safeprojectname$

open System.Windows
open System.Windows.Controls
open System.Windows.Navigation

type MainPage() as this =
    inherit MainPageXaml()
    
    // For code examples, refer to the OpenSilver Showcase app at: https://opensilver.net/gallery/
    do
        this.InitializeComponent()
        // Enter construction logic here...

    member this.ContentFrame_Navigated(sender : obj, e : NavigationEventArgs) =
        for child in this.LinksStackPanel.Children do
            match child with
            | :? HyperlinkButton as hb when hb.NavigateUri <> null ->
                if hb.NavigateUri.ToString() = e.Uri.ToString() then
                    VisualStateManager.GoToState(hb, "ActiveLink", true) |> ignore
                else
                    VisualStateManager.GoToState(hb, "InactiveLink", true) |> ignore
            | _ -> ()

    member this.ContentFrame_NavigationFailed(sender : obj, e : NavigationFailedEventArgs) =
        e.Handled <- true
        ErrorWindow.Show(e.Uri, e.Exception)