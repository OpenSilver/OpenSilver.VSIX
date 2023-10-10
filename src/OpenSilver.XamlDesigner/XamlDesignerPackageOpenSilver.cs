using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using Userware.XamlDesigner.SplitXamlEditor;

namespace OpenSilver.XamlDesigner
{
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is a package:
    [PackageRegistration(UseManagedResourcesOnly = true)]

    // This attribute is used to register the information needed to show this package in the Help/About dialog of Visual Studio:
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]

    // This attribute is needed to let the shell know that this package exposes some menus:
    [ProvideMenuResource("Menus.ctmenu", 1)]

    // Register the editor:
    [ProvideEditorExtension(typeof(EditorFactoryOpenSilver), BaseEditorFactory.Extension, 2300, NameResourceID = 113)]

    // Key binding table:
    [ProvideKeyBindingTable(GuidList.guidEditorFactoryString, 102)]

    // We register that our editor supports LOGVIEWID_Designer logical view:
    [ProvideEditorLogicalView(typeof(EditorFactoryOpenSilver), LogicalViewID.Designer)]

    // Our Editor supports Find and Replace therefore we need to declare support for LOGVIEWID_TextView.
    // This attribute declares that your EditorPane class implements IVsCodeWindow interface
    // used to navigate to find results from a "Find in Files" type of operation.
    [ProvideEditorLogicalView(typeof(EditorFactoryOpenSilver), LogicalViewID.TextView)]

    [Guid(GuidList.guidEditorPkgString)]
    public sealed class XamlDesignerPackageOpenSilver : BasePackage
    {
        protected override BaseEditorFactory GetEditorFactory()
        {
            return new EditorFactoryOpenSilver(this);
        }
    }
}
