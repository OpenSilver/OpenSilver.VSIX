namespace OpenSilver.TemplateWizards.Shared
{
    /// <summary>
    /// Represents the style of the initial files generated for an application project.
    /// </summary>
    public enum TemplateKind
    {
        /// <summary>
        /// MainWindow and Application.StartupUri.
        /// </summary>
        Wpf,

        /// <summary>
        /// MainPage and Application.RootVisual.
        /// </summary>
        Silverlight,
    }
}
