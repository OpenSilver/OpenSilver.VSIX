namespace OpenSilver.TemplateWizards.AppCustomizationWindow.Models
{
    public class TargetPlatform
    {
        public string Title { get; set; }
        public string Framework { get; set; }
        public string ThumbnailUri { get; set; }
        public double Opacity { get; set; } = 0.6;
        public bool IsAlwaysSelected { get; set; }
        public object Tag { get; set; }
    }
}
