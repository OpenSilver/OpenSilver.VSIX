using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow.Models
{
    public class ThemeOptions
    {
        public string ThumbnailUri { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string BackgroundColor { get; set; }

        public static IEnumerable<ThemeOptions> GeThemes()
        {
            var themes = new List<ThemeOptions>();
            themes.Add(new ThemeOptions { Name="Light",Title = "Modern - Light", ThumbnailUri = "/OpenSilver.TemplateWizards;component/Assets/Images/light_theme.png", BackgroundColor = "#FFFFFF" });
            themes.Add(new ThemeOptions { Name="Dark",Title = "Modern - Dark", ThumbnailUri = "/OpenSilver.TemplateWizards;component/Assets/Images/dark_theme.png", BackgroundColor = "#111111" });
            themes.Add(new ThemeOptions { Name="Classic",Title = "Classic - Silverlight", ThumbnailUri = "/OpenSilver.TemplateWizards;component/Assets/Images/classic_theme.png", BackgroundColor = "#FFFFFF" });
            return themes;
        }
    }
}
