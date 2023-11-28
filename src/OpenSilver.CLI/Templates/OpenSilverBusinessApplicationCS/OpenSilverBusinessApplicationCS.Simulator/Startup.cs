using OpenSilver.Simulator;
using System;

namespace OpenSilverBusinessApplicationCS.Simulator
{
    internal static class Startup
    {
        [STAThread]
        private static int Main(string[] args)
        {
            return SimulatorLauncher.Start(typeof(App));
        }
    }
}