using System.Collections.Generic;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow.Models
{
    public class ThemeOptions
    {
        public const string Classic = "Classic";
        public const string Dark = "Dark";
        public const string Light = "Light";

        public string ThumbnailUri { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string BackgroundColor { get; set; }

        public static IEnumerable<ThemeOptions> GeThemes()
        {
            var themes = new List<ThemeOptions>
            {
                new ThemeOptions { Name = Light, Title = "Modern - Light", ThumbnailUri = "/OpenSilver.TemplateWizards;component/Assets/Images/light_theme.png", BackgroundColor = "#FFFFFF" },
                new ThemeOptions { Name = Dark, Title = "Modern - Dark", ThumbnailUri = "/OpenSilver.TemplateWizards;component/Assets/Images/dark_theme.png", BackgroundColor = "#111111" },
                new ThemeOptions { Name = Classic, Title = "Classic - Silverlight", ThumbnailUri = "/OpenSilver.TemplateWizards;component/Assets/Images/classic_theme.png", BackgroundColor = "#FFFFFF" }
            };
            return themes;
        }
    }
}
