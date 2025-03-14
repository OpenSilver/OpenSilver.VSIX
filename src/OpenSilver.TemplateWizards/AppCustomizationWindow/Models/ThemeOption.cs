using System.Collections.Generic;

namespace OpenSilver.TemplateWizards.AppCustomizationWindow.Models
{
    public class ThemeOptions
    {
        public static ThemeOptions Light { get; } =
            new ThemeOptions(nameof(Light), "Modern Light Theme", "/OpenSilver.TemplateWizards;component/Assets/Images/light_theme.png", "#FFFFFF");

        public static ThemeOptions Dark { get; } =
            new ThemeOptions(nameof(Dark), "Modern Dark Theme", "/OpenSilver.TemplateWizards;component/Assets/Images/dark_theme.png", "#111111");

        public static ThemeOptions Classic { get; } =
            new ThemeOptions(nameof(Classic), "Classic Silverlight Theme", "/OpenSilver.TemplateWizards;component/Assets/Images/classic_theme.png", "#FFFFFF");

        public static IEnumerable<ThemeOptions> Themes { get; } = new ThemeOptions[] { Light, Dark, Classic };

        private ThemeOptions(string name, string title, string thumbnailUri, string backgroundColor)
        {
            Name = name;
            Title = title;
            ThumbnailUri = thumbnailUri;
            BackgroundColor = backgroundColor;
        }

        public string ThumbnailUri { get; }
        public string Name { get; }
        public string Title { get; }
        public string BackgroundColor { get; }

        public override string ToString() => Name;
    }
}
