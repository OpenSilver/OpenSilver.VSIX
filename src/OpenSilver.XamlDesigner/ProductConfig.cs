using Userware.XamlDesigner;
using Userware.XamlDesigner.Data.Primitives;

namespace OpenSilver.XamlDesigner
{
    internal class ProductConfig : IProductConfig
    {
        public string Name => "OpenSilver";

        public string Slug => "opensilver";

        public TargetPlatforms TargetPlatform => TargetPlatforms.OpenSilver;
    }
}
