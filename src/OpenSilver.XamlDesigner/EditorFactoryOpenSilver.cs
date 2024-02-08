using EnvDTE;
using System;
using Userware.XamlDesigner.SplitXamlEditor;
using Microsoft.VisualStudio.Shell;
using System.Runtime.InteropServices;

namespace OpenSilver.XamlDesigner
{
    [Guid(GuidList.guidEditorFactoryString)]
    internal class EditorFactoryOpenSilver : BaseEditorFactory
    {
        private const string OpenSilver = "OpenSilver";
        private const string XRSharp = "XRSharp";

        public EditorFactoryOpenSilver(Package package) : base(package, new ProductConfig())
        {
        }

        protected override Guid EditorFactoryId => GuidList.guidEditorFactory;

        protected override bool IsSupportedFile(Project project)
        {
            return CheckNuGetPackages(project, new[] { OpenSilver }, null, new[] { XRSharp });
        }
    }
}
