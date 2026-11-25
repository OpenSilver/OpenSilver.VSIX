using Microsoft.VisualStudio.Shell;
using System;
using System.Globalization;

namespace OpenSilver.XamlDesigner
{
    /// <summary>
    /// Writes "DeferUntilIntellisenseIsReady"=0 for the editor factory.
    /// When the value is missing, VS opens the document in a placeholder frame and later calls
    /// OpenSpecificEditor with this factory only, so returning VS_E_UNSUPPORTEDFORMAT for a
    /// non-OpenSilver project (e.g. WPF) no longer falls back to the next editor and the
    /// "IntelliSense finished initializing, but an error occurred..." page is shown instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class ProvideEditorNoDeferAttribute : RegistrationAttribute
    {
        private const string DeferUntilIntellisenseIsReady = "DeferUntilIntellisenseIsReady";

        private readonly Guid _editorFactoryGuid;

        public ProvideEditorNoDeferAttribute(Type editorFactoryType)
        {
            _editorFactoryGuid = editorFactoryType.GUID;
        }

        private string EditorRegKey => string.Format(CultureInfo.InvariantCulture, "Editors\\{0:B}", _editorFactoryGuid);

        public override void Register(RegistrationContext context)
        {
            using (Key key = context.CreateKey(EditorRegKey))
            {
                key.SetValue(DeferUntilIntellisenseIsReady, 0);
            }
        }

        public override void Unregister(RegistrationContext context)
        {
            context.RemoveValue(EditorRegKey, DeferUntilIntellisenseIsReady);
        }
    }
}
