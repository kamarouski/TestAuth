using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace TestAuth
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddAuthentication();
            services.AddDistributedMemoryCache();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
            // Add a new middleware validating access tokens issued by the server.
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Audience = "resource_server_1",
                Authority = "http://localhost:10577/",
                RequireHttpsMetadata = false
            });

            // Add a new middleware issuing tokens.
            app.UseOpenIdConnectServer(options =>
            {
                options.TokenEndpointPath = "/api/token";
                // Disable the HTTPS requirement.
                options.AllowInsecureHttp = true;

                // Disable the authorization endpoint as it's not used in this scenario.
                options.AuthorizationEndpointPath = PathString.Empty;

                options.Provider = new AuthorizationProvider();

                // Force the OpenID Connect server middleware to use JWT tokens
                // instead of the default opaque/encrypted format.
                options.UseJwtTokens();
            });
        }
    }
}
