using System;
using System.Net.Http;
using System.Threading.Tasks;

using Blazored.LocalStorage;

using Blazorise;
using Blazorise.Frolic;
using Blazorise.Icons.FontAwesome;

using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace BlazeSnes {
    public class Program {
        public static async Task Main(string[] args) {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.Services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = true;
                } )
                .AddFrolicProviders()
                .AddFontAwesomeIcons()
                .AddBlazorContextMenu()
                .AddBlazoredLocalStorage(config => config.JsonSerializerOptions.WriteIndented = true);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            await builder.Build().RunAsync();
        }
    }
}