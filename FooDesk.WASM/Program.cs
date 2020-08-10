using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FooDesk.WASM
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("FooDesk", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("FooDesk"));

            builder.Services.AddAuthorizationCore(config =>
            {
                config.AddPolicy("Admin", policy =>
                {
                    policy.RequireRole("Admin");
                });
                config.AddPolicy("User", policy =>
                {
                    policy.RequireRole("User");
                });
            });
            builder.Services.AddOidcAuthentication(options =>
            {
                options.UserOptions.RoleClaim = "role";
                builder.Configuration.Bind("identityserver", options.ProviderOptions);
            });

            await builder.Build().RunAsync();
        }
    }
}
