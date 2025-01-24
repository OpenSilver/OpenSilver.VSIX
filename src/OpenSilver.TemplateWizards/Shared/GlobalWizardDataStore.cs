using System;
using System.Collections.Concurrent;

namespace OpenSilver.TemplateWizards.Shared
{
    public static class GlobalWizardDataStore
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, object>> _dataBySolution
            = new ConcurrentDictionary<string, ConcurrentDictionary<string, object>>(StringComparer.OrdinalIgnoreCase);

        public static ConcurrentDictionary<string, object> GetSharedData(string key)
        {
            return _dataBySolution.GetOrAdd(key, _ => new ConcurrentDictionary<string, object>());
        }
    }
}
