using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Configuration;
using OpenSilver.WebAssembly;
using System;
using System.Threading.Tasks;

namespace $ext_safeprojectname$.Browser.Pages
{
    [Route("/")]
    public class Index : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await Runner.RunApplicationAsync(() =>
                new $ext_safeprojectname$.App(ServiceProvider, Configuration)
            );
        }

        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }
    }
}