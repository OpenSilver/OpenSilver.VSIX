using DotNetForHtml5;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.JSInterop;
using $ext_safeprojectname$.Browser.Interop;
using System;

namespace $ext_safeprojectname$.Browser.Pages
{
    [Route("/")]
    public class Index : ComponentBase
    {
        protected override void BuildRenderTree(RenderTreeBuilder __builder)
        {
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Cshtml5Initializer.Initialize(new UnmarshalledJavaScriptExecutionHandler(JSRuntime));
            Program.RunApplication(ServiceProvider, Configuration);
        }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }

        [Inject]
        private IConfiguration Configuration { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }
    }
}