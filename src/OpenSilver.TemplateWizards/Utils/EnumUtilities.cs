using System;
using System.ComponentModel;
using System.Reflection;

namespace OpenSilver.TemplateWizards.Utils
{
    internal class EnumUtilities
    {
        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attr = field.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
