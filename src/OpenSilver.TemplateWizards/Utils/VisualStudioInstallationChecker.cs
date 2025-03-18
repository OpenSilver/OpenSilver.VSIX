using Microsoft.VisualStudio.Setup.Configuration;
using System.Linq;

namespace OpenSilver.TemplateWizards.Utils
{
    public class VisualStudioInstallationChecker
    {
        public bool IsMauiInstalled()
        {
            var query = new SetupConfiguration();
            var enumInstances = query.EnumAllInstances();

            int fetched;
            var instances = new ISetupInstance[1];

            do
            {
                enumInstances.Next(1, instances, out fetched);
                if (fetched > 0)
                {
                    var instance2 = (ISetupInstance2)instances[0];
                    var packages = instance2.GetPackages();

                    bool mauiInstalled = packages.Any(package => package.GetId().Contains("Microsoft.VisualStudio.Workload.NetCrossPlat"));

                    if (mauiInstalled)
                    {
                        return true;
                    }
                }
            }
            while (fetched > 0);

            return false;
        }
    }
}
