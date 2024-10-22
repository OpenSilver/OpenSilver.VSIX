namespace OpenSilver.TemplateWizards
{
    public enum TemplateType
    {
        Empty,
        Business
    }

    public enum BusinessTemplateType
    {
        Modern,
        Migration
    }

    public enum PlatformType
    {
        Web,
        Simulator,
        Windows,
        Android,
        iOS,
        macOS
    }

    public enum DatabaseType
    {
        Sqlite,
        SqlServer,
        PostgreSQL
    }

    public enum FrameworkVersion
    {
        Net7 = 7,
        Net8 = 8,
        Net9 = 9
    }

    public enum Language
    {
        CSharp,
        VisualBasic,
        FSharp
    }
    public enum LanguageCode
    {
        CS,
        VB,
        FS
    }
}