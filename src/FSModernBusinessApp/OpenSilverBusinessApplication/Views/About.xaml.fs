namespace $ext_safeprojectname$

open System.Windows.Navigation

type About() as this =
    inherit AboutXaml()

    do
        this.InitializeComponent()

    // Executes when the user navigates to this page.
    override this.OnNavigatedTo(e: NavigationEventArgs) =
        ()