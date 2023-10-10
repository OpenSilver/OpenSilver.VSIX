using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenSilver.XamlDesigner
{
    internal static class GuidList
    {
        public const string guidEditorPkgString = "589a0204-d2d0-4e8d-a341-22ca39db8eda";
        public const string guidEditorFactoryString = "8b35c9f1-1603-46b5-88cd-75b25e3504cd";

        public static readonly Guid guidEditorFactory = new Guid(guidEditorFactoryString);
    }
}
