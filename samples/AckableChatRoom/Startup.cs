// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Microsoft.Azure.SignalR.Samples.AckableChatRoom
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSignalR()
                    .AddAzureSignalR(options =>
                    {
                        //  This is a tircky way to associate user name with connection for sample purpose.
                        //  For PROD, we suggest to use authentication and authorization, see here:
                        //  https://docs.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-2.2
                        options.ClaimsProvider = context => new[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, context.Request.Query["username"])
                        };
                    });

            services.AddSingleton<IAckHandler, AckHandler>();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            app.UseFileServer();
            app.UseAzureSignalR(routes =>
            {
                routes.MapHub<AckableChatSampleHub>("/chat");
            }
            );
        }
    }
}