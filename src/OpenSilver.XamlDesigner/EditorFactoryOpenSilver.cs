using EnvDTE;
using System;
using Userware.XamlDesigner.SplitXamlEditor;
using Userware.XamlDesigner;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace OpenSilver.XamlDesigner
{
    [Guid(GuidList.guidEditorFactoryString)]
    internal class EditorFactoryOpenSilver : BaseEditorFactory
    {
        private const string OpenSilver = "OpenSilver";
        private const string XRSharp = "XRSharp";
        private const string XamlForBlazor = "XamlForBlazor";

        public EditorFactoryOpenSilver(Package package) : base(package, new ProductConfig())
        {
        }

        protected override Guid EditorFactoryId => GuidList.guidEditorFactory;

        protected override bool IsSupportedFile(Project project)
        {
            var hasOpenSilverReference = project.FindReferencePath(OpenSilver) != null;
            var hasXrSharpReference = project.FindReferencePath(XRSharp) != null;
            var hasXamlForBlazorReference = project.FindReferencePath(XamlForBlazor) != null;

            return !hasXrSharpReference && !hasXamlForBlazorReference && hasOpenSilverReference;
        }
    }
}
