namespace OpenSilverApplication
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Windows
open System.Windows.Controls

type App = class
    inherit AppXaml
    
    new () as this = {} then
        this.InitializeComponent()
        // Enter construction logic here...
//#if (!isWpf)

        let mainPage = new MainPage()
        this.RootVisual <- mainPage;
//#endif

end