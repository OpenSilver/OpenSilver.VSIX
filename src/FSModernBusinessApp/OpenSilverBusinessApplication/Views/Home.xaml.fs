namespace $ext_safeprojectname$

open System.Windows.Navigation

type Home() as this =
    inherit HomeXaml()

    do
        this.InitializeComponent()

    // Executes when the user navigates to this page.
    override this.OnNavigatedTo(e: NavigationEventArgs) =
        ()