using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SimpleCmsWebApi.Authentication
{
    public static class SecretTokenAuthenitcatoinServicesExtension
    {
        public static void AddSecretTokenAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(SecretTokenAuthenticationOptions.SchemeName)
                .AddScheme<SecretTokenAuthenticationOptions, SecretTokenAuthenticationHandler>(SecretTokenAuthenticationOptions.SchemeName,
                    options =>
                    {
                        options.SuperSecretToken = configuration.GetValue<string>("SuperSercretToken");
                    });

            services.AddAuthorization(options =>
            {
                var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(SecretTokenAuthenticationOptions.SchemeName);
                defaultAuthorizationPolicyBuilder = defaultAuthorizationPolicyBuilder.RequireClaim("token");
                options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
            });
        }
    }
}
