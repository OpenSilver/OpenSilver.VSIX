
using System;

namespace OpenSilver.TemplateWizards.Shared
{
    /// <summary>
    /// Represents the platforms supported by the MAUI Hybrid application.
    /// </summary>
    [Flags]
    public enum MauiHybridPlatform
    {
        None = 0,
        iOS = 1,
        Android = 2,
        Windows = 4,
        Mac = 8
    }
}
