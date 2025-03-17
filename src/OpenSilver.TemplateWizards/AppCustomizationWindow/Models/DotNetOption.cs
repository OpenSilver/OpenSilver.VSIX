using OpenSilver.TemplateWizards.Shared;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow.Models
{
    public class DotNetOption
    {
        public string Title { get; set; }
        public string SupportDescription { get; set; }
        public string Notes { get; set; }
        public DotNetVersion Version { get; set; }
    }
}
